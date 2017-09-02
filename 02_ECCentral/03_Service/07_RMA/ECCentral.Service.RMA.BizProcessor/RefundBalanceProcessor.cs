using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.EventMessage.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(RefundBalanceProcessor))]
    public class RefundBalanceProcessor
    {
        private readonly decimal pointExchangeRate = ExternalDomainBroker.GetPointToMoneyRatio();

        #region interface

        private IRefundBalanceDA refundBalanceDA = ObjectFactory<IRefundBalanceDA>.Instance;
        private IRefundDA refundDA = ObjectFactory<IRefundDA>.Instance;
        private RefundProcessor refundProcessor = ObjectFactory<RefundProcessor>.Instance;

        #endregion interface

        #region Load

        /// <summary>
        /// 根据退款调整单系统编号获取退款单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo LoadRefundBalanceBySysNo(int sysNo)
        {
            SOIncomeRefundInfo soIncomRefundInfo = new SOIncomeRefundInfo();

            RefundBalanceInfo refundBalanceInfo = GetRefundBalanceBySysNo(sysNo);
            soIncomRefundInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(sysNo, RefundOrderType.RO_Balance);
            refundBalanceInfo.IncomeBankInfo = soIncomRefundInfo;
            refundBalanceInfo.CustomerID = ExternalDomainBroker.GetCustomerBasicInfo(refundBalanceInfo.CustomerSysNo.Value).CustomerID;
            return refundBalanceInfo;
        }

        /// <summary>
        /// 根据退款单系统编号获取新退款调整单基本信息
        /// </summary>
        /// <param name="OrgRefundSysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo LoadNewRefundBalanceByRefundSysNo(int refundSysNo)
        {
            RefundBalanceInfo refundBalanceInfo = refundBalanceDA.LoadNewRefundBalanceByRefundSysNo(refundSysNo);
            if (refundBalanceInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Void_RefundBalanceInfo"));
            }
            refundBalanceInfo.IncomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(refundBalanceInfo.OriginalRefundSysNo.Value, RefundOrderType.RO);
            refundBalanceInfo.CustomerID = ExternalDomainBroker.GetCustomerBasicInfo(refundBalanceInfo.CustomerSysNo.Value).CustomerID;
            return refundBalanceInfo;

        }

        /// <summary>
        /// 根据退款单号获取已购买的商品列表作为退款类型
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <returns></returns>
        public virtual List<RefundItemInfo> LoadRefundItemListByRefundSysNo(int refundSysNo)
        {
            List<RefundItemInfo> list = refundDA.GetItemsWithProductInfoByRefundSysNo(refundSysNo);
            List<RefundItemInfo> newList = new List<RefundItemInfo>();
            foreach (RefundItemInfo item in list)
            {
                if (newList.Count == 0)
                {
                    newList.Add(item);
                }
                else if (!newList.FirstOrDefault().ProductID.Contains(item.ProductID))
                {
                    newList.Add(item);
                }
            }
            return newList;
        }
        #endregion Load

        #region Create
        /// <summary>
        /// 创建RefundBalance
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo CreateRefundBalance(RefundBalanceInfo entity)
        {
            SOBaseInfo soBaseInfo;

            VerifyForCreate(entity, out soBaseInfo);

            Calc(entity, soBaseInfo);

            entity.Status = RefundBalanceStatus.WaitingRefund;

            RefundBalanceInfo obj = null;

            using (TransactionScope tran = TransactionScopeFactory.CreateTransactionScope())
            {
                obj = ObjectFactory<IRefundBalanceDA>.Instance.CreateRefundBalance(entity);

                //创建RMA退款调整单待审核 - 待办事项:
                EventPublisher.Publish<RMACreateRefundBalanceWaitingForAuditMessage>(new RMACreateRefundBalanceWaitingForAuditMessage()
                {
                    RefundBalanceSysNo = obj.SysNo.Value,
                    RefundSysNo = obj.OriginalRefundSysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                tran.Complete();
            }
            return obj;
        }

        /// <summary>
        /// CreateRefundBalance预检查
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soBaseInfo"></param>
        private void VerifyForCreate(RefundBalanceInfo entity, out SOBaseInfo soBaseInfo)
        {
            if (entity.CashAmt == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CashAmtRequired"));
            }
            if (entity.CashAmt.Value == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CashAmtNotEqualZero"));
            }
            if (entity.RefundPayType != RefundPayType.TransferPointRefund && entity.RefundPayType != RefundPayType.NetWorkRefund
                && entity.RefundPayType != RefundPayType.PrepayRefund && entity.CashAmt < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundPayTypeAndCashAmtValid"));
            }

            soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(entity.OriginalSOSysNo.Value);
            if (soBaseInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "SONotExists"));
            }

            #region 创建时加入金额验证
            decimal availShipPrice, cashRemoveGiftCard, availGiftCard, totalRoBoBalanceAmt;
            int availPointAmt;
            CalculateAvailRefundAmt(entity, soBaseInfo, out availShipPrice, out cashRemoveGiftCard, out availGiftCard, out availPointAmt, out totalRoBoBalanceAmt);

            decimal ROAmt;
            PreCheckForRefund(entity, totalRoBoBalanceAmt, availShipPrice, out ROAmt);
            #endregion
        }

        /// <summary>
        /// 计算退现金、初算退礼品卡、退现金、退礼品卡
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soBaseInfo"></param>
        private void Calc(RefundBalanceInfo entity, SOBaseInfo soBaseInfo)
        {
            decimal availShipPrice, cashRemoveGiftCard, availGiftCard, totalRoBoBalanceAmt;
            int availPointAmt;
            CalculateAvailRefundAmt(entity, soBaseInfo, out availShipPrice, out cashRemoveGiftCard, out availGiftCard, out availPointAmt, out totalRoBoBalanceAmt);
            soBaseInfo.GiftCardPay = soBaseInfo.GiftCardPay ?? 0M;

            #region 根据可用退现金，修正调整单金额的现金、礼品卡、积分部分

            if (entity.CashAmt > 0)
            {
                if (cashRemoveGiftCard >= entity.CashAmt)//有现金可作调整单
                {
                    entity.GiftCardAmt = 0;
                    entity.PointAmt = 0;
                }
                else if (cashRemoveGiftCard == 0)//可退现金为0
                {
                    if (availGiftCard > 0)
                    {
                        if (entity.CashAmt <= availGiftCard)//礼品卡可用
                        {
                            entity.GiftCardAmt = entity.CashAmt;
                            entity.PointAmt = 0;
                        }
                        else//退完礼品卡还有金额转为积分
                        {
                            entity.GiftCardAmt = availGiftCard;

                            entity.PointAmt = Convert.ToInt32(Decimal.Round((entity.CashAmt.Value - availGiftCard) * pointExchangeRate, 0));
                            if (entity.PointAmt > availPointAmt)
                            {
                                entity.PointAmt = availPointAmt;
                            }
                            if (availGiftCard == 0 && availPointAmt == 0)
                            {
                                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundTooMuch"));
                            }
                            if (availGiftCard == 0 && entity.CashAmt.Value < 0.1m)
                            {
                                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "OnlyAdjustPoints"));
                            }
                        }
                    }
                    else
                    {
                        entity.PointAmt = Convert.ToInt32(Decimal.Round(entity.CashAmt.Value * pointExchangeRate, 0));
                    }
                    entity.CashAmt = 0;
                }
                else//有部分现金可退，剩余部分使用礼品卡
                {
                    entity.GiftCardAmt = entity.CashAmt - cashRemoveGiftCard;
                    if (entity.GiftCardAmt > availGiftCard)
                    {
                        entity.PointAmt = Convert.ToInt32(Decimal.Round((entity.GiftCardAmt.Value - availGiftCard) * pointExchangeRate, 0));
                        if (entity.PointAmt > availPointAmt)
                        {
                            entity.PointAmt = availPointAmt;
                        }
                        entity.GiftCardAmt = availGiftCard;
                    }
                    else
                    {
                        entity.PointAmt = 0;
                    }
                    entity.CashAmt = cashRemoveGiftCard;
                }
            }
            else
            {
                entity.GiftCardAmt = 0;
                entity.PointAmt = 0;
            }

            #endregion 根据可用退现金，修正调整单金额的现金、礼品卡、积分部分

            if (entity.ProductSysNo != 0) //商品调整
            {
                #region SOItem

                RefundItemInfo refundItem = refundBalanceDA.GetRefundTotalAmount(entity);

                decimal tempRefunded = (refundItem.RefundPrice ?? 0M) + entity.CashAmt.Value + totalRoBoBalanceAmt;
                decimal productValue = refundItem.ProductValue ?? 0;

                if (productValue < tempRefunded)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundTooMuch"));
                }
                if (tempRefunded < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "NotGreaterThanRefundedAmt"));
                }

                #region 现金、礼品卡、积分金额划分

                decimal refundAmt = entity.CashAmt.Value + entity.GiftCardAmt.Value;
                if (refundAmt > 0 && entity.PointAmt == 0)
                {
                    #region 计算 现金与积分比率

                    decimal originalSOCashPointRate = 0;//不四舍五入的比率:现金部分与总额的比例

                    decimal pointOnly = 0;  // 仅积分支付部分对应的价值
                    decimal cashOnly = 0;   // 仅现金支付部分对应的价值
                    decimal totalPromotionDiscount = 0;//优惠券
                    List<SOItemInfo> soItems = ExternalDomainBroker.GetSOItemList(entity.OriginalSOSysNo.Value);
                    soItems.ForEach(item =>
                    {
                        if (item.PayType == ProductPayType.PointOnly)
                        {
                            pointOnly += item.Price.Value * item.Quantity.Value + item.PromotionAmount.Value;
                        }
                        else if (item.PayType == ProductPayType.MoneyOnly)
                        {
                            cashOnly += item.Price.Value * item.Quantity.Value + item.PromotionAmount.Value;
                        }

                        totalPromotionDiscount += item.PromotionAmount.Value * item.Quantity.Value;
                    });

                    //表示只有仅积分或仅现金部分，没有均支持
                    if ((soBaseInfo.SOAmount + soBaseInfo.PromotionAmount) - (totalPromotionDiscount + pointOnly + cashOnly) == 0M)
                    {   //仅现金
                        if (pointOnly == 0 && cashOnly> 0)
                        {
                            originalSOCashPointRate = 1;    
                        }
                        else //仅积分
                        {
                            originalSOCashPointRate = 0;
                        }                        
                    }
                    else if (soBaseInfo.CashPay - cashOnly == 0)
                    {
                        originalSOCashPointRate = 1;    
                    }
                    else if (soBaseInfo.PointPay - pointOnly == 0)
                    {
                        originalSOCashPointRate = 0;    
                    }
                    else
                    {   //表示均支持的现金部分与均支持的总额的比例
                        originalSOCashPointRate = (soBaseInfo.CashPay + soBaseInfo.PromotionAmount.Value - cashOnly)
                            / (soBaseInfo.SOAmount.Value + soBaseInfo.PromotionAmount.Value - (totalPromotionDiscount + pointOnly + cashOnly));
                    }

                    #endregion 计算 现金与积分比率

                    //根据单件退款额度，调整比率精度
                    int decimals = Decimal.Round(refundAmt, 0).ToString().Length + 2;
                    //实际计算精度
                    decimal SOCashPointRate = Decimal.Round(originalSOCashPointRate, decimals);

                    int refundPoint = Convert.ToInt32(Decimal.Round(refundAmt * (1 - SOCashPointRate) * pointExchangeRate, 0));

                    decimal refundCash = refundAmt - (Decimal.Round(refundAmt * (1 - SOCashPointRate), 2));

                    //积分补偿
                    decimal pointRedeem = -Decimal.Round((Decimal.Round(refundAmt * (1 - SOCashPointRate) * pointExchangeRate, 0)
                        - (refundAmt * (1 - SOCashPointRate) * pointExchangeRate)) / pointExchangeRate, 2);

                    refundCash += pointRedeem;

                    #region 分配现金、礼品卡

                    if (refundPoint < 0)
                    {
                        refundPoint = 0;
                    }
                    if (entity.CashAmt > 0)
                    {
                        if (entity.GiftCardAmt == 0)
                        {
                            entity.CashAmt = refundCash;
                        }
                        else
                        {
                            if (entity.CashAmt > refundCash)
                            {
                                entity.GiftCardAmt = entity.CashAmt - refundCash;
                            }
                            else
                            {
                                entity.GiftCardAmt = refundCash - entity.CashAmt;
                            }
                        }
                    }
                    else
                    {
                        entity.GiftCardAmt = refundCash;
                    }
                    entity.PointAmt = refundPoint;
                    if (entity.PointAmt > availPointAmt)
                    {
                        entity.PointAmt = availPointAmt;
                    }

                    #endregion 分配现金、礼品卡

                #endregion 现金、礼品卡、积分金额划分
                }

                #endregion SOItem
            }
            else //运费补偿及其他
            {
                #region 运费补偿及其他

                if (availShipPrice < entity.CashAmt.Value)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "OverfullShipFee"));
                }
                //if (entity.CashAmt.Value < 0)
                //{
                //    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "NotGreaterThanRefundedAmt"));
                //}
                if (entity.PointAmt > 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CannotUsePointsForRefundShipFee"));
                }

                #endregion 运费补偿及其他
            }
        }
        #endregion

        #region SubmitAudit
        /// <summary>
        /// 提交审核（审核银行信息）
        /// </summary>
        /// <param name="entity"></param>
        public virtual RefundBalanceInfo SubmitAudit(RefundBalanceInfo entity)
        {
            RefundBalanceInfo refundBalanceInfo = GetRefundBalanceBySysNo(entity.SysNo.Value);
            if (refundBalanceInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundBalanceNotExists"));
            }
            refundBalanceInfo.PointAmt = refundBalanceInfo.PointAmt ?? 0;

            if (refundBalanceInfo.Status != RefundBalanceStatus.WaitingRefund)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Audit_RefundBalanceWaitingRefundValid"));
            }
            if (refundBalanceInfo.CashAmt == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CashAmtRequired"));
            }
            if (refundBalanceInfo.CashAmt.Value < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Audit_CashAmtValid"));
            }
            SOIncomeRefundInfo oldIncomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO_Balance);
            SOIncomeRefundInfo newIncomeBankInfo = new SOIncomeRefundInfo()
            {
                RefundReason = 9, // RefundReason.RefundBalance
                BankName = entity.IncomeBankInfo.BankName,
                BranchBankName = entity.IncomeBankInfo.BranchBankName,
                CardNumber = entity.IncomeBankInfo.CardNumber,
                CardOwnerName = entity.IncomeBankInfo.CardOwnerName,
                PostAddress = entity.IncomeBankInfo.PostAddress,
                PostCode = entity.IncomeBankInfo.PostCode,
                ReceiverName = entity.IncomeBankInfo.ReceiverName,
                Note = entity.IncomeBankInfo.Note,
                CompanyCode = refundBalanceInfo.CompanyCode
            };

            TransactionScopeFactory.TransactionAction(() =>
            { 
                if (oldIncomeBankInfo == null)
                {
                    newIncomeBankInfo.RefundPayType = refundBalanceInfo.RefundPayType;
                    newIncomeBankInfo.SOSysNo = refundBalanceInfo.OriginalSOSysNo;
                    newIncomeBankInfo.OrderType = RefundOrderType.RO_Balance;
                    newIncomeBankInfo.OrderSysNo = refundBalanceInfo.SysNo;
                    newIncomeBankInfo.HaveAutoRMA = false;

                    if (newIncomeBankInfo.RefundPayType == RefundPayType.CashRefund)
                    {
                        newIncomeBankInfo.Status = ECCentral.BizEntity.Invoice.RefundStatus.Audit;
                    }
                    else
                    {
                        newIncomeBankInfo.Status = ECCentral.BizEntity.Invoice.RefundStatus.Origin;
                    }

                    if (newIncomeBankInfo.RefundPayType == RefundPayType.TransferPointRefund)
                    {
                        newIncomeBankInfo.RefundCashAmt = 0;
                        newIncomeBankInfo.RefundPoint = Convert.ToInt32(Decimal.Round((refundBalanceInfo.CashAmt ?? 0M)
                            * pointExchangeRate, 0));
                        if (refundBalanceInfo.PointAmt > 0)//如果有积分累加到bankInfo的PointAmt字段上
                        {
                            newIncomeBankInfo.RefundPoint += refundBalanceInfo.PointAmt;
                        }
                    }
                    else
                    {
                        newIncomeBankInfo.RefundCashAmt = refundBalanceInfo.CashAmt;
                        newIncomeBankInfo.RefundPoint = refundBalanceInfo.PointAmt;
                    }
                    newIncomeBankInfo.RefundGiftCard = refundBalanceInfo.GiftCardAmt;

                    ExternalDomainBroker.CreateSOIncomeRefundInfo(newIncomeBankInfo);
                }
                else if (oldIncomeBankInfo.Status == RefundStatus.Origin
                    || (oldIncomeBankInfo.Status != RefundStatus.Origin && oldIncomeBankInfo.RefundPayType == RefundPayType.CashRefund))
                {
                    newIncomeBankInfo.SysNo = oldIncomeBankInfo.SysNo;
                    newIncomeBankInfo.OrderType = oldIncomeBankInfo.OrderType;
                    newIncomeBankInfo.RefundPayType = oldIncomeBankInfo.RefundPayType;
                    newIncomeBankInfo.RefundReason = oldIncomeBankInfo.RefundReason;
                    newIncomeBankInfo.HaveAutoRMA = oldIncomeBankInfo.HaveAutoRMA;
                    newIncomeBankInfo.RefundCashAmt = oldIncomeBankInfo.RefundCashAmt;
                    newIncomeBankInfo.RefundGiftCard = oldIncomeBankInfo.RefundGiftCard;
                    newIncomeBankInfo.RefundPoint = oldIncomeBankInfo.RefundPoint;
                    newIncomeBankInfo.ToleranceAmt = oldIncomeBankInfo.ToleranceAmt;
                    newIncomeBankInfo.Status = oldIncomeBankInfo.Status;

                    ExternalDomainBroker.UpdateSOIncomeRefundInfo(newIncomeBankInfo);
                }
                else if (oldIncomeBankInfo.Status == RefundStatus.Abandon)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Audit_RefundBalanceAbandonStatusValid"));
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Audit_RefundBalanceStatusValid"));
                }
                entity.IncomeBankInfo = newIncomeBankInfo;

                //完成RMA退款调整单待审核 - 待办事项:
                EventPublisher.Publish<RMACompleteRefundBalanceWaitingForAuditMessage>(new RMACompleteRefundBalanceWaitingForAuditMessage()
                {
                    RefundBalanceSysNo = entity.SysNo.Value,
                    RefundSysNo = entity.OriginalRefundSysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
            });
            return entity;
        }

        #endregion

        #region Refund
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Refund(RefundBalanceInfo entity)
        {
            RefundBalanceInfo refundBalanceInfo;
            SOInfo soInfo;
            VerifyForRefund(entity, out  soInfo, out  refundBalanceInfo);
            soInfo.BaseInfo.GiftCardPay = soInfo.BaseInfo.GiftCardPay ?? 0M;
            refundBalanceInfo.GiftCardAmt = refundBalanceInfo.GiftCardAmt ?? 0M;
            refundBalanceInfo.PointAmt = refundBalanceInfo.PointAmt ?? 0;

            decimal availShipPrice, cashRemoveGiftCard, availGiftCard, totalRoBoBalanceAmt;
            int availPointAmt;
            CalculateAvailRefundAmt(entity, soInfo.BaseInfo, out availShipPrice, out cashRemoveGiftCard, out availGiftCard, out availPointAmt, out totalRoBoBalanceAmt);

            if (refundBalanceInfo.CashAmt > cashRemoveGiftCard || refundBalanceInfo.GiftCardAmt > availGiftCard
                || refundBalanceInfo.PointAmt > availPointAmt)
            {
                throw new BizException(
                    ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_ReCreateRefundBalance"));
            }


            decimal ROAmt;
            PreCheckForRefund(refundBalanceInfo, totalRoBoBalanceAmt, availShipPrice, out ROAmt);

            int tmpNewOrderSysNo = 0;
            #region 事务中执行退款操作
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                AbandonPoint(refundBalanceInfo);

                if (refundBalanceInfo.RefundPayType == RefundPayType.PrepayRefund)
                {
                    if (refundBalanceInfo.CashAmt.Value > soInfo.BaseInfo.SOAmount)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_CashAmtValid"));
                    }

                    RefundPrepay(refundBalanceInfo);
                }
                else if (refundBalanceInfo.RefundPayType == RefundPayType.TransferPointRefund)
                {
                    if (refundBalanceInfo.CashAmt.Value > soInfo.BaseInfo.SOAmount)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_CashAmtValid"));
                    }

                    TransferPointRefund(refundBalanceInfo);
                }

                CreateSOIncome(refundBalanceInfo);

                // 现金部分生成新礼品卡
                RefundInfo refundInfo = refundProcessor.LoadBySysNo(entity.OriginalRefundSysNo.Value);
                if (refundBalanceInfo.RefundPayType == RefundPayType.GiftCardRefund && refundBalanceInfo.CashAmt > 0)
                {
                    refundProcessor.CreateElectronicGiftCard(refundInfo, refundBalanceInfo.CashAmt.Value, "ROBalance");
                }
                //礼品卡部分依次退返
                if (refundBalanceInfo.GiftCardAmt > 0)
                {
                    refundProcessor.RefundGiftCard(refundInfo, refundBalanceInfo.GiftCardAmt.Value, "ROBalance", refundBalanceInfo.SysNo.Value);
                }

                UpdateRefundBalanceForRefund(entity.SysNo.Value, ROAmt, out tmpNewOrderSysNo);

                //20130808 Chester Added:完成RMA退款调整单待审核- 待办事项:
                EventPublisher.Publish<RMACompleteRefundBalanceWaitingForAuditMessage>(new RMACompleteRefundBalanceWaitingForAuditMessage()
                {
                    RefundBalanceSysNo = entity.SysNo.Value,
                    RefundSysNo = entity.OriginalRefundSysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }
            #endregion

            //发送SSB
            string stockID = ExternalDomainBroker.GetSOItemList(soInfo.SysNo.Value).FirstOrDefault().StockSysNo.Value.ToString();

            if (stockID != null && soInfo != null)
            {
                //if (stockID.Trim() == RMAConst.WarehouseNo_GZ
                //   || stockID.Trim() == RMAConst.WarehouseNo_XA
                //   || soInfo.InvoiceInfo.IsVAT == true)
                if(soInfo.InvoiceInfo.IsVAT == true)
                {
                    ObjectFactory<SendSSBProcessor>.Instance.SendADJUSTMessage(tmpNewOrderSysNo, stockID, entity.CompanyCode);

                }
            }
        }

        /// <summary>
        /// 退款预检查.必须有退款调整单且状态为待审核&必须由退款调整单单据且状态为审核通过。
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soBaseInfo"></param>
        /// <param name="refundBalanceInfo"></param>
        private void VerifyForRefund(RefundBalanceInfo entity, out SOInfo soInfo, out RefundBalanceInfo refundBalanceInfo)
        {
            refundBalanceInfo = GetRefundBalanceBySysNo(entity.SysNo.Value);
            if (refundBalanceInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundBalanceNotExists"));
            }
            if (refundBalanceInfo.Status != RefundBalanceStatus.WaitingRefund)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_RefundBalanceStatusValid"));
            }
            if (refundBalanceInfo.CashAmt == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CashAmtRequired"));
            }
            if (refundBalanceInfo.CashAmt.Value >= 0)
            {
                SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(entity.SysNo.Value, RefundOrderType.RO_Balance);
                if (incomeBankInfo == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_SOIncomeRefundInfoValid"));
                }
                else if (incomeBankInfo.Status != ECCentral.BizEntity.Invoice.RefundStatus.Audit)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Refund_SOIncomeStatusValid"));
                }
            }
            soInfo = ExternalDomainBroker.GetSOInfo(entity.OriginalSOSysNo.Value);
            if (soInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "SONotExists"));
            }
        }

        /// <summary>
        /// 退款前的业务检查
        /// </summary>
        /// <param name="refundInfo"></param>
        private void PreCheckForRefund(RefundBalanceInfo refundBalanceInfo, decimal totalRoBoBalanceAmt, decimal availShipPrice, out decimal ROAmt)
        {
            refundBalanceInfo.PointAmt = refundBalanceInfo.PointAmt.HasValue ? refundBalanceInfo.PointAmt.Value : 0;
            refundBalanceInfo.GiftCardAmt = refundBalanceInfo.GiftCardAmt.HasValue ? refundBalanceInfo.GiftCardAmt.Value : 0M;

            decimal point = refundBalanceInfo.PointAmt.Value;
            ROAmt = refundBalanceInfo.CashAmt.Value + refundBalanceInfo.GiftCardAmt.Value + (point / pointExchangeRate);

            if (refundBalanceInfo.ProductSysNo != 0)   // 正常商品调整
            {
                RefundItemInfo refundItem = refundBalanceDA.GetRefundTotalAmount(refundBalanceInfo);
                decimal tempRefunded = (refundItem.RefundPrice ?? 0M) + ROAmt + totalRoBoBalanceAmt;
                decimal productValue = refundItem.ProductValue ?? 0;
                if (productValue < tempRefunded)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundTooMuch"));
                }
                if (tempRefunded < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "NotGreaterThanRefundedAmt"));
                }
            }
            else   // 运费补偿及其他
            {
                if (availShipPrice < ROAmt)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "OverfullShipFee"));
                }
                //if (ROAmt < 0)
                //{
                //    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "NotGreaterThanRefundedAmt"));
                //}
                if (refundBalanceInfo.PointAmt > 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "CannotUsePointsForRefundShipFee"));
                }
            }
        }

        /// <summary>
        /// 积分撤销操作
        /// </summary>
        /// <param name="refundInfo"></param>
        private void AbandonPoint(RefundBalanceInfo refundBalanceInfo)
        {
            int pointAmount = refundBalanceInfo.PointAmt.Value;
            if (refundBalanceInfo.PointAmt > 0)
            {
                AdjustPointRequest itemPointInfo = new AdjustPointRequest()
                {
                    CustomerSysNo = refundBalanceInfo.CustomerSysNo.Value,
                    Point = pointAmount,
                    PointType = (int)AdjustPointType.ReturnProductPoint,
                    Memo = "RoBalanceRefundPointPart",
                    SOSysNo = refundBalanceInfo.OriginalSOSysNo,
                    Source = "RMA Domain",
                    OperationType = AdjustPointOperationType.Abandon
                };
                ExternalDomainBroker.AdjustPoint(itemPointInfo);
            }
        }

        /// <summary>
        /// 退入客户余额帐户
        /// </summary>
        /// <param name="refundBalanceInfo"></param>
        private void RefundPrepay(RefundBalanceInfo refundBalanceInfo)
        {
            // 退入客户余额帐户
            decimal prepayAmount = refundBalanceInfo.CashAmt.Value;
            if (prepayAmount != 0)
            {
                CustomerPrepayLog prepayLogInfo = new CustomerPrepayLog()
                {
                    SOSysNo = refundBalanceInfo.OriginalSOSysNo.Value,
                    CustomerSysNo = refundBalanceInfo.CustomerSysNo.Value,
                    AdjustAmount = prepayAmount,
                    PrepayType = PrepayType.RO_BalanceReturn,
                    Note = ResouceManager.GetMessageString("RMA.RefundBalance", "PrepayLog_Note")
                };

                ExternalDomainBroker.AdjustPrePay(prepayLogInfo);
            }
        }

        /// <summary>
        /// 系统直接发放或者收回用户积分
        /// </summary>
        /// <param name="refundBalanceInfo"></param>
        private void TransferPointRefund(RefundBalanceInfo refundBalanceInfo)
        {
            int pointAmount = Convert.ToInt32(Decimal.Round(refundBalanceInfo.CashAmt.Value * pointExchangeRate, 0));
            if (pointAmount != 0)
            {
                AdjustPointRequest transferPointInfo = new AdjustPointRequest()
                {
                    CustomerSysNo = refundBalanceInfo.CustomerSysNo.Value,
                    Point = pointAmount,
                    PointType = (int)AdjustPointType.RefundCashToPoints,
                    Memo = refundBalanceInfo.SysNo.ToString(),
                    SOSysNo = refundBalanceInfo.OriginalSOSysNo,
                    OperationType = AdjustPointOperationType.AddOrReduce
                };
                ExternalDomainBroker.AdjustPoint(transferPointInfo);
            }
        }

        /// <summary>
        /// 创建财务收款单
        /// </summary>
        /// <param name="refundBalanceInfo"></param>
        private void CreateSOIncome(RefundBalanceInfo refundBalanceInfo)
        {
            SOIncomeInfo soIncomeInfo = new SOIncomeInfo()
            {
                OrderSysNo = refundBalanceInfo.SysNo.Value,
                OrderType = SOIncomeOrderType.RO_Balance,
                OrderAmt = refundBalanceInfo.CashAmt.Value * -1,
                IncomeAmt = (refundBalanceInfo.RefundPayType == RefundPayType.TransferPointRefund)
                ? 0 : refundBalanceInfo.CashAmt.Value * -1,
                GiftCardPayAmt = -refundBalanceInfo.GiftCardAmt.Value,
                PointPay = -refundBalanceInfo.PointAmt.Value,
                IncomeStyle = SOIncomeOrderStyle.RO_Balance,
                ReferenceID = "",
                Status = SOIncomeStatus.Origin,
                Note = "",
                CompanyCode = refundBalanceInfo.CompanyCode
            };

            ExternalDomainBroker.CreateSOIncome(soIncomeInfo);

            if (refundBalanceInfo.RefundPayType == RefundPayType.TransferPointRefund)
            {
                int userSysNo = ExternalDomainBroker.GetUserSysNo(AppSettingManager.GetSetting("RMA", RMAConst.AutoRMAPhysicalUserName),
                            AppSettingManager.GetSetting("RMA", RMAConst.AutoRMALoginUserName), AppSettingManager.GetSetting("RMA", RMAConst.AutoRMASourceDirectoryKey));
                ExternalDomainBroker.AutoConfirmIncomeInfo(soIncomeInfo.SysNo.Value, refundBalanceInfo.SysNo.Value, userSysNo);
            }
        }

        /// <summary>
        /// 退款后更新退款调整单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="ROAmt"></param>
        private void UpdateRefundBalanceForRefund(int sysNo, decimal ROAmt, out int NewOrderSysNo)
        {

            RefundBalanceInfo newEntity = new RefundBalanceInfo();
            if (ROAmt > 0)
            {
                newEntity.NewOrderSysNo = ObjectFactory<IRefundDA>.Instance.CreateSysNo();
                newEntity.BalanceOrderType = RefundBalanceType.RO;
            }
            else//生成虚拟SO
            {
                newEntity.NewOrderSysNo = ExternalDomainBroker.NewSOSysNo();
                newEntity.BalanceOrderType = RefundBalanceType.SO;
            }

            NewOrderSysNo = newEntity.NewOrderSysNo.Value;

            newEntity.SysNo = sysNo;
            newEntity.Status = RefundBalanceStatus.Refunded;
            newEntity.RefundTime = DateTime.Now;
            newEntity.RefundUserSysNo = ServiceContext.Current.UserSysNo;
            this.refundBalanceDA.UpdateRefundBalance(newEntity);
        }
        #endregion

        #region Abound
        /// <summary>
        /// 作废退款调整单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Abandon(int sysNo)
        {
            RefundBalanceInfo refundBalanceInfo = GetRefundBalanceBySysNo(sysNo);
            if (refundBalanceInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "RefundBalanceNotExists"));
            }
            if (refundBalanceInfo.Status != RefundBalanceStatus.WaitingRefund)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Void_RefundBalanceStatusValid"));
            }
            RefundBalanceInfo newEntity = new RefundBalanceInfo()
            {
                SysNo = refundBalanceInfo.SysNo,
                Status = RefundBalanceStatus.Abandon
            };

            SOIncomeRefundInfo incomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(refundBalanceInfo.SysNo.Value, RefundOrderType.RO_Balance);

            TransactionScopeFactory.TransactionAction(()=>
            {
                if (incomeBankInfo != null)
                {
                    if (incomeBankInfo.Status != RefundStatus.Origin)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.RefundBalance", "Void_SOIncomeStatusValid"));
                    }
                    ExternalDomainBroker.AbandonSOIncomeRefundForROBalance(incomeBankInfo.SysNo.Value);

                    //20130808 Chester Added: RO_Balance取消审核 - 待办事项:
                    EventPublisher.Publish<RMAROBalanceCancelAuditMessage>(new RMAROBalanceCancelAuditMessage()
                    {
                        SOIncomeRefundSysNo = incomeBankInfo.SysNo.Value,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                }

                refundBalanceDA.UpdateRefundBalance(newEntity);

                //20130808 Chester Added 完成RMA退款调整单待审核 - 待办事项:
                EventPublisher.Publish<RMACompleteRefundBalanceWaitingForAuditMessage>(new RMACompleteRefundBalanceWaitingForAuditMessage()
                {
                    RefundBalanceSysNo = refundBalanceInfo.SysNo.Value,
                    RefundSysNo = refundBalanceInfo.OriginalRefundSysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
            });
        }

        #endregion

        #region 公用方法
        /// <summary>
        /// 查询得到运费信息
        /// </summary>
        /// <param name="refundSysNo">退款单号</param>
        /// <param name="soInfo">销售单</param>
        /// <returns>运费总额</returns>
        public decimal GetShipFee(int soSysNo, string stockID)
        {
            decimal totalAmount = 0M;
            int stockSysNo = Convert.ToInt32(stockID);
            var invoice = ExternalDomainBroker.GetSOInvoiceMaster(soSysNo);
            invoice = invoice.Where(p => p.StockSysNo == stockSysNo).ToList();
            if (invoice != null)
            {
                invoice.ForEach(p =>
                {
                    totalAmount += (p.PremiumAmt.Value + p.ShippingCharge.Value + p.ExtraAmt.Value);
                });
            }

            return totalAmount;
        }

        /// <summary>
        /// 计算可退款金额（运费，现金，礼品卡，积分）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soBaseInfo"></param>
        /// <param name="availShipPrice">可退运费</param>
        /// <param name="cashRemoveGiftCard">可退现金</param>
        /// <param name="availGiftCard">可退礼品卡</param>
        /// <param name="availPointAmt">可退积分</param>
        /// <param name="totalRoBoBalanceAmt">历史退款总额</param>
        private void CalculateAvailRefundAmt(RefundBalanceInfo entity, SOBaseInfo soBaseInfo, out decimal availShipPrice,
            out decimal cashRemoveGiftCard, out decimal availGiftCard, out int availPointAmt, out decimal totalRoBoBalanceAmt)
        {
            string stockID = ExternalDomainBroker.GetSOItemList(soBaseInfo.SysNo.Value).FirstOrDefault().StockSysNo.Value.ToString();

            #region 历史退款

            //正常商品退款总额
            HistoryRefundAmount historyAmt = refundProcessor.GetHistoryRefundAmt(entity.OriginalSOSysNo.Value, entity.ProductSysNo.Value, stockID);
            decimal totalGiftAmtHistory = historyAmt.TotalGiftCardAmt;
            int totalPointAmtHistory = historyAmt.TotalPointAmt;
            totalRoBoBalanceAmt = historyAmt.TotalRoBalanceAmt;
            historyAmt = refundProcessor.GetHistoryRefundAmt(entity.OriginalSOSysNo.Value, 0, stockID);
            decimal totalCashAmtHistory = historyAmt.TotalCashAmt;
            //运费补偿退款总额
            decimal totalShipPriceAmtHistory = historyAmt.TotalShipPriceAmt;

            #endregion 历史退款

            #region 计算可退现金/积分/礼品卡

            //运费总额（运费+保价费+附加费）
            decimal shipPrice = GetShipFee(soBaseInfo.SysNo.Value, stockID);
            //可退运费（运费总额-已退运费总额）
            availShipPrice = shipPrice - totalShipPriceAmtHistory;
            if (availShipPrice < 0)
            {
                availShipPrice = 0;
            }
            //可退现金
            cashRemoveGiftCard = soBaseInfo.CashPay + shipPrice + soBaseInfo.PromotionAmount.Value
                - soBaseInfo.GiftCardPay.Value - totalCashAmtHistory;

            if (soBaseInfo.GiftCardPay == 0 && availShipPrice > 0 && entity.ProductSysNo != 0)
            {
                cashRemoveGiftCard -= availShipPrice;
            }
            if (cashRemoveGiftCard < 0)
            {
                cashRemoveGiftCard = 0;
            }
            ///可退礼品卡金额
            availGiftCard = soBaseInfo.GiftCardPay.Value - totalGiftAmtHistory;

            if (soBaseInfo.GiftCardPay.Value > 0 && availShipPrice > 0 && entity.ProductSysNo != 0)
            {
                availGiftCard -= availShipPrice;
            }
            if (availGiftCard < 0)
            {
                availGiftCard = 0;
            }
            //可退积分
            availPointAmt = soBaseInfo.PointPay.Value - totalPointAmtHistory;
            if (availPointAmt < 0)
            {
                availPointAmt = 0;
            }

            #endregion 计算可退现金/积分/礼品卡
        }

        /// <summary>
        /// 根据系统编号获取调整单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo GetRefundBalanceBySysNo(int sysNo)
        {
            return refundBalanceDA.GetRefundBalanceBySysNo(sysNo);
        }

        #endregion
    }
}