using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using System.Threading;
using ECCentral.Service.EventMessage.RMA;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(RefundProcessor))]
    public class RefundProcessor
    {
        private IRefundDA refundDA = ObjectFactory<IRefundDA>.Instance;
        private IGiftCardRMARedeemLogDA giftCardRMARedeemLogDA = ObjectFactory<IGiftCardRMARedeemLogDA>.Instance;
        private SOInfo soInfo;
        private readonly decimal pointExchangeRate = ExternalDomainBroker.GetPointToMoneyRatio();

        /// <summary>
        /// 创建退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo Create(RefundInfo entity)
        {
            List<RMARegisterInfo> registers;

            PreCheckCreate(entity, out registers);

            if (registers != null && registers.Count > 0)
            {
                entity.PriceprotectPoint = ExternalDomainBroker.GetPriceprotectPoint(entity.SOSysNo.Value,
                registers.Select(item => item.BasicInfo.ProductSysNo.Value).ToList());

                entity.HasPriceprotectPoint = true;
            }
            entity.CustomerSysNo = soInfo.BaseInfo.CustomerSysNo;
            entity.CompensateShipPrice = 0;
            entity.InvoiceLocation = registers[0].BasicInfo.LocationWarehouse;
            var invoice = ExternalDomainBroker.GetSOInvoiceMaster(entity.SOSysNo.Value);
            entity.SOInvoiceNo = invoice.Count == 0 ? string.Empty : invoice[0].InvoiceNo;
            entity.Status = RMARefundStatus.WaitingRefund;


            // 5、构建 Refund Item 信息
            entity.RefundItems.Clear();
            entity.RefundItems = GetRefundItems(registers);

            // 5、计算相关费用
            this.Calc(entity, this.soInfo);

            // 6、设置退款方式
            SetRefundPayType(entity, registers);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity.SysNo = this.refundDA.CreateSysNo();
                entity.RefundID = this.GenerateRefundID(entity.SysNo.Value);

                this.refundDA.InsertMaster(entity);

                entity.RefundItems.ForEach(item =>
                {
                    item.RefundSysNo = entity.SysNo;
                    item.CompanyCode = entity.CompanyCode;
                    this.refundDA.InsertItem(item);
                });


                // 完成RMA申请单待审核 - 待办事项:
                if (null != registers && registers.Count > 0)
                {
                    RMARequestInfo getRequest = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(registers[0].SysNo.Value);
                    if (null != getRequest)
                    {
                        EventPublisher.Publish<RMACompleteRequestWaitingForAuditMessage>(new RMACompleteRequestWaitingForAuditMessage()
                        {
                            RequestSysNo = getRequest.SysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                    }
                }

                // 创建RMA退款单待提交审核 - 待办事项:
                EventPublisher.Publish<RMACreateRefundWaitingForSubmitMessage>(new RMACreateRefundWaitingForSubmitMessage()
                {
                    RefundSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("创建退款单", BizLogType.RMA_Refund_Create, entity.SysNo.Value, entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 更新退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(RefundInfo entity)
        {
            this.PreCheckUpdate(entity);

            RefundInfo refundInfo = LoadBySysNo(entity.SysNo.Value);
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo newIncomeBankInfo = new SOIncomeRefundInfo()
            {
                OrderType = RefundOrderType.RO,
                OrderSysNo = refundInfo.SysNo.Value,
                SOSysNo = refundInfo.SOSysNo.Value,
                RefundPayType = entity.RefundPayType,
                BankName = entity.IncomeBankInfo.BankName,
                BranchBankName = entity.IncomeBankInfo.BranchBankName,
                CardNumber = entity.IncomeBankInfo.CardNumber,
                CardOwnerName = entity.IncomeBankInfo.CardOwnerName,
                PostAddress = entity.IncomeBankInfo.PostAddress,
                PostCode = entity.IncomeBankInfo.PostCode,
                ReceiverName = entity.IncomeBankInfo.ReceiverName,
                Note = entity.IncomeBankInfo.Note,
                RefundReason = entity.RefundReason,
                HaveAutoRMA = false,
                Status = RefundStatus.WaitingRefund,
                RefundGiftCard = refundInfo.GiftCardAmt,
                CompanyCode = refundInfo.CompanyCode
            };

            int affectedPoint = 0;
            affectedPoint = -1 * (refundInfo.DeductPointFromAccount ?? 0) + (refundInfo.PointAmt ?? 0);
            if (entity.RefundPayType == RefundPayType.TransferPointRefund)
            {
                affectedPoint += Convert.ToInt32(Decimal.Round((refundInfo.CashAmt ?? 0M) * pointExchangeRate, 0));

                newIncomeBankInfo.RefundPoint = affectedPoint;
                newIncomeBankInfo.RefundCashAmt = 0;
            }
            else
            {
                newIncomeBankInfo.RefundPoint = affectedPoint;
                newIncomeBankInfo.RefundCashAmt = refundInfo.CashAmt;
            }
            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO);
            if (incomeBankInfo != null)
            {
                newIncomeBankInfo.SysNo = incomeBankInfo.SysNo;
            }

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo.Value,
                RefundPayType = entity.RefundPayType,
                CashFlag = entity.CashFlag,
                Note = entity.Note,
                FinanceNote = entity.FinanceNote,
                SOInvoiceNo = entity.SOInvoiceNo,
                InvoiceLocation = entity.InvoiceLocation,
                RefundReason = entity.RefundReason
            };

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                this.refundDA.UpdateMaster(newEntity);

                if (incomeBankInfo == null)
                {
                    ExternalDomainBroker.CreateSOIncomeRefundInfo(newIncomeBankInfo);
                }
                else
                {
                    ExternalDomainBroker.UpdateSOIncomeRefundInfo(newIncomeBankInfo);
                }

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("更新退款单", BizLogType.RMA_Refund_Upate, entity.SysNo.Value, refundInfo.CompanyCode);
        }

        /// <summary>
        /// 更新退款单的退款支付方式和退款原因
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="refundReason">退款原因</param>
        public virtual void UpdateRefundPayTypeAndReason(int sysNo, int refundPayType, int refundReason)
        {
            refundDA.UpdateRefundPayTypeAndReason(sysNo, refundPayType, refundReason);
        }

        /// <summary>
        /// 重新计算退款金额
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo Calculate(RefundInfo entity)
        {
            this.PreCheckCalculate(entity);

            List<RefundItemInfo> entityItems = entity.RefundItems;

            entity.RefundItems = this.refundDA.GetItemsByRefundSysNo(entity.SysNo.Value, entity.CompanyCode);
            entity.RefundItems.ForEach(item =>
            {
                RefundItemInfo itemEntity = entityItems.Find(entityItem =>
                {
                    return entityItem.SysNo == item.SysNo;
                });
                item.RefundPriceType = itemEntity.RefundPriceType;
                item.RefundPrice = itemEntity.RefundPrice;
            });

            if (!entity.HasPriceprotectPoint)
            {
                entity.PriceprotectPoint = 0;
            }
            else
            {
                entity.PriceprotectPoint = GetPriceProtectPoint(entity.SysNo.Value, entity.SOSysNo.Value, entity.CompanyCode);
            }

            this.Calc(entity, this.soInfo);

            //Clone RefundInfo
            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo,
                CompensateShipPrice = entity.CompensateShipPrice,
                OrgCashAmt = entity.OrgCashAmt,
                OrgPointAmt = entity.OrgPointAmt,
                DeductPointFromAccount = entity.DeductPointFromAccount,
                DeductPointFromCurrentCash = entity.DeductPointFromCurrentCash,
                CashAmt = entity.CashAmt,
                CashFlag = entity.CashFlag,
                PointAmt = entity.PointAmt,
                RefundPayType = entity.RefundPayType,
                OrgGiftCardAmt = entity.OrgGiftCardAmt,
                GiftCardAmt = entity.GiftCardAmt,
                PriceprotectPoint = entity.PriceprotectPoint
            };
            if (entity.RefundItems != null)
            {
                newEntity.RefundItems = new List<RefundItemInfo>();

                entity.RefundItems.ForEach(item =>
                {
                    newEntity.RefundItems.Add(new RefundItemInfo()
                    {
                        SysNo = item.SysNo,
                        RefundPrice = item.RefundPrice,
                        RefundPriceType = item.RefundPriceType,
                        RefundCash = item.RefundCash,
                        RefundPoint = item.RefundPoint,
                        OrgGiftCardAmt = item.OrgGiftCardAmt
                    });
                });
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                this.refundDA.UpdateMasterForCalc(newEntity);

                newEntity.RefundItems.ForEach(item => this.refundDA.UpdateItemForCalc(item));

                scope.Complete();
            }

            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO);

            SOIncomeRefundInfo newIncomeBankInfo = new SOIncomeRefundInfo()
            {
                OrderType = RefundOrderType.RO,
                Status = RefundStatus.Origin,
                OrderSysNo = entity.SysNo.Value,
                SOSysNo = entity.SOSysNo.Value,
                RefundPayType = entity.RefundPayType,
                BankName = entity.IncomeBankInfo.BankName,
                BranchBankName = entity.IncomeBankInfo.BranchBankName,
                CardNumber = entity.IncomeBankInfo.CardNumber,
                CardOwnerName = entity.IncomeBankInfo.CardOwnerName,
                PostAddress = entity.IncomeBankInfo.PostAddress,
                PostCode = entity.IncomeBankInfo.PostCode,
                ReceiverName = entity.IncomeBankInfo.ReceiverName,
                RefundReason = entity.IncomeBankInfo.RefundReason,
                CompanyCode = entity.CompanyCode,
            };

            if (incomeBankInfo == null)
            {
                if (CheckSOIncomeRefundInfo(newIncomeBankInfo))
                {
                    ExternalDomainBroker.CreateSOIncomeRefundInfo(newIncomeBankInfo);
                }
            }
            else
            {
                newIncomeBankInfo.Status = incomeBankInfo.Status;//RefundStatus.Origin;
                newIncomeBankInfo.SysNo = incomeBankInfo.SysNo;
                newIncomeBankInfo.Note = incomeBankInfo.Note;
                ExternalDomainBroker.UpdateSOIncomeRefundInfo(newIncomeBankInfo);
            }

            ExternalDomainBroker.CreateOperationLog("更新退款单", BizLogType.RMA_Refund_Upate, entity.SysNo.Value, entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 更新财务备注
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo UpdateFinanceNote(RefundInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (string.IsNullOrEmpty(entity.FinanceNote))
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_FinanceNoteRequired");
                throw new BizException(msg);
            }

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo,
                FinanceNote = entity.FinanceNote
            };

            this.refundDA.UpdateMaster(newEntity);

            ExternalDomainBroker.CreateOperationLog("更新退款单", BizLogType.RMA_Refund_Upate, entity.SysNo.Value, entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 作废退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo Abandon(int sysNo)
        {
            RefundInfo refundInfo = LoadBySysNo(sysNo);
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            if (refundInfo.Status != RMARefundStatus.WaitingRefund)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Abandon_InvalidStatus");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo newIncomeBankInfo = null;

            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(sysNo, RefundOrderType.RO);
            if (incomeBankInfo != null)
            {
                if (incomeBankInfo.Status == RefundStatus.Audit)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Abandon_IncomeBankInfo_VerifyPass_InvalidStatus");
                    throw new BizException(msg);
                }

                newIncomeBankInfo = new SOIncomeRefundInfo()
                {
                    SysNo = incomeBankInfo.SysNo,
                    Status = RefundStatus.Abandon
                };
            }

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = sysNo,
                Status = RMARefundStatus.Abandon
            };

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                this.refundDA.UpdateMaster(newEntity);

                // 更新对应 Register 的 RefundStatus 为 Null，使 Register 能够再次创建 Refund
                this.refundDA.BatchUpdateRegisterRefundStatus(sysNo, null);

                if (newIncomeBankInfo != null)
                {
                    ExternalDomainBroker.AbandonSOIncomeRefundForRO(newIncomeBankInfo.SysNo.Value);
                }


                //20130808 Chester Added:完成RMA退款单待提交审核 - 待办事项
                EventPublisher.Publish<RMACompleteRefundWaitingForSubmitMessage>(new RMACompleteRefundWaitingForSubmitMessage()
                {
                    RefundSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("作废退款单", BizLogType.RMA_Refund_Abandon, sysNo, refundInfo.CompanyCode);

            return LoadWithItemsBySysNo(sysNo);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo SubmitAudit(RefundInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }

            entity.Status = RMARefundStatus.WaitingAudit;

            RefundInfo refundInfo = LoadBySysNo(entity.SysNo.Value);
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }

            if (entity.CashFlag != CashFlagStatus.Yes && entity.RefundPayType != RefundPayType.NetWorkRefund)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_NewWorkRefundNoCash");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO);
            if (incomeBankInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_Empty");
                throw new BizException(msg);
            }

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo,
                Status = entity.Status,
                AuditTime = DateTime.Now,
                AuditUserSysNo = ServiceContext.Current.UserSysNo
            };

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                UpdateSOIncomeRefundInfo(refundInfo, incomeBankInfo);

                this.refundDA.UpdateMaster(newEntity);

                // 更新对应 Register 的 RefundStatus 为 WaitingAudit
                this.refundDA.BatchUpdateRegisterRefundStatus(newEntity.SysNo.Value, RMARefundStatus.WaitingAudit);

                //不涉及现金则自动审核通过，否则需要财务那边人工审核
                //如果先点击计算的话，这时的CashFlag为null值，也能确保是未勾选涉及金额，所以加以下判断 Norton 2012.12.6
                if (refundInfo.CashFlag == null || refundInfo.CashFlag == CashFlagStatus.No)
                {
                    ExternalDomainBroker.AutoAuditSOIncomeRefundForRO(incomeBankInfo.SysNo.Value);
                }
                else
                {
                    ExternalDomainBroker.SubmitAuditSOIncomeRefundForRO(incomeBankInfo.SysNo.Value);
                }

                //201300808 Chester Added:完成RMA退款单待提交审核 - 待办事项
                EventPublisher.Publish<RMACompleteRefundWaitingForSubmitMessage>(new RMACompleteRefundWaitingForSubmitMessage()
                {
                    RefundSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("提交审核退款单", BizLogType.RMA_Refund_Audit, entity.SysNo.Value, refundInfo.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 取消提交审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo CancelSubmitAudit(RefundInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }

            entity.Status = RMARefundStatus.WaitingRefund;

            RefundInfo refundInfo = LoadBySysNo(entity.SysNo.Value);
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            if (refundInfo.Status != RMARefundStatus.WaitingAudit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelSubmitAudit_InvalidStatus");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO);
            if (incomeBankInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_Empty");
                throw new BizException(msg);
            }
            if (incomeBankInfo.Status == RefundStatus.Abandon)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelSubmitAudit_IncomeBankInfo_AbandonStatus");
                throw new BizException(msg);
            }
            if (incomeBankInfo.Status == RefundStatus.Audit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelSubmitAudit_IncomeBankInfo_VerifyPassStatus");
                throw new BizException(msg);
            }
            if (incomeBankInfo.Status != RefundStatus.Origin)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelSubmitAudit_IncomeBankInfo_NotApplyStatus");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo newIncomeBankInfo = new SOIncomeRefundInfo()
            {
                SysNo = incomeBankInfo.SysNo,
                Status = RefundStatus.WaitingRefund
            };

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo,
                Status = entity.Status
            };

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                this.refundDA.UpdateMaster(newEntity);

                // 更新对应 Register 的 RefundStatus 为 WaitingRefund
                this.refundDA.BatchUpdateRegisterRefundStatus(newEntity.SysNo.Value, RMARefundStatus.WaitingRefund);

                ExternalDomainBroker.CancelSubmitAuditSOIncomeRefundForRO(newIncomeBankInfo.SysNo.Value);

                //201300808 Chester Added 创建RMA退款单提交审核 - 待办事项:
                EventPublisher.Publish<RMACreateRefundWaitingForSubmitMessage>(new RMACreateRefundWaitingForSubmitMessage()
                {
                    RefundSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                //2013060808 Chester Added:RO单取消提交审核 - 待办事项
                EventPublisher.Publish<RMAROCancelSubmitMessage>(new RMAROCancelSubmitMessage()
                {
                    SOIncomeRefundSysNo = incomeBankInfo.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("取消提交审核退款单", BizLogType.RMA_Refund_CancelAudit, entity.SysNo.Value, refundInfo.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundInfo Refund(int sysNo)
        {
            #region 业务说明

            /*1、首先必须有退款单(RMA_Refund)，并且退款单的状态为待审核，
                [Description("作废")]
                Abandon = -1,
                [Description("待退款")]
                WaitingRefund = 0,
                [Description("已退款")]
                Refunded = 2,
                [Description("待审核")]
                WaitingAudit = 3
            2、检验是否有对应的退款审核单据，并且状态为审核通过（IPP3.dbo.Finance_SOIncome_BankInfo）
            退款审核状态
                -1 -- 审核拒绝
                 0 -- 待审核
                 1 -- 审核通过
                 2 -- 待RMA退款
            3、当需要从客户的积分账户里扣除积分，而客户的积分账户又余额不足时，需要将不足的积分转换成金额扣除（DeductPointFromCurrentCash），会
             检查该退款单对应的订单是否已经有对应的收款单，并且收款单的状态为已确认。（IPP3.dbo.Finance_SOIncome）
            收款单状态
                -1 -- Abandon
                0  -- Origin
                1  -- Confirmed
            4、检查本次退款后，每类商品的退款总额是否大于订单中每类商品总额
             * 先找出本次Refund所涉及到得产品在订单里分别销售的总额
             * 再找出已经退款的和本次退款的各个产品的总额
             * 如果后者大于前者则抛异常
            5、如果 应该退给客户的总的积分-该从客户的积分账户里扣除的积分 != 0,则调用调整积分的服务
            6、如果转积分退款，会将所有需要退的积分和金钱都转成积分的形式退到客户的积分账户上
            7、如果是推入帐户余额，则调用Customer BalanceAccount服务
            8、更新RMA_Refund表的状态为 已退款
            9、获取本Refund涉及到的所有单件，判断器状态是否为已经发还给顾客，如果已经发还则终止此退款
            10、获取每一个RefundItem的成本,更新RMA_Refund_Item表里的成本
            11、更新相应单件的状态为已退款，和更新成本，如果是延保产品则关闭此单件
            12、如果不是延保产品，则更新RMA_Inventory 的库存
            13、并且记录库存修改的日志
            14、在Finance_SOIncome表里添加相关的财务记录，如果不涉及现金则自动 Confirm
            15、记录Refund日志
            16、如果是北京仓 或 广州仓 或 西安仓 或 订单是增票， 则往税控中心发送消息
          */

            #endregion 业务说明

            RefundInfo refundInfo = LoadBySysNo(sysNo);

            PreCheckRefund(refundInfo);

            CheckConflictRefund(refundInfo);

            CheckSOItemRefundAmt(refundInfo);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //积分撤销
                ReturnProductPoint(refundInfo);

                //转积分调整
                RefundCashToPoints(refundInfo);

                //退余额账户
                RefundPrepay(refundInfo);

                //礼品卡相关,退到原礼品卡或新礼品卡
                refundInfo.GiftCardAmt = refundInfo.GiftCardAmt ?? 0M;
                if (refundInfo.GiftCardAmt > 0)
                {
                    RefundGiftCard(refundInfo, refundInfo.GiftCardAmt.Value, "RMARefund", refundInfo.SysNo.Value);
                }
                if (refundInfo.RefundPayType == RefundPayType.GiftCardRefund && refundInfo.CashAmt.Value > 0)
                {
                    CreateElectronicGiftCard(refundInfo, refundInfo.CashAmt.Value, "RO");
                }

                //更改RO、单件状态

                refundInfo.Status = RMARefundStatus.Refunded;
                UpdateRefundStatus(refundInfo);

                UpdateRefundItemCostAndRegister(refundInfo);

                //创建负的收款单信息
                CreateSOIncome(refundInfo);

                scope.Complete();
            }

            //记录refund日志
            ExternalDomainBroker.CreateOperationLog("对退款单进行退款", BizLogType.RMA_Refund_Refund, refundInfo.SysNo.Value, refundInfo.CompanyCode);

            //给仓库发送信息
            SendInvoiceMessageToSSB(refundInfo);

            return refundInfo;
        }

        /// <summary>
        /// 取消退款
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>退款单信息</returns>
        public virtual RefundInfo CancelRefund(int sysNo)
        {
            CustomerPrepayLog prepayLogInfo = null;
            AdjustPointRequest itemPointInfo = null;
            AdjustPointRequest transferPointInfo = null;

            RefundInfo refundInfo = this.refundDA.GetMasterBySysNo(sysNo);
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            if (refundInfo.Status != RMARefundStatus.Refunded)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelRefund_InvalidStatus");
                throw new BizException(msg);
            }

            SOIncomeInfo incomeInfo = ExternalDomainBroker.GetValidSOIncomeInfo(sysNo, SOIncomeOrderType.RO);
            if (incomeInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelRefund_IncomeInfo_NotExists");
                throw new BizException(msg);
            }
            if (incomeInfo.Status == SOIncomeStatus.Confirmed)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_CancelRefund_IncomeInfo_InvalidStatus");
                throw new BizException(msg);
            }
            refundInfo.Status = RMARefundStatus.WaitingAudit;

            // 1、更新用户积分
            int affectedPoint1 = 0;
            int affectedPoint2 = 0;

            affectedPoint1 = -1 * (refundInfo.DeductPointFromAccount ?? 0) + (refundInfo.PointAmt ?? 0);

            if (affectedPoint1 != 0)
            {
                itemPointInfo = new AdjustPointRequest()
                {
                    Source = "RMA",
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    Point = affectedPoint1 * -1,
                    PointType = (int)AdjustPointType.ReturnProductPoint,
                    OperationType = AdjustPointOperationType.Abandon,
                    Memo = sysNo.ToString(),
                    SOSysNo = refundInfo.SOSysNo ?? 0
                };
            }
            if (refundInfo.RefundPayType == RefundPayType.TransferPointRefund)
            {
                affectedPoint2 = Convert.ToInt32(Decimal.Round((refundInfo.CashAmt ?? 0) * pointExchangeRate, 0));
            }
            if (affectedPoint2 != 0)
            {
                transferPointInfo = new AdjustPointRequest()
                {
                    Source = "RMA",
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    Point = affectedPoint2 * -1,
                    PointType = (int)AdjustPointType.RefundCashToPoints,
                    OperationType = AdjustPointOperationType.AddOrReduce,
                    Memo = sysNo.ToString(),
                    SOSysNo = refundInfo.SOSysNo ?? 0
                };
            }

            // 退入余额帐户
            if (refundInfo.RefundPayType == RefundPayType.PrepayRefund)
            {
                prepayLogInfo = new CustomerPrepayLog()
                {
                    SOSysNo = refundInfo.SOSysNo.Value,
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    AdjustAmount = (refundInfo.CashAmt ?? 0) * -1,
                    PrepayType = PrepayType.ROReturn,
                    Note = "取消RMA退款单退入余额账户"
                };
            }

            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = sysNo,
                Status = refundInfo.Status
            };

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                this.refundDA.UpdateMaster(newEntity);

                // 获取 Refund 对应 Registers 数据
                List<RegisterForRefund> registers = this.refundDA.GetRegistersForRefund(newEntity.SysNo.Value);
                if (registers != null)
                {
                    registers.ForEach(item =>
                    {
                        // Update Register Item
                        RMARegisterInfo rrItem = new RMARegisterInfo();
                        rrItem.BasicInfo.Cost = null;
                        rrItem.SysNo = item.RegisterSysNo;
                        rrItem.BasicInfo.RefundStatus = RMARefundStatus.WaitingAudit;
                        rrItem.BasicInfo.OwnBy = RMAOwnBy.Customer;
                        rrItem.BasicInfo.OwnByWarehouse = item.SalesWarehouse;

                        ObjectFactory<IRegisterDA>.Instance.UpdateRegisterAfterRefund(rrItem);
                    });
                }

                if (itemPointInfo != null)//item积分撤消
                {
                    ExternalDomainBroker.AdjustPoint(itemPointInfo);
                }
                if (transferPointInfo != null)//转积分调整
                {
                    ExternalDomainBroker.AdjustPoint(transferPointInfo);
                }
                if (prepayLogInfo != null)
                {
                    ExternalDomainBroker.AdjustPrePay(prepayLogInfo);
                }
                if (incomeInfo != null)
                {
                    ExternalDomainBroker.AbandonSOIncome(incomeInfo.SysNo.Value);
                }

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("对退款单进行取消退款", BizLogType.RMA_Refund_CancelRefund, sysNo, refundInfo.CompanyCode);

            return refundInfo;
        }

        /// <summary>
        /// 获取退款单对应的单件信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public virtual List<RegisterForRefund> GetRegistersForRefund(int? SysNo)
        {
            return this.refundDA.GetRegistersForRefund(SysNo.Value);
        }

        /// <summary>
        /// 生成新礼品卡
        /// </summary>
        /// <param name="refundInfo">退款单实体</param>
        /// <param name="cashAmount">退款金额</param>
        /// <param name="memo">备注</param>
        public virtual void CreateElectronicGiftCard(RefundInfo refundInfo, decimal cashAmount, string memo)
        {
            string statusCode = ExternalDomainBroker.CreateElectronicGiftCard(refundInfo.SOSysNo.Value, refundInfo.CustomerSysNo.Value, 1, cashAmount, memo, refundInfo.CompanyCode);

            if (!string.IsNullOrEmpty(statusCode))
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardCreateFailed");
                throw new BizException(msg);
            }
            GiftCardInfo giftCardInfo = ExternalDomainBroker.GetGiftCardInfoByReferenceSOSysNo(refundInfo.SOSysNo.Value, refundInfo.CustomerSysNo.Value, GiftCardType.Refund, CardMaterialType.Electronic);
            giftCardRMARedeemLogDA.Create(new GiftCardRMARedeemLog()
            {
                Code = giftCardInfo.CardCode,
                CustomerSysNo = refundInfo.CustomerSysNo,
                Amount = cashAmount,
                RefundSysNo = refundInfo.SysNo.Value,
                SOSysNo = refundInfo.SOSysNo.Value,
                Status = "A",
                Memo = "ActiveElectronicCard From " + memo,
                CurrencySysNo = 1,
            });

            SendEmail(cashAmount, giftCardInfo, refundInfo.CustomerSysNo.Value);
        }

        /// <summary>
        /// 对支付礼品卡列表依次退款
        /// </summary>
        /// <param name="refundInfo">退款单实体</param>
        /// <param name="giftCardAmt">退款金额</param>
        /// <param name="actionName">接口名称</param>
        public virtual void RefundGiftCard(RefundInfo refundInfo, decimal giftCardAmt, string actionName, int referenceSysno)
        {
            #region 获取礼品卡使用记录

            List<GiftCardRedeemLog> giftCardRedeemLogList =
                ExternalDomainBroker.GetGiftCardRedeemLog(refundInfo.SOSysNo.Value, ActionType.SO);

            if (giftCardRedeemLogList == null || giftCardRedeemLogList.Count < 1)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRedeemLogNotExists");
            }

            #endregion 获取礼品卡使用记录

            #region 获取历史退款记录

            List<GiftCardRMARedeemLog> giftCardRMARedeemLogList =
                giftCardRMARedeemLogDA.GetGiftCardRMARedeemLogBySOSysNo(refundInfo.SOSysNo.Value);

            #endregion 获取历史退款记录

            #region 如果已对某些卡已进行退款，则进行数据更正

            if (giftCardRMARedeemLogList != null && giftCardRMARedeemLogList.Count > 0)
            {
                giftCardRMARedeemLogList.ForEach(item =>
                {
                    var redeemLog = giftCardRedeemLogList.Find(findItem =>
                    {
                        return findItem.Code == item.Code;
                    });
                    if (redeemLog != null)
                    {
                        redeemLog.Amount += item.Amount;//因为该记录为负，所以变为+
                        if (redeemLog.Amount < 0)
                        {
                            redeemLog.Amount = 0;
                        }
                    }
                });
            }

            if (giftCardRedeemLogList.Sum(item => item.Amount) <= 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRefundTooMuch");
                throw new BizException(msg);
            }

            #endregion 如果已对某些卡已进行退款，则进行数据更正

            #region 构建退卡列表

            List<GiftCard> paramGiftCardRefundlist =
                new List<GiftCard>();

            foreach (var item in giftCardRedeemLogList)
            {
                if (item.Amount == 0)
                {
                    continue;
                }
                if (giftCardAmt <= 0)//礼品卡退款金额
                {
                    break;
                }
                decimal consumeAmount = 0m;

                if (giftCardAmt >= item.Amount)//退款金额大于当前记录金额
                {
                    consumeAmount = item.Amount.Value;
                    giftCardAmt -= item.Amount.Value;//减去单次礼品卡支付
                }
                else//当前记录金额大于退款金额
                {
                    consumeAmount = giftCardAmt;
                    giftCardAmt = 0;
                }
                if (consumeAmount + item.AvailAmount > item.TotalAmount)
                {
                    string msg = string.Format(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRefundCalculateErrorFormat"), item.Code);
                    throw new BizException(msg);
                }
                paramGiftCardRefundlist.Add(new GiftCard()
                {
                    ConsumeAmount = -consumeAmount,
                    Code = item.Code,
                    CustomerSysNo = item.CustomerSysNo.Value,
                    ReferenceSysNo = referenceSysno
                });
            }

            #endregion 构建退卡列表

            #region 调用接口，如成功记录日志

            if (paramGiftCardRefundlist.Count > 0)
            {
                string statusCode = ExternalDomainBroker.GiftCardRMARefund(paramGiftCardRefundlist, refundInfo.CompanyCode);

                if (!string.IsNullOrEmpty(statusCode))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRefundFailed");
                    throw new BizException(msg);
                }
                foreach (var item in paramGiftCardRefundlist)//记录RMA礼品卡退款日志
                {
                    giftCardRMARedeemLogDA.Create(new GiftCardRMARedeemLog()
                    {
                        Code = item.Code,
                        CustomerSysNo = item.CustomerSysNo,
                        Amount = item.ConsumeAmount,
                        RefundSysNo = refundInfo.SysNo.Value,
                        SOSysNo = refundInfo.SOSysNo.Value,
                        Status = "A",
                        Memo = actionName,
                        CurrencySysNo = 1,
                        CompanyCode = refundInfo.CompanyCode
                    });
                }
            }

            #endregion 调用接口，如成功记录日志
        }

        #region 打印专用
        public virtual DataTable GetRefundPrintDetail(int sysNo)
        {
            return ObjectFactory<IRefundQueryDA>.Instance.GetRefundPrintDetail(sysNo);
        }

        public virtual DataTable GetRefundPrintItems(int refundSysNo)
        {
            return ObjectFactory<IRefundQueryDA>.Instance.GetRefundPrintItems(refundSysNo);
        }
        #endregion

        #region 查询

        public virtual int GetRefundSysNoByRegisterSysNo(int registerSysNo)
        {
            return refundDA.GetRefundSysNoByRegisterSysNo(registerSysNo);
        }

        /// <summary>
        /// 根据系统编号查询退款单信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>退款单信息（不包含Items）</returns>
        public RefundInfo LoadBySysNo(int sysNo)
        {
            RefundInfo refund = this.refundDA.GetMasterBySysNo(sysNo);
            if (refund == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            return refund;
        }

        public int GetAutoRMARefundCountBySOSysNo(int soSysNo)
        {
            return refundDA.GetAutoRMARefundCountBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 根据系统编号查询退款单信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>退款单信息（包含Items）</returns>
        public virtual RefundInfo LoadWithItemsBySysNo(int sysNo)
        {
            RefundInfo refund = LoadBySysNo(sysNo);
            refund.RefundItems = refundDA.GetItemsWithProductInfoByRefundSysNo(sysNo);
            return refund;
        }

        /// <summary>
        /// 获取退款原因列表
        /// </summary>
        /// <returns>退款原因列表</returns>
        public virtual List<CodeNamePair> GetRefundReasons()
        {
            List<CodeNamePair> list = refundDA.GetRefundReasons();
            list.ForEach(p => p.Name = p.Name.Trim());
            return list;
        }

        /// <summary>
        /// 查询待退款的订单列表
        /// </summary>
        /// <returns>订单编号列表</returns>
        public virtual List<int> GetWaitingSOForRefund()
        {
            return refundDA.GetWaitingSOForRefund();
        }

        /// <summary>
        /// 获取退款单/退款调整单中的历史退款信息（现金、礼品卡、运费）
        /// </summary>
        /// <param name="soSysNo">So系统编号</param>
        /// <param name="productSysNo">产品系统编号---productSysNo=0代表“运费补偿及其它”</param>
        /// <param name="wareHouseID">仓库编号</param>
        /// <param name="refundSysNo">退款单系统编号</param>
        /// <returns></returns>
        public virtual HistoryRefundAmount GetHistoryRefundAmt(int soSysNo, int productSysNo, string stockID)
        {
            //现金、礼品卡RO查询
            HistoryRefundAmount cashAndGiftCardRO = refundDA.GetRefundCashAmtBySOSysNo(soSysNo,
                RMARefundStatus.Refunded);
            //现金、礼品卡ROBlance查询
            HistoryRefundAmount cashAndGiftCardROBlance = refundDA.GetRO_BalanceCashAmtByOrgSOSysNo(soSysNo,
               RMARefundStatus.Refunded);

            decimal totalGiftAmtHistory = ((cashAndGiftCardRO != null) ? cashAndGiftCardRO.TotalGiftCardAmt : 0M)
                + ((cashAndGiftCardROBlance != null) ? cashAndGiftCardROBlance.TotalGiftCardAmt : 0M);
            int totalPointHistory = ((cashAndGiftCardRO != null) ? cashAndGiftCardRO.TotalPointAmt : 0)
                + ((cashAndGiftCardROBlance != null) ? cashAndGiftCardROBlance.TotalPointAmt : 0);

            //ROBlance本分仓运费
            HistoryRefundAmount shipPriceROBlance =
                refundDA.GetRO_BalanceShipPriceAmtByOrgSOSysNo(soSysNo, RefundBalanceStatus.Refunded, productSysNo, stockID);

            //ROBlance其他分仓运费
            HistoryRefundAmount shipPriceROBlanceOther =
                refundDA.GetRO_BalanceShipPriceAmtByOrgSOSysNoAndOtherStockID(soSysNo, RefundBalanceStatus.Refunded, productSysNo, stockID);

            //ro本分仓运费
            HistoryRefundAmount shipPriceRO =
                refundDA.GetRefundShipPriceBySOSysNoAndStockID(soSysNo, RMARefundStatus.Refunded, stockID);

            //ro其他分仓运费
            HistoryRefundAmount shipPriceROOther =
                refundDA.GetRefundShipPriceBySOSysNoAndOtherStockID(soSysNo, RMARefundStatus.Refunded, stockID);

            decimal totalCashAmtHistory =
                ((cashAndGiftCardRO != null) ? cashAndGiftCardRO.TotalCashAmt : 0M)
                - ((shipPriceROOther != null) ? shipPriceROOther.TotalShipPriceAmt : 0M)
                + ((cashAndGiftCardROBlance != null) ? cashAndGiftCardROBlance.TotalCashAmt : 0M)
                - ((shipPriceROBlanceOther != null) ? shipPriceROBlanceOther.TotalCashAmt : 0M)
                ;

            decimal totalShipPriceAmt = ((shipPriceRO != null) ?
                shipPriceRO.TotalShipPriceAmt : 0M)
                + (
                (shipPriceROBlance != null) ?
                (
                shipPriceROBlance.TotalCashAmt
                + shipPriceROBlance.TotalGiftCardAmt
                + (shipPriceROBlance.TotalPointAmt / pointExchangeRate)
                ) : 0M
                );

            decimal totalRoBalanceAmt =
                (shipPriceROBlance != null) ?
                (
                shipPriceROBlance.TotalCashAmt
                + shipPriceROBlance.TotalGiftCardAmt
                + (shipPriceROBlance.TotalPointAmt / pointExchangeRate)
                ) : 0M;

            return new HistoryRefundAmount()
            {
                TotalCashAmt = totalCashAmtHistory,
                TotalGiftCardAmt = totalGiftAmtHistory,
                TotalShipPriceAmt = totalShipPriceAmt,
                TotalPointAmt = totalPointHistory,
                TotalRoBalanceAmt = totalRoBalanceAmt
            };
        }

        /// <summary>
        /// 获取三费合计，运费，手续费，保价费，已退金额
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="totalAmt"></param>
        /// <param name="premiumAmt"></param>
        /// <param name="shippingCharge"></param>
        /// <param name="payPrice"></param>
        /// <param name="historyRefund"></param>
        public virtual void GetShipFee(RefundInfo entity,
            out decimal totalAmt,
            out decimal premiumAmt,
            out decimal shippingCharge,
            out decimal payPrice,
            out decimal historyRefund)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (entity.SOSysNo == null)
            {
                throw new ArgumentNullException("entity.SOSysNo");
            }

            string warehouseNo = refundDA.GetWarehouseNo(entity.SysNo.Value);

            ExternalDomainBroker.GetShipFee(entity.SOSysNo.Value, warehouseNo, out totalAmt, out premiumAmt, out shippingCharge, out payPrice);

            HistoryRefundAmount historyRefundShipPrice = refundDA.GetRefundShipPriceBySOSysNoAndStockID(entity.SOSysNo.Value, RMARefundStatus.Refunded, warehouseNo);
            HistoryRefundAmount historyROBalanceShipPrice = refundDA.GetRO_BalanceShipPriceAmtByOrgSOSysNo(entity.SOSysNo.Value, RefundBalanceStatus.Refunded, 0, warehouseNo);

            decimal totalShipPriceAmt =
                 ((historyRefundShipPrice != null) ? historyRefundShipPrice.TotalShipPriceAmt : 0M)
                 +
                 ((historyROBalanceShipPrice != null) ?
                 (
                 historyROBalanceShipPrice.TotalCashAmt
                 + historyROBalanceShipPrice.TotalGiftCardAmt
                 + (historyROBalanceShipPrice.TotalShipPriceAmt
                 * 10
                 )
                 ) : 0M
                 );

            historyRefund = totalShipPriceAmt;
            totalAmt -= historyRefund;
            if (totalAmt < 0)
            {
                totalAmt = 0;
            }
        }

        #endregion 查询

        #region Protected Methods

        /// <summary>
        /// 根据单件列表获取退款单Items信息
        /// </summary>
        /// <param name="registers">单件列表</param>
        /// <returns>退款单Items信息</returns>
        protected virtual List<RefundItemInfo> GetRefundItems(List<RMARegisterInfo> registers)
        {
            List<RefundItemInfo> list = new List<RefundItemInfo>();
            registers.ForEach(item =>
            {
                if (item.SysNo != null && item.BasicInfo.ProductSysNo != null)
                {
                    SOItemInfo soItem = null;

                    if (item.BasicInfo.SOItemType != SOProductType.ExtendWarranty)
                    {
                        soItem = soInfo.Items.Find(entityItem =>
                        {
                            return (entityItem.ProductType != SOProductType.ExtendWarranty) && (entityItem.ProductSysNo == item.BasicInfo.ProductSysNo.Value);
                        });
                    }
                    else
                    {
                        soItem = soInfo.Items.Find(entityItem =>
                        {
                            return (entityItem.ProductType == SOProductType.ExtendWarranty) && (entityItem.MasterProductSysNo == item.BasicInfo.ProductSysNo.ToString());
                        });
                    }

                    if (soItem != null)
                    {
                        RefundItemInfo refundItem = new RefundItemInfo();

                        refundItem.RegisterSysNo = item.SysNo;
                        refundItem.OrgPrice = soItem.Price;
                        decimal? discountAmt = soItem.PromotionAmount;
                        refundItem.UnitDiscount = Decimal.Round(discountAmt.Value / soItem.Quantity.Value, 2);
                        refundItem.ProductValue = refundItem.OrgPrice + refundItem.UnitDiscount;
                        refundItem.OrgPoint = soItem.GainAveragePoint ?? 0;
                        refundItem.RefundPrice = refundItem.ProductValue;   //缺省设置为全额
                        refundItem.PointType = soItem.PayType;
                        refundItem.CompanyCode = item.CompanyCode;
                        refundItem.RefundPriceType = ReturnPriceType.OriginPrice;    //缺省设置原价退款

                        list.Add(refundItem);
                    }
                }
            });

            return list;
        }

        /// <summary>
        /// 设置退款单支付方式
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="registers"></param>
        protected virtual void SetRefundPayType(RefundInfo entity, List<RMARegisterInfo> registers)
        {

            CustomerContactInfo customer = ObjectFactory<ICustomerContactDA>.Instance.LoadByRegisterSysNo(registers[0].SysNo.Value);
            if (customer != null)
            {

                if (customer.RefundPayType == 1) //银行转帐，改为对应的类型
                {
                    entity.RefundPayType = RefundPayType.BankRefund;
                }
                else if (customer.RefundPayType == 2)  //余额账户，改为对应的类型
                {
                    entity.RefundPayType = RefundPayType.PrepayRefund;
                }
                else
                {
                    entity.RefundPayType = RefundPayType.NetWorkRefund;
                }
            }
            else
            {
                entity.RefundPayType = RefundPayType.NetWorkRefund;
            }
        }

        /// <summary>
        /// 修改退款单Item的成本价格以及单件信息
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void UpdateRefundItemCostAndRegister(RefundInfo refundInfo)
        {
            List<RegisterForRefund> registers = this.refundDA.GetRegistersForRefund(refundInfo.SysNo.Value);

            if (registers != null && registers.Count > 0)
            {
                //获取申请单业务模式信息
                RMARequestInfo requestInfo = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(registers[0].RegisterSysNo.Value);

                //更新单件的相关信息
                foreach (RegisterForRefund item in registers)
                {
                    if (item.RevertStatus == RMARevertStatus.Reverted)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Register_Reverted");
                        msg = string.Format(msg, item.RegisterSysNo);
                        throw new BizException(msg);
                    }

                    RefundItemInfo costItem = this.refundDA.GetRefundItemCost(item.ItemSysNo.Value);

                    if (costItem != null)
                    {
                        UpdateRefundItemCost(item, costItem);
                    }
                    decimal? refundCost = costItem != null ? costItem.RefundCost : default(decimal?);

                    UpdateRegister(requestInfo, item, refundCost);

                    UpdateRMAInventory(item);
                }
            }
        }

        /// <summary>
        /// 获取价保积分
        /// </summary>
        /// <param name="refundSysNo">退款单编号</param>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>价保分数</returns>
        protected virtual int GetPriceProtectPoint(int refundSysNo, int soSysNo, string companyCode)
        {
            List<int> registers = refundDA.GetItemsByRefundSysNo(refundSysNo, companyCode).Select(p => p.RegisterSysNo.Value).ToList();
            List<int> productSysNoList = ObjectFactory<IRegisterDA>.Instance.GetRegistersBySysNoList(registers).Select(p => p.BasicInfo.ProductSysNo.Value).ToList();

            return ExternalDomainBroker.GetPriceprotectPoint(soSysNo, productSysNoList);
        }

        /// <summary>
        /// 退款前的业务检查
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void PreCheckRefund(RefundInfo refundInfo)
        {
            if (refundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            if (refundInfo.Status != RMARefundStatus.WaitingAudit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_InvalidStatus");
                throw new BizException(msg);
            }

            SOIncomeRefundInfo soIncomeRefundInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(refundInfo.SysNo.Value, RefundOrderType.RO);
            if (soIncomeRefundInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_Empty");
                throw new BizException(msg);
            }
            if (soIncomeRefundInfo.Status != RefundStatus.Audit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_IncomeBankInfo_NoVerifyPass");
                throw new BizException(msg);
            }

            this.soInfo = ExternalDomainBroker.GetSOInfo(refundInfo.SOSysNo.Value);

            if (this.soInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_SO_Empty");
                throw new BizException(msg);
            }

            //提交审核人与退款确认人不能相同
            //if (refundInfo.AuditUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_AuditRefuntUser_SameCode");
            //    throw new BizException(msg);
            //}

            if (refundInfo.DeductPointFromCurrentCash != 0M)
            {
                PayType payType = ExternalDomainBroker.GetPayType(soInfo.BaseInfo.PayTypeSysNo.Value);

                if (payType == null)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_InvalidPayType");
                    throw new BizException(msg);
                }

                if (!payType.IsPayWhenRecv.Value)//款到发货
                {
                    SOIncomeInfo soIncomeInfo = ExternalDomainBroker.GetValidSOIncomeInfo(refundInfo.SOSysNo.Value, SOIncomeOrderType.SO);
                    if (soIncomeInfo == null)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_SOIncomeInfoNotExists");
                        throw new BizException(msg);
                    }
                    if (soIncomeInfo.Status != SOIncomeStatus.Confirmed)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_SOIncomeInfoNotConfirmed");
                        throw new BizException(msg);
                    }
                }
            }
        }

        /// <summary>
        /// 是否有冲突退款
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void CheckConflictRefund(RefundInfo refundInfo)
        {
            RefundInfo newRefundInfo = new RefundInfo()
            {
                AuditTime = refundInfo.AuditTime,
                AuditUserSysNo = refundInfo.AuditUserSysNo,
                CashAmt = refundInfo.CashAmt,
                SOSysNo = refundInfo.SOSysNo,
                SOCashPointRate = refundInfo.SOCashPointRate,
                SOInvoiceNo = refundInfo.SOInvoiceNo,
                Status = refundInfo.Status,
                SysNo = refundInfo.SysNo,
                CashFlag = refundInfo.CashFlag,
                CheckIncomeStatus = refundInfo.CheckIncomeStatus,
                CompensateShipPrice = refundInfo.CompensateShipPrice,
                CreateTime = refundInfo.CreateTime,
                DeductPointFromAccount = refundInfo.DeductPointFromAccount,
                DeductPointFromCurrentCash = refundInfo.DeductPointFromCurrentCash,
                CreateUserSysNo = refundInfo.CreateUserSysNo,
                CustomerSysNo = refundInfo.CustomerSysNo,
                FinanceNote = refundInfo.FinanceNote,
                GiftCardAmt = refundInfo.GiftCardAmt,
                IncomeBankInfo = refundInfo.IncomeBankInfo,
                InvoiceLocation = refundInfo.InvoiceLocation,
                Note = refundInfo.Note,
                OrgGiftCardAmt = refundInfo.OrgGiftCardAmt,
                OrgCashAmt = refundInfo.OrgCashAmt,
                OrgPointAmt = refundInfo.OrgPointAmt,
                PointAmt = refundInfo.PointAmt,
                RefundID = refundInfo.RefundID,
                RefundItems = refundInfo.RefundItems,
                RefundPayType = refundInfo.RefundPayType,
                RefundReason = refundInfo.RefundReason,
                RefundTime = refundInfo.RefundTime,
                RefundUserSysNo = refundInfo.RefundUserSysNo,
                PriceprotectPoint = refundInfo.PriceprotectPoint,
                HasPriceprotectPoint = refundInfo.PriceprotectPoint.HasValue && refundInfo.PriceprotectPoint > 0,
                CompanyCode = refundInfo.CompanyCode
            };

            newRefundInfo.HasPriceprotectPoint = refundInfo.PriceprotectPoint.HasValue && refundInfo.PriceprotectPoint > 0;
            newRefundInfo.RefundItems = this.refundDA.GetItemsByRefundSysNo(newRefundInfo.SysNo.Value, newRefundInfo.CompanyCode);

            this.Calc(newRefundInfo, this.soInfo);

            if (!newRefundInfo.GiftCardAmt.HasValue)
            {
                newRefundInfo.GiftCardAmt = 0;
            }

            if (!refundInfo.GiftCardAmt.HasValue)
            {
                refundInfo.GiftCardAmt = 0;
            }

            if (!newRefundInfo.CashAmt.HasValue)
            {
                newRefundInfo.CashAmt = 0;
            }

            if (!refundInfo.CashAmt.HasValue)
            {
                refundInfo.CashAmt = 0;
            }

            if (!newRefundInfo.PointAmt.HasValue)
            {
                newRefundInfo.PointAmt = 0;
            }

            if (!refundInfo.PointAmt.HasValue)
            {
                refundInfo.PointAmt = 0;
            }

            if (Math.Abs(newRefundInfo.CashAmt.Value - refundInfo.CashAmt.Value) > 0.01M
                || Math.Abs(newRefundInfo.GiftCardAmt.Value - refundInfo.GiftCardAmt.Value) > 0.01M
                || newRefundInfo.PointAmt != refundInfo.PointAmt)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_RefundAmtChanged");
                throw new BizException(msg);
            }
        }

        /// <summary>
        /// 检查本次退款后，每类商品的退款总额是否大于订单中每类商品总额
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CheckSOItemRefundAmt(RefundInfo entity)
        {
            List<RefundAmountForCheck> soAmountList = this.refundDA.GetSOAmountForRefund(entity.SysNo.Value, entity.CompanyCode);
            List<RefundAmountForCheck> roAmountList = this.refundDA.GetROAmountForRefund(entity.SysNo.Value, entity.CompanyCode);


            foreach (RefundAmountForCheck roItem in roAmountList)
            {
                roItem.ProductType = roItem.SOItemType;
                roItem.RefundAmount = roItem.RO;
                var soItem = soAmountList.Find(findItem =>
                {
                    return (roItem.ProductSysNo == findItem.ProductSysNo)
                        && (roItem.MasterProductSysNo == findItem.MasterProductSysNo
                        && roItem.ProductType == findItem.SOItemType);
                });
                if (soItem != null)
                {
                    soItem.SaleAmount = soItem.SO;
                    if (roItem.RefundAmount > soItem.SaleAmount)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_SalesOrder_ItemInvalidTotalPrice");
                        msg = string.Format(msg, roItem.ProductSysNo);
                        throw new BizException(msg);
                    }
                }
            }
        }

        /// <summary>
        /// 修改单件的退款状态等相关信息，并关闭单件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="item"></param>
        /// <param name="costItem"></param>
        protected virtual void UpdateRegister(RMARequestInfo request, RegisterForRefund item, decimal? cost)
        {
            RMARegisterInfo rrItem = new RMARegisterInfo();
            rrItem.SysNo = item.RegisterSysNo;
            rrItem.BasicInfo.RefundStatus = RMARefundStatus.Refunded;
            rrItem.BasicInfo.OwnBy = RMAOwnBy.Self;
            rrItem.BasicInfo.Cost = cost;
            rrItem.BasicInfo.OwnByWarehouse = item.SalesWarehouse;

            //2,3,5,6 模式，退款后关闭单件
            if (request != null &&
                 (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.SELF)
                     ||
                     (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.MET)
                    ||
                      (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.SELF)
                    ||
                     (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.MET)
                )
            {
                ObjectFactory<RegisterProcessor>.Instance.Close(rrItem.SysNo.Value);
            }
            else
            {
                if (item.ProductType == SOProductType.ExtendWarranty)
                {
                    ObjectFactory<RegisterProcessor>.Instance.Close(rrItem.SysNo.Value);
                }
            }

            ObjectFactory<IRegisterDA>.Instance.UpdateRegisterAfterRefund(rrItem);
        }

        /// <summary>
        /// 更新RefundItem的成本
        /// </summary>
        /// <param name="item"></param>
        /// <param name="costItem"></param>
        protected virtual void UpdateRefundItemCost(RegisterForRefund item, RefundItemInfo costItem)
        {
            RefundItemInfo roItem = new RefundItemInfo();
            roItem.SysNo = item.ItemSysNo;
            roItem.RefundCost = costItem.RefundCost;
            roItem.RefundCostWithoutTax = costItem.RefundCostWithoutTax;
            roItem.RefundCostPoint = (int)((costItem.RefundCostPoint ?? 0) / pointExchangeRate);

            this.refundDA.UpdateItemForRefund(roItem);
        }

        /// <summary>
        /// 修改RMA退款单状态
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void UpdateRefundStatus(RefundInfo entity)
        {
            RefundInfo newEntity = new RefundInfo()
            {
                SysNo = entity.SysNo,
                Status = entity.Status,
                RefundTime = DateTime.Now,
                RefundUserSysNo = ServiceContext.Current.UserSysNo
            };
            this.refundDA.UpdateMaster(newEntity);
        }

        /// <summary>
        /// 修改RMA库存
        /// </summary>
        /// <param name="item"></param>
        protected virtual void UpdateRMAInventory(RegisterForRefund item)
        {
            if (item.ProductType != SOProductType.ExtendWarranty)
            {
                RMAInventory inventory = null;

                //Bob.H.Li item.ReceiveWarehouse 可能为空  2011.06.11
                if (!string.IsNullOrEmpty(item.ReceiveWarehouse))
                {
                    inventory = ObjectFactory<IRegisterDA>.Instance.GetRMAInventoryBy(int.Parse(item.ReceiveWarehouse), item.ProductSysNo.Value);
                }
                if (inventory != null)
                {
                    if (!inventory.AverageCost.HasValue)
                    {
                        inventory.AverageCost = 0;
                    }
                    if (!inventory.OwnbyNeweggQty.HasValue)
                    {
                        inventory.OwnbyNeweggQty = 0;
                    }

                    decimal? averageCost;
                    averageCost = (inventory.AverageCost.Value * inventory.OwnbyNeweggQty.Value + item.Cost) / (inventory.OwnbyNeweggQty.Value + 1);
                    inventory.AverageCost = averageCost;
                    inventory.AverageCostWithoutTax = inventory.AverageCost.Value / (decimal)1.7;
                    inventory.OwnbyNeweggQty = 1;
                    inventory.OwnbyCustomerQty = -1;
                    ObjectFactory<IRegisterDA>.Instance.UpdateInventory(inventory);

                    //[Jay]:Refund成功以后添加日志
                    var rmaInventoryLog = new RMAInventoryLog
                    {
                        WarehouseSysNo = Convert.ToInt32(item.ReceiveWarehouse),
                        ProductSysNo = item.ProductSysNo.Value,
                        RegisterSysNo = item.RegisterSysNo,
                        OperationType = "退款单退款",
                        RMAStockQty = 0,
                        RMAOnVendorQty = 0,
                        ShiftQty = 0,
                        OwnbyNeweggQty = 1,
                        OwnbyCustomerQty = -1,
                        OperationTime = DateTime.Now
                    };
                    if (item.ProductSysNo.HasValue)
                    {
                        rmaInventoryLog.ProductSysNo = item.ProductSysNo.Value;
                        rmaInventoryLog.Memo = ObjectFactory<IRequestDA>.Instance.GetInventoryMemo(
                            Convert.ToInt32(item.ReceiveWarehouse),
                            item.ProductSysNo.Value, inventory.CompanyCode);
                    }
                    ObjectFactory<IRequestDA>.Instance.InsertRMAInventoryLog(rmaInventoryLog);
                }
            }
        }

        /// <summary>
        /// 把退还的现金通过转积分的形式退给顾客
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void RefundCashToPoints(RefundInfo refundInfo)
        {
            int affectedPoint2 = 0;
            if (refundInfo.RefundPayType == RefundPayType.TransferPointRefund)
            {
                affectedPoint2 = Convert.ToInt32(Decimal.Round((refundInfo.CashAmt ?? 0) * pointExchangeRate, 0));
            }
            if (affectedPoint2 != 0)
            {
                AdjustPointRequest transferPointInfo = new AdjustPointRequest()
                {
                    Source = "RMA",
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    Point = affectedPoint2,
                    PointType = (int)AdjustPointType.RefundCashToPoints,
                    Memo = refundInfo.SysNo.ToString(),
                    SOSysNo = refundInfo.SOSysNo
                };
                ExternalDomainBroker.AdjustPoint(transferPointInfo);//转积分调整
            }
        }

        /// <summary>
        /// 积分撤销，撤销顾客获得的积分
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void ReturnProductPoint(RefundInfo refundInfo)
        {
            int affectedPoint1 = -1 * (refundInfo.DeductPointFromAccount ?? 0) + (refundInfo.PointAmt ?? 0);

            if (affectedPoint1 != 0)
            {
                AdjustPointRequest itemPointInfo = new AdjustPointRequest()
                {
                    Source = "RMA",
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    Point = affectedPoint1,
                    PointType = (int)AdjustPointType.ReturnProductPoint,
                    Memo = refundInfo.SysNo.ToString(),
                    OperationType = AdjustPointOperationType.Abandon,
                    SOSysNo = refundInfo.SOSysNo
                };

                ExternalDomainBroker.AdjustPoint(itemPointInfo);//item积分撤消
            }
        }

        /// <summary>
        /// 退款到余额账户
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void RefundPrepay(RefundInfo refundInfo)
        {
            if (refundInfo.RefundPayType == RefundPayType.PrepayRefund)
            {
                CustomerPrepayLog prepayLogInfo = new CustomerPrepayLog()
                {
                    SOSysNo = refundInfo.SOSysNo.Value,
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    AdjustAmount = refundInfo.CashAmt ?? 0M,
                    PrepayType = PrepayType.ROReturn,
                    Note = "RMA退款单退入余额账户"
                };
                ExternalDomainBroker.AdjustPrePay(prepayLogInfo);
            }
        }

        /// <summary>
        /// 创建负收款单信息,如果不涉及现金则自动Confirm
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void CreateSOIncome(RefundInfo refundInfo)
        {
            SOIncomeInfo incomeEntity = ExternalDomainBroker.GetValidSOIncomeInfo(refundInfo.SysNo.Value, SOIncomeOrderType.RO);
            if (incomeEntity == null)
            {
                decimal affectedCash = 0M;
                if (refundInfo.RefundPayType != RefundPayType.TransferPointRefund)
                {
                    affectedCash = -1 * (refundInfo.CashAmt ?? 0M);
                }

                var incomeInfo = new SOIncomeInfo()
                {
                    OrderType = SOIncomeOrderType.RO,
                    OrderSysNo = refundInfo.SysNo.Value,
                    OrderAmt = affectedCash,
                    IncomeAmt = affectedCash,
                    IncomeStyle = SOIncomeOrderStyle.RO,
                    Status = SOIncomeStatus.Origin,
                    GiftCardPayAmt = -1 * (refundInfo.GiftCardAmt ?? 0M),//GiftCard
                    CompanyCode = refundInfo.CompanyCode
                };

                ExternalDomainBroker.CreateSOIncome(incomeInfo);

                //泰隆优选不自动确认收款单
                //if (refundInfo.CashFlag == CashFlagStatus.No)  // 如果不涉及现金则自动 Confirm
                //{
                //    SOIncomeInfo incomeRO = ExternalDomainBroker.GetValidSOIncomeInfo(refundInfo.SysNo.Value, SOIncomeOrderType.RO);
                //    SOIncomeInfo incomeSO = ExternalDomainBroker.GetValidSOIncomeInfo(refundInfo.SOSysNo.Value, SOIncomeOrderType.SO);

                //    if (incomeRO != null && incomeSO != null && incomeSO.Status == SOIncomeStatus.Origin)
                //    {
                //        int userSysNo = ExternalDomainBroker.GetUserSysNo(AppSettingManager.GetSetting("RMA", RMAConst.AutoRMAPhysicalUserName),
                //            AppSettingManager.GetSetting("RMA", RMAConst.AutoRMALoginUserName), AppSettingManager.GetSetting("RMA", RMAConst.AutoRMASourceDirectoryKey));
                //        if ((incomeRO.IncomeAmt + incomeSO.IncomeAmt) == 0)
                //        {
                //            ExternalDomainBroker.AutoConfirmIncomeInfo(incomeRO.SysNo.Value, incomeRO.OrderSysNo.Value, userSysNo);
                //            ExternalDomainBroker.AutoConfirmIncomeInfo(incomeSO.SysNo.Value, incomeSO.OrderSysNo.Value, userSysNo);
                //        }
                //        else if (refundInfo.RefundPayType == RefundPayType.TransferPointRefund)
                //        {
                //            ExternalDomainBroker.AutoConfirmIncomeInfo(incomeRO.SysNo.Value, incomeRO.OrderSysNo.Value, userSysNo);
                //        }
                //    }
                //}
            }
        }



        protected virtual void SendEmail(decimal cashAmount, GiftCardInfo giftCardInfo, int customerSysNo)
        {
            CustomerInfo customerInfo = ExternalDomainBroker.GetCustomerInfo(customerSysNo);
            if (customerInfo != null && !string.IsNullOrEmpty(customerInfo.BasicInfo.Email))
            {
                KeyValueVariables vars = new KeyValueVariables();
                vars.Add("CustomerName", customerInfo.BasicInfo.CustomerName);
                vars.Add("CustomerID", customerInfo.BasicInfo.CustomerID);
                vars.Add("Quantity", "1");
                vars.Add("TotalValue", decimal.Round(cashAmount, 2).ToString());
                vars.Add("ExpireYear", giftCardInfo.EndDate.Value.Year.ToString());
                vars.Add("ExpireMonth", giftCardInfo.EndDate.Value.Month.ToString());
                vars.Add("ExpireDay", giftCardInfo.EndDate.Value.Day.ToString());
                vars.Add("Year", DateTime.Today.Year.ToString());

                EmailHelper.SendEmailByTemplate(customerInfo.BasicInfo.Email, "RMARefund_GiftCardCreate", vars);
            }
        }

        protected virtual void PreCheckCalculate(RefundInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (entity.CompensateShipPrice.HasValue && entity.CompensateShipPrice.Value < 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_ShipPriceLessThanZero");
                throw new BizException(msg);
            }
            RefundInfo orgRefund = LoadBySysNo(entity.SysNo.Value);
            if (orgRefund == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Empty");
                throw new BizException(msg);
            }
            if (orgRefund.Status != RMARefundStatus.WaitingRefund)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Calculate_InvalidStatus");
                throw new BizException(msg);
            }

            this.soInfo = ExternalDomainBroker.GetSOInfo(orgRefund.SOSysNo.Value);
            if (soInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_SO_Empty");
                throw new BizException(msg);
            }
            if (soInfo.Items == null || soInfo.Items.Count == 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_SO_ItemsEmpty");
                throw new BizException(msg);
            }
        }

        protected virtual void UpdateSOIncomeRefundInfo(RefundInfo refundInfo, SOIncomeRefundInfo incomeBankInfo)
        {
            incomeBankInfo.OrderType = RefundOrderType.RO;
            incomeBankInfo.OrderSysNo = refundInfo.SysNo.Value;
            incomeBankInfo.SOSysNo = refundInfo.SOSysNo.Value;
            incomeBankInfo.RefundPayType = refundInfo.RefundPayType;
            incomeBankInfo.RefundReason = refundInfo.RefundReason;
            incomeBankInfo.HaveAutoRMA = false;
            incomeBankInfo.Status = RefundStatus.WaitingRefund;

            int affectedPoint = 0;
            affectedPoint = -1 * (refundInfo.DeductPointFromAccount ?? 0) + (refundInfo.PointAmt ?? 0);
            if (refundInfo.RefundPayType == RefundPayType.TransferPointRefund)
            {
                affectedPoint += Convert.ToInt32(Decimal.Round((refundInfo.CashAmt ?? 0M) * pointExchangeRate, 0));

                incomeBankInfo.RefundPoint = affectedPoint;
                incomeBankInfo.RefundCashAmt = 0;
            }
            else
            {
                incomeBankInfo.RefundPoint = affectedPoint;
                incomeBankInfo.RefundCashAmt = refundInfo.CashAmt;
            }
            incomeBankInfo.RefundGiftCard = refundInfo.GiftCardAmt;

            ExternalDomainBroker.UpdateSOIncomeRefundInfo(incomeBankInfo);
        }

        protected virtual void PreCheckCreate(RefundInfo entity, out List<RMARegisterInfo> registers)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("refundInfo");
            }
            if (entity.SOSysNo == null)
            {
                throw new ArgumentNullException("refundInfo.SOSysNo");
            }
            if (entity.RefundItems == null || entity.RefundItems.Count < 1)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_ItemsInvalid");
                throw new BizException(msg);
            }

            List<int> registerSysNoList = new List<int>();

            entity.RefundItems.ForEach(item =>
            {
                if (item.RegisterSysNo != null)
                {
                    //只有处于处理中的单件才可以创建退款条目
                    RMARegisterInfo register = ObjectFactory<RegisterProcessor>.Instance.LoadBySysNo(item.RegisterSysNo.Value);
                    if (register != null && register.BasicInfo.Status == RMARequestStatus.Handling
                        && register.BasicInfo.RefundStatus == RMARefundStatus.WaitingRefund)
                    {
                        registerSysNoList.Add(item.RegisterSysNo.Value);
                    }
                }
            });
            // 0、检查是否包含有单件数据
            if (registerSysNoList.Count < 1)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_ItemsInvalid");
                throw new BizException(msg);
            }

            // 1、检查对应的 SO单是否存在有效的退款单状态
            //SOIncomeInfo incomeInfo = ExternalDomainBroker.GetValidSOIncomeInfo(entity.SOSysNo.Value, SOIncomeOrderType.SO);
            //if (incomeInfo != null && incomeInfo.Status != SOIncomeStatus.Confirmed
            //    && entity.CheckIncomeStatus == true)
            //{
            //    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeInfo_StatusNoConfirmed");
            //    throw new BizException(msg);
            //}

            // 2、判断是否存在已生成有效 RO 的 Register
            List<RefundItemInfo> items = refundDA.GetItemsByRegisterSysNoList(registerSysNoList);
            if (items != null && items.Count > 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Register_ItemsUsed");
                throw new BizException(msg);
            }

            // 3、检查是否有效的 Register
            registers = this.refundDA.GetRegistersForCreate(registerSysNoList);
            if (registers == null || registers.Count == 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Register_ItemsInvalid");
                throw new BizException(msg);
            }

            // 4、加载 SO 单数据
            this.soInfo = ExternalDomainBroker.GetSOInfo(entity.SOSysNo.Value);

            if (soInfo == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_SO_Empty");
                throw new BizException(msg);
            }
            if (soInfo.Items == null || soInfo.Items.Count == 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_SO_ItemsEmpty");
                throw new BizException(msg);
            }
            if (entity.CheckIncomeStatus.Value)//非强制
            {
                foreach (var register in registers)
                {
                    List<SOItemInfo> giftInfoList = ExternalDomainBroker.GetGiftBySOProductSysNo(entity.SOSysNo.Value, register.BasicInfo.ProductSysNo.Value);

                    if (giftInfoList != null && giftInfoList.Count > 0)
                    {
                        foreach (var item in giftInfoList)
                        {
                            if (this.refundDA.GetRefundCountBySOSysNoAndProductSysNo
                                (item.SOSysNo ?? 0
                                , item.ProductSysNo ?? 0
                                ) == 0)
                            {
                                //所选单件中有附属赠品,必须通过强制创建生成
                                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_RegisterHasGifts");
                                throw new BizException(msg);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void PreCheckUpdate(RefundInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (entity.SOSysNo == null)
            {
                throw new ArgumentNullException("entity.SOSysNo");
            }
            if (entity.IncomeBankInfo == null)
            {
                throw new ArgumentNullException("entity.IncomeBankInfo");
            }
            if (string.IsNullOrEmpty(entity.IncomeBankInfo.Note))
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_NoteRequired");
                throw new BizException(msg);
            }
            if (entity.CashFlag != CashFlagStatus.Yes && entity.RefundPayType != RefundPayType.NetWorkRefund)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_NewWorkRefundNoCash");
                throw new BizException(msg);
            }

            if (entity.RefundPayType == RefundPayType.BankRefund)
            {
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.BankName))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_BankNameRequired");
                    throw new BizException(msg);
                }
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.BranchBankName))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_BranchBankNameRequired");
                    throw new BizException(msg);
                }
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.CardNumber))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_CardNumberRequired");
                    throw new BizException(msg);
                }
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.CardOwnerName))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_CardOwnerNameRequired");
                    throw new BizException(msg);
                }
            }
            else if (entity.RefundPayType == RefundPayType.PostRefund)
            {
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.PostAddress))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_PostAddressRequired");
                    throw new BizException(msg);
                }
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.PostCode))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_PostCodeInvalid");
                    throw new BizException(msg);
                }
                if (!(!string.IsNullOrEmpty(entity.IncomeBankInfo.PostCode) && Regex.IsMatch(entity.IncomeBankInfo.PostCode, @"^\d{6}$")))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_PostCodeInvalid");
                    throw new BizException(msg);
                }
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.ReceiverName))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_ReceiveNameRequired");
                    throw new BizException(msg);
                }
            }
            else if (entity.RefundPayType == RefundPayType.NetWorkRefund)
            {
                if (string.IsNullOrEmpty(entity.IncomeBankInfo.BankName))
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_BankNameRequired");
                    throw new BizException(msg);
                }
            }
            else if (entity.RefundPayType == RefundPayType.CashRefund)
            {
                SOBaseInfo so = ExternalDomainBroker.GetSOBaseInfo(entity.SOSysNo.Value);
                if (so != null)
                {
                    if (so.PayTypeSysNo.HasValue && !ExternalDomainBroker.CheckPayTypeCanCashRefund(so.PayTypeSysNo.Value))
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeBankInfo_PayType"));
                    }
                }
            }
        }

        protected virtual string GenerateRefundID(int sysno)
        {
            return String.Format("R3{0:00000000}", sysno);  //return "R3" + sysno.ToString().PadLeft(8, '0');
        }

        /// <summary>
        /// 计算退现金、初算退礼品卡、退现金、退礼品卡
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soInfo"></param>
        protected virtual void Calc(RefundInfo entity, SOInfo soInfo)
        {
            entity.OrgPointAmt = 0;     // 清空
            entity.OrgCashAmt = 0M;     // 清空
            entity.OrgGiftCardAmt = 0m;
            soInfo.BaseInfo.GiftCardPay = soInfo.BaseInfo.GiftCardPay ?? 0m;

            #region 计算 现金与积分比率

            decimal? pointOnly = 0;  // 仅积分支付部分对应的价值
            decimal? cashOnly = 0;   // 仅现金支付部分对应的价值
            decimal? totalCouponDiscount = 0;//蛋劵折扣金额

            soInfo.Items.ForEach(item =>
            {
                decimal? discountAmt = item.PromotionAmount;
                if (item.PayType == ProductPayType.PointOnly)
                {
                    pointOnly += item.Price * item.Quantity + discountAmt;
                }
                else if (item.PayType == ProductPayType.MoneyOnly)
                {
                    cashOnly += item.Price * item.Quantity + discountAmt;
                }

                totalCouponDiscount += item.CouponAmount;
            });

            decimal originalSOCashPointRate = 0;//不四舍五入的比率
            if ((soInfo.BaseInfo.SOAmount + soInfo.BaseInfo.PromotionAmount) - (pointOnly + cashOnly) == 0)
            {
                entity.SOCashPointRate = 0;
            }
            else if ((soInfo.BaseInfo.SOAmount + soInfo.BaseInfo.PromotionAmount) - (totalCouponDiscount + pointOnly + cashOnly) == 0M)
            {
                entity.SOCashPointRate = 0;
            }
            else if (soInfo.BaseInfo.CashPay - cashOnly == 0)
            {
                entity.SOCashPointRate = 0;
            }
            else
            {
                //订单现金支付金额 + 促销优惠 - 仅现金支付金额 = 订单需要积分支付的金额
                //订单金额-促销折扣 - (优惠卷金额 + 仅积分支付金额 + 仅现金支付金额) = 既支持积分也支持现金支付的金额
                decimal? soCachPointRate = (soInfo.BaseInfo.CashPay + soInfo.BaseInfo.PromotionAmount - cashOnly)
                    / (soInfo.BaseInfo.SOAmount + soInfo.BaseInfo.PromotionAmount - (totalCouponDiscount + pointOnly + cashOnly));


                entity.SOCashPointRate = Decimal.Round(soCachPointRate ?? 0, 4);

                originalSOCashPointRate = (soInfo.BaseInfo.CashPay + soInfo.BaseInfo.PromotionAmount - cashOnly).Value
                    / (soInfo.BaseInfo.SOAmount + soInfo.BaseInfo.PromotionAmount - (totalCouponDiscount + pointOnly + cashOnly)).Value;
            }

            #endregion 计算 现金与积分比率

            string warehouseNumber = (entity.SysNo.HasValue) ? refundDA.GetWarehouseNo(entity.SysNo.Value) : "0";

            #region 获取历史退现金、礼品卡、运费

            HistoryRefundAmount historyAmt = GetHistoryRefundAmt(entity.SOSysNo.Value, 0, warehouseNumber);
            decimal totalCashAmtHistory = historyAmt.TotalCashAmt;
            decimal totalGiftAmtHistory = historyAmt.TotalGiftCardAmt;
            decimal totalShipPriceAmtHitory = historyAmt.TotalShipPriceAmt;
            int totalPointAmtHistory = historyAmt.TotalPointAmt;

            #endregion 获取历史退现金、礼品卡、运费

            decimal opcShipFee = 0;
            List<InvoiceMasterInfo> invoiceList = ExternalDomainBroker.GetSOInvoiceMaster(soInfo.SysNo.Value);//运费
            invoiceList = invoiceList.Where(p => p.StockSysNo.ToString() == warehouseNumber).ToList();
            if (invoiceList != null)
            {
                invoiceList.ForEach(p =>
                {
                    opcShipFee += (p.PremiumAmt + p.ShippingCharge + p.ExtraAmt).Value;
                });
            }
            decimal availShipPrice = opcShipFee - totalShipPriceAmtHitory;//减去历史退运费

            if (availShipPrice < 0)
            {
                availShipPrice = 0;
            }
            if (entity.CompensateShipPrice > availShipPrice)//补偿运费退得太多
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_CompensateShipPriceTooMuch");
                throw new BizException(msg);
            }

            #region 计算初算退现金、初算退积分

            if (entity.RefundItems != null)
            {
                entity.RefundItems.ForEach(item =>
                {
                    if (item.ProductValue < item.RefundPrice)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_ItemRefundTooMuch");
                        throw new BizException(string.Format(msg, item.SysNo));
                    }
                    if (item.PointType == ProductPayType.All)//只有既支持积分又支持现金的商品才需要计算
                    {
                        //根据单件退款额度，调整比率精度
                        int decimals = Decimal.Round(item.RefundPrice.Value, 0).ToString().Length + 2;
                        //实际计算精度
                        decimal soCashPointRate = Decimal.Round(originalSOCashPointRate, decimals);

                        item.RefundPoint = Convert.ToInt32(Decimal.Round(
                            item.RefundPrice.Value
                            * (1 - soCashPointRate)
                            * pointExchangeRate
                            , 0));

                        #region pointRedeem

                        decimal pointRedeem = -Decimal.Round(
                            (Decimal.Round(
                            item.RefundPrice.Value
                            * (1 - soCashPointRate)
                            * pointExchangeRate
                            , 0)
                            -
                            (item.RefundPrice.Value
                            * (1 - soCashPointRate)
                            * pointExchangeRate)
                            )
                            * 0.1M
                        , 2);

                        #endregion pointRedeem

                        item.RefundCash = item.RefundPrice.Value
                            - (Decimal.Round(item.RefundPrice.Value * (1 - soCashPointRate), 2));
                        item.RefundCash += pointRedeem;
                        item.RefundPoint -= item.OrgPoint;//减去赠送积分
                    }
                    else if (item.PointType == ProductPayType.MoneyOnly)
                    {
                        item.RefundCash = item.RefundPrice;
                        item.RefundPoint = -1 * item.OrgPoint;
                    }
                    else
                    {
                        item.RefundCash = 0M;
                        item.RefundPoint = Convert.ToInt32(Decimal.Round(item.RefundPrice.Value * pointExchangeRate, 0)) - item.OrgPoint;
                    }

                    entity.OrgCashAmt += item.RefundCash;
                    entity.OrgPointAmt += item.RefundPoint;
                });
            }
            entity.OrgCashAmt = Math.Round(entity.OrgCashAmt ?? 0, 2);

            #endregion 计算初算退现金、初算退积分

            if (entity.HasPriceprotectPoint)
            {
                entity.OrgPointAmt -= (entity.PriceprotectPoint ?? 0);
            }

            #region 计算顾客积分归还积分折合现金

            entity.DeductPointFromAccount = 0;
            entity.DeductPointFromCurrentCash = 0;
            if (entity.OrgPointAmt < 0)
            {
                CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(entity.CustomerSysNo.Value);
                if (customer == null)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_CustomerNotExists");
                    throw new BizException(msg);
                }
                if (entity.OrgPointAmt * -1 < customer.ValidScore)
                {
                    entity.DeductPointFromAccount = entity.OrgPointAmt * -1;
                }
                else
                {
                    entity.DeductPointFromAccount = customer.ValidScore;
                    entity.DeductPointFromCurrentCash =
                        Decimal.Round(((entity.OrgPointAmt ?? 0) * -1
                        - (customer.ValidScore ?? 0)) / pointExchangeRate, 2);
                }
            }

            #endregion 计算顾客积分归还积分折合现金

            #region 现金CashPay+PayPrice+ShipPrice+PremiumAmt+DiscountAmt-GiftCardPay

            decimal cashRemoveGiftCard =
                soInfo.BaseInfo.CashPay
                + opcShipFee
                + soInfo.BaseInfo.PromotionAmount.Value
                - soInfo.BaseInfo.GiftCardPay.Value
                - totalCashAmtHistory;

            if (cashRemoveGiftCard < 0)
            {
                cashRemoveGiftCard = 0;
            }

            #endregion 现金CashPay+PayPrice+ShipPrice+PremiumAmt+DiscountAmt-GiftCardPay

            #region 最后修正初算退现金、初算退礼品卡、退现金、退礼品卡

            //初算礼品卡退款金额为，初算退款金额-原支付的现金部分(不包含礼品卡)
            entity.OrgGiftCardAmt = entity.OrgCashAmt.Value - cashRemoveGiftCard;
            if (soInfo.BaseInfo.GiftCardPay.Value == 0)
            {
                entity.OrgGiftCardAmt = 0;
            }
            if (entity.OrgGiftCardAmt < 0)
            {
                entity.OrgGiftCardAmt = 0;
            }

            decimal orgRefundValue = 0;//原始退款金额,退现金+退礼品卡，用于计算现金、礼品卡的比率

            if (cashRemoveGiftCard == 0)//只使用了礼品卡支付或现金已退完
            {
                orgRefundValue = entity.OrgGiftCardAmt.Value;

                entity.GiftCardAmt = entity.OrgGiftCardAmt
                    - entity.DeductPointFromCurrentCash
                + entity.CompensateShipPrice;

                entity.OrgCashAmt = 0;
                entity.CashAmt = 0;
            }
            else
            {
                if (entity.OrgGiftCardAmt > 0)
                {
                    orgRefundValue = entity.OrgCashAmt.Value;

                    entity.GiftCardAmt = entity.OrgGiftCardAmt + entity.CompensateShipPrice;

                    entity.OrgCashAmt = entity.OrgCashAmt - entity.OrgGiftCardAmt;//减去礼品卡部分

                    entity.CashAmt = cashRemoveGiftCard - entity.DeductPointFromCurrentCash;
                }
                else
                {
                    orgRefundValue = entity.OrgCashAmt.Value;

                    entity.GiftCardAmt = entity.OrgGiftCardAmt;

                    entity.CashAmt = entity.OrgCashAmt - entity.DeductPointFromCurrentCash + entity.CompensateShipPrice;
                }
            }
            if (entity.GiftCardAmt + totalGiftAmtHistory > soInfo.BaseInfo.GiftCardPay)
            {
                string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRefundTooMuch");
                new BizException(string.Format(msg, totalGiftAmtHistory, entity.GiftCardAmt, soInfo.BaseInfo.GiftCardPay));
            }

            #endregion 最后修正初算退现金、初算退礼品卡、退现金、退礼品卡

            #region 重新计算item的cash，giftcard

            if (entity.RefundItems != null)
            {
                entity.RefundItems.ForEach(item =>
                {
                    if (orgRefundValue != 0)
                    {
                        item.OrgGiftCardAmt = Decimal.Round(
                                                entity.OrgGiftCardAmt.Value *
                                                (item.RefundCash.Value /
                                                orgRefundValue)
                                                , 2);
                        if (item.OrgGiftCardAmt
                            >
                            entity.OrgGiftCardAmt.Value *
                                                (item.RefundCash.Value /
                                                orgRefundValue)
                            )
                        {
                            item.OrgGiftCardAmt -= 0.01M;
                        }
                    }
                    else
                    {
                        item.OrgGiftCardAmt = 0;
                    }
                    //如只用了礼品卡，或现金已退完，则让item的现金退款为0
                    if (cashRemoveGiftCard == 0)
                    {
                        item.RefundCash = 0;
                    }
                    item.RefundCash -= item.OrgGiftCardAmt;
                    if (item.RefundCash < 0)
                    {
                        item.RefundCash = 0;
                    }
                });
            }

            #endregion 重新计算item的cash，giftcard

            entity.PointAmt = entity.OrgPointAmt + entity.DeductPointFromAccount
                + Convert.ToInt32(entity.DeductPointFromCurrentCash * pointExchangeRate);

            if ((entity.PointAmt + totalPointAmtHistory) > soInfo.BaseInfo.PointPay)
            {
                entity.PointAmt = soInfo.BaseInfo.PointPay - totalPointAmtHistory;
            }
            if (entity.PointAmt < 0)
            {
                entity.PointAmt = 0;
            }
        }

        protected virtual bool CheckSOIncomeRefundInfo(SOIncomeRefundInfo newIncomeBankInfo)
        {
            if ((newIncomeBankInfo.RefundPayType == RefundPayType.BankRefund
                      && !string.IsNullOrEmpty(newIncomeBankInfo.BankName)
                      && !string.IsNullOrEmpty(newIncomeBankInfo.BranchBankName)
                      && !string.IsNullOrEmpty(newIncomeBankInfo.CardNumber)
                      && !string.IsNullOrEmpty(newIncomeBankInfo.CardOwnerName))
                      || (
                      newIncomeBankInfo.RefundPayType == RefundPayType.PostRefund
                      && !string.IsNullOrEmpty(newIncomeBankInfo.PostAddress)
                      && !string.IsNullOrEmpty(newIncomeBankInfo.PostCode)
                      && !string.IsNullOrEmpty(newIncomeBankInfo.ReceiverName))
                      || (
                      newIncomeBankInfo.RefundPayType == RefundPayType.NetWorkRefund
                      && !string.IsNullOrEmpty(newIncomeBankInfo.BankName))
                      )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Private Methods

        #region 发送SSB
        /// <summary>
        /// 发送SSB消息
        /// </summary>
        /// <param name="refundInfo"></param>
        protected virtual void SendInvoiceMessageToSSB(RefundInfo refundInfo)
        {
            string warehouseNo = this.refundDA.GetWarehouseNo(refundInfo.SysNo.Value);

            if (warehouseNo != null && soInfo != null)
            {
                // 如果是北京仓 或 广州仓 或 西安仓 或 订单是增票， 则发送；
                //if (warehouseNo.Trim() == RMAConst.WarehouseNo_GZ
                //    || warehouseNo.Trim() == RMAConst.WarehouseNo_XA
                //    || this.soInfo.InvoiceInfo.IsVAT == true)

                //只要订单是增票就发送
                if (this.soInfo.InvoiceInfo.IsVAT == true)
                {
                    ObjectFactory<SendSSBProcessor>.Instance.SendROMessage(refundInfo.SysNo.Value, warehouseNo, refundInfo.CompanyCode);

                }
            }
        }
        #endregion
    }
}
