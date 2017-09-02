using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(SOIncomeRefundProcessor))]
    public class SOIncomeRefundProcessor
    {
        private ISOIncomeRefundDA m_SOIncomeRefundDA = ObjectFactory<ISOIncomeRefundDA>.Instance;
        TransactionOptions options = new TransactionOptions();
        /// <summary>
        /// 创建销售退款单
        /// </summary>
        /// <param name="entity">退款信息</param>
        /// <param name="payAmt">实收金额,如果实收金额大于0，则优先退还现金</param>
        public virtual SOIncomeRefundInfo Create(SOIncomeRefundInfo entity)
        {
            PreCheckForCreate(entity);
            SOIncomeRefundInfo result = new SOIncomeRefundInfo();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {

                if (entity.OrderType == RefundOrderType.OverPayment)
                {
                    ObjectFactory<SOIncomeProcessor>.Instance.CreateNegative(entity);
                }
                result = m_SOIncomeRefundDA.Create(entity);

                //发送cs审核退款审核Message
                EventPublisher.Publish(new CreateSOIncomeRefundInfoMessage()
                {
                    SOIncomeRefundSysNo = result.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                ts.Complete();
            }
            return result;
        }

        /// <summary>
        /// 创建销售退款单同Create方法，去事务（防止与外层不同事务级别不一致引发异常）
        /// </summary>
        /// <param name="entity">退款信息</param>
        /// <param name="payAmt">实收金额,如果实收金额大于0，则优先退还现金</param>
        public virtual SOIncomeRefundInfo CreateSOIncomeRefund(SOIncomeRefundInfo entity)
        {
            PreCheckForCreate(entity);
            SOIncomeRefundInfo result = new SOIncomeRefundInfo();

            if (entity.OrderType == RefundOrderType.OverPayment)
            {
                ObjectFactory<SOIncomeProcessor>.Instance.CreateNegative(entity);
            }
            result = m_SOIncomeRefundDA.Create(entity);

            //发送cs审核退款审核Message
            EventPublisher.Publish(new CreateSOIncomeRefundInfoMessage()
            {
                SOIncomeRefundSysNo = result.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            return result;
        }

        /// <summary>
        /// 创建预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForCreate(SOIncomeRefundInfo entity)
        {
            //现在执行非空数据检查
            CheckRequiredFields(entity);

            //如果单据类型是AO或多付款退款,则不允许"转积分退款"
            if ((entity.OrderType == RefundOrderType.AO || entity.OrderType == RefundOrderType.OverPayment)
                && entity.RefundPayType == RefundPayType.TransferPointRefund)
            {
                ThrowBizException("SOIncomeRefund_CanNotTransferPoint");
            }
        }

        /// <summary>
        /// 更新销售退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(SOIncomeRefundInfo entity)
        {
            PreCheckForUpdate(entity);

            //fixbug:在财务管理-退款审核对退款信息进行编辑后，需要和RMA进行同步
            if (entity.OrderType == RefundOrderType.RO)
            {
                //调用RMA服务，更新RO单的退款类型和退看原因
                ExternalDomainBroker.UpdateRefundPayTypeAndReason(entity.OrderSysNo.Value, (int)entity.RefundPayType, entity.RefundReason.Value);
            }

            m_SOIncomeRefundDA.Update(entity);
        }

        /// <summary>
        /// 更新销售退款单前预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForUpdate(SOIncomeRefundInfo entity)
        {
            CheckRequiredFields(entity);

            var refundInfo = LoadBySysNo(entity.SysNo.Value);
            var refundStatus = refundInfo.Status;

            //如果退款已经“审核通过”或者“审核拒绝”，则不允许修改。
            if (refundStatus != RefundStatus.WaitingFinAudit && refundStatus != RefundStatus.Origin && refundStatus != RefundStatus.WaitingRefund
                && !(refundStatus == RefundStatus.Audit && refundInfo.RefundPayType == RefundPayType.BankRefund))
            {
                ThrowBizException("SOIncomeRefund_CanNotEdit");
            }

            //如果单据类型是AO或多付款退款,则不允许"转积分退款"
            if ((entity.OrderType == RefundOrderType.AO || entity.OrderType == RefundOrderType.OverPayment)
                && entity.RefundPayType == RefundPayType.TransferPointRefund)
            {
                ThrowBizException("SOIncomeRefund_CanNotTransferPoint");
            }
        }

        /// <summary>
        /// CS审核退款单
        /// </summary>
        public virtual void CSAudit(SOIncomeRefundInfo info)
        {
            var entity = LoadBySysNo(info.SysNo.Value);

            PreCheckForCSAudit(entity, info);

            var soEntity = ExternalDomainBroker.GetSOInfo(entity.SOSysNo.Value);

            //判断订单是否是【增票订单】
            if (soEntity == null)
            {
                //ThrowBizException("没有找到对应的订单");
                ThrowBizException("SOIncomeRefund_OrderNotFound");
            }
            //add by norton 2012-11-7

            {
                if (!soEntity.InvoiceInfo.IsVAT.Value)
                {
                    //不是【增票订单】,状态直接改为【已审核】
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        var flag = m_SOIncomeRefundDA.UpdateStatus(entity.SysNo.Value, ServiceContext.Current.UserSysNo, RefundStatus.Audit, DateTime.Now);
                        if (flag)
                        {
                            PassAudit(entity);
                        }


                        //发送cs审核退款审核Message
                        EventPublisher.Publish(new RefundCSAuditMessage()
                        {
                            RefundSysNo = entity.SysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        ts.Complete();
                    }
                    //记录操作日志
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                        GetMessageString("SOIncomeRefund_Log_CSAudit", ServiceContext.Current.UserSysNo, entity.SysNo)
                       , BizLogType.AuditRefund_Update
                       , entity.SysNo.Value
                       , entity.CompanyCode);
                }
                else
                {
                    //是【增票订单】，状态改为【待财务审核】
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        m_SOIncomeRefundDA.UpdateStatus(entity.SysNo.Value, ServiceContext.Current.UserSysNo, RefundStatus.WaitingFinAudit, DateTime.Now);

                        //发送cs审核退款审核Message
                        EventPublisher.Publish(new RefundCSAuditMessage()
                        {
                            RefundSysNo = entity.SysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        ts.Complete();
                    }

                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                        GetMessageString("SOIncomeRefund_Log_CSAuditIsVAT", ServiceContext.Current.UserSysNo, entity.SysNo)
                        , BizLogType.AuditRefund_Update
                        , entity.SysNo.Value
                        , entity.CompanyCode);
                }
            }
        }

        /// <summary>
        /// CS审核退款单前预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForCSAudit(SOIncomeRefundInfo entity, SOIncomeRefundInfo info)
        {
            ////创建人，客服审核人，财务审核人不能相同
            //if (entity.CreateUserSysNo == info.AuditUserSysNo)
            //{
            //    ThrowBizException("SOInComeRefund_CSAudit_CreateUserAndAuditUserCanNotSame");
            //}
            if (entity.Status != RefundStatus.Origin)
            {
                ThrowBizException("SOIncomeRefund_CSAudit_CanNotAudit");
            }
            //多付款退款单需要NetPay或邮局电汇审核通过后才能进行审核
            if (entity.OrderType == RefundOrderType.OverPayment)
            {
                NetPayInfo netPayQueryCriteria = new NetPayInfo
                {
                    SOSysNo = entity.SOSysNo,
                    Status = NetPayStatus.Approved
                };
                List<NetPayInfo> netPayList = ObjectFactory<NetPayProcessor>.Instance.GetListByCriteria(netPayQueryCriteria);
                if (netPayList.Count == 0)
                {
                    List<PostPayInfo> postPayList = ObjectFactory<PostPayProcessor>.Instance.GetListBySOSysNoAndStatus(entity.SOSysNo.Value, PostPayStatus.Yes, PostPayStatus.Splited);
                    if (postPayList.Count == 0)
                    {
                        ThrowBizException("SOIncomeRefund_CSAudit_NoValidNetPayOrPostPay");
                    }
                }
            }
        }

        /// <summary>
        /// 审核通过的处理
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PassAudit(SOIncomeRefundInfo entity)
        {

            if (entity.RefundCashAmt > 0
                   && entity.RefundPayType == RefundPayType.GiftCardRefund
                   && (entity.OrderType == RefundOrderType.AO || entity.OrderType == RefundOrderType.OverPayment))
            {
                //调用IM接口创建一张电子礼品卡，礼品卡类型为[CS补偿电子卡]
                var giftCardResultCode = ExternalDomainBroker.CreateElectronicGiftCard(entity.SOSysNo.Value, entity.CustomerSysNo.Value, 1, entity.RefundCashAmt.Value
                    , ECCentral.BizEntity.IM.GiftCardType.Compensate, "退款转礼品卡", entity.CompanyCode);

                //发送邮件通知用户
                SendGiftCardEmail(entity, giftCardResultCode);

                //记录操作日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("SOIncomeRefund_Log_PassAudit", ServiceContext.Current.UserSysNo, entity.SysNo, entity.RefundCashAmt)
                    , BizLogType.AuditRefund_Update
                    , entity.SysNo.Value
                    , entity.CompanyCode);
            }

            //更新用户账户余额
            if (entity.RefundPayType == RefundPayType.PrepayRefund && entity.RefundCashAmt != 0)
            {
                if (entity.OrderType == RefundOrderType.AO)
                {
                    ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, entity.RefundCashAmt.Value, PrepayType.BOReturn, GetMessageString("SOIncomeRefund_AuditAOAdjustBalanceNote"));
                }
                else if (entity.OrderType == RefundOrderType.OverPayment)
                {
                    ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, entity.RefundCashAmt.Value, PrepayType.RemitReturn, GetMessageString("SOIncomeRefund_AuditOverPayAdjustBalanceNote"));
                }
                else if (entity.OrderType == RefundOrderType.RO && entity.HaveAutoRMA == true)
                {
                    ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, entity.RefundCashAmt.Value, PrepayType.ROReturn, GetMessageString("SOIncomeRefund_AuditROShipRejectPrepayRefundNote"));
                }
                //else if (entity.OrderType == RefundOrderType.RO_Adjsut)
                //{
                //    ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, entity.RefundCashAmt.Value, PrepayType.RO_AdjustReturn, GetMessageString("SOIncomeRefund_AuditROAdjustNote"));
                //}
            }
        }

        /// <summary>
        /// CS审核拒绝退款单
        /// </summary>
        public virtual void CSReject(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);

            PreCheckForCSReject(entity);



            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_SOIncomeRefundDA.UpdateStatus(sysNo, ServiceContext.Current.UserSysNo, RefundStatus.Abandon, DateTime.Now);

                //发送cs审核退款审核Message
                EventPublisher.Publish(new RefundCSCancelAuditMessage()
                {
                    RefundSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }

            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("SOIncomeRefund_Log_CSReject", ServiceContext.Current.UserSysNo, entity.SysNo)
                 , BizLogType.AuditRefund_Update
                 , entity.SysNo.Value
                 , entity.CompanyCode);
        }

        /// <summary>
        /// CS审核拒绝退款单前预检查
        /// </summary>
        protected void PreCheckForCSReject(SOIncomeRefundInfo entity)
        {
            if (entity.Status != RefundStatus.Origin)
            {
                ThrowBizException("SOIncomeRefund_CanNotAbandon");
            }
        }

        /// <summary>
        /// 财务审核退款单
        /// </summary>
        public virtual void FinAudit(SOIncomeRefundInfo info)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(info.SysNo.Value);

            PreCheckForFinAudit(entity, info);

            var flag = m_SOIncomeRefundDA.UpdateStatus(info.SysNo.Value, ServiceContext.Current.UserSysNo, RefundStatus.Audit, DateTime.Now);
            if (flag)
            {
                PassAudit(entity);
                //记录操作日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("SOIncomeRefund_Log_FinAudit", ServiceContext.Current.UserSysNo, entity.SysNo)
                   , BizLogType.AuditRefund_Update
                   , entity.SysNo.Value
                   , entity.CompanyCode);
            }
        }

        /// <summary>
        /// 财务审核退款单前预检查
        /// </summary>
        /// <param name="entity"></param>
        protected void PreCheckForFinAudit(SOIncomeRefundInfo entity, SOIncomeRefundInfo info)
        {
            //创建人，客服审核人，财务审核人不能相同
            //if (entity.CreateUserSysNo == info.AuditUserSysNo)
            //{
            //    ThrowBizException("SOInComeRefund_CSAudit_CreateUserAndAuditUserCanNotSame");
            //}
            if (entity.Status != RefundStatus.WaitingFinAudit)
            {
                ThrowBizException("SOIncomeRefund_FinAudit_StatusNotMatchWaitingFinAudit");
            }
        }

        /// <summary>
        /// 财务审核拒绝退款单
        /// </summary>
        public virtual void FinReject(int sysNo, string appendFinNote)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);

            PreCheckForFinReject(entity, appendFinNote);

            //新的财务备注和原来的财务备注之间用分号；隔开
            string finNote = string.Concat(entity.FinNote, ";", appendFinNote).TrimStart(';').TrimEnd(';');

            m_SOIncomeRefundDA.UpdateStatusAndFinNote(sysNo, ServiceContext.Current.UserSysNo, RefundStatus.Origin, DateTime.Now, finNote);

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("SOIncomeRefund_Log_FinReject", ServiceContext.Current.UserSysNo, entity.SysNo)
                 , BizLogType.AuditRefund_Update
                 , entity.SysNo.Value
                 , entity.CompanyCode);
        }

        /// <summary>
        /// 财务审核拒绝退款单前预检查
        /// </summary>
        /// <param name="entity"></param>
        protected void PreCheckForFinReject(SOIncomeRefundInfo entity, string appendFinNote)
        {
            if (entity.Status != RefundStatus.WaitingFinAudit)
            {
                ThrowBizException("SOIncomeRefund_FinReject_StatusNotMatchWaitingFinAudit");
            }

            if (entity.FinNote.EmptyIfNull().Length + appendFinNote.EmptyIfNull().Length > 500)
            {
                ThrowBizException("SOIncomeRefund_FinNoteLengthMoreThan500");
            }
        }

        /// <summary>
        /// 取消审核退款单
        /// </summary>
        public virtual void CancelAudit(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);

            PreCheckForCancelAudit(entity);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //如果是补偿退款单，需要回写状态，并且作废对应的销售-收款单

                bool flag = m_SOIncomeRefundDA.UpdateStatus(sysNo, ServiceContext.Current.UserSysNo, RefundStatus.Origin, DateTime.Now);
                if (flag)
                {
                    if (entity.RefundPayType == RefundPayType.PrepayRefund && entity.RefundCashAmt != 0)
                    {
                        if (entity.OrderType == RefundOrderType.AO)
                        {
                            ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, -entity.RefundCashAmt.Value, PrepayType.BOReturn
                                , GetMessageString("SOIncomeRefund_CancelAudit_AOAdjustBalanceNote"));
                        }
                        else if (entity.OrderType == RefundOrderType.OverPayment)
                        {
                            ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, -entity.RefundCashAmt.Value, PrepayType.RemitReturn
                                , GetMessageString("SOIncomeRefund_CancelAudit_OverPayAdjustBalanceNote"));
                        }
                        else if (entity.OrderType == RefundOrderType.RO && entity.HaveAutoRMA == true)
                        {
                            ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, -entity.RefundCashAmt.Value, PrepayType.ROReturn,
                                GetMessageString("SOIncomeRefund_CancelAudit_ROAdjustShipRejectPrepayRefundNote"));
                        }
                        //else if (entity.OrderType == RefundOrderType.RO_Adjsut)
                        //{
                        //    ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, entity.OrderSysNo.Value, -entity.RefundCashAmt.Value, PrepayType.RO_AdjustReturn,
                        //        GetMessageString("SOIncomeRefund_CancelAudit_ROAdjustNote"));
                        //}
                    }
                    ts.Complete();
                }
            }
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("SOIncomeRefund_Log_CancelAudit", ServiceContext.Current.UserSysNo, entity.SysNo)
              , BizLogType.AuditRefund_Update
              , entity.SysNo.Value
              , entity.CompanyCode);
        }

        /// <summary>
        /// 取消审核退款单前预检查
        /// </summary>
        protected void PreCheckForCancelAudit(SOIncomeRefundInfo entity)
        {
            //审核状态不等于已审核，不能取消审核。
            if (entity.Status != RefundStatus.Audit)
            {
                ThrowBizException("SOIncomeRefund_CancelAudit_StatusNotMatchAudit");
            }
            //如果单据类型为“RO”，并且对应的RO的状态为”已经作废”或者”已经退款”，则取消审核失败。
            if (entity.OrderType == RefundOrderType.RO)
            {
                var roEntity = ExternalDomainBroker.GetRefundBySysNo(entity.OrderSysNo.Value);
                if (roEntity.Status == RMARefundStatus.Abandon || roEntity.Status == RMARefundStatus.Refunded)
                {
                    ThrowBizException("SOIncomeRefund_CancelAudit_ROStatusNotMatchAbandonOrRefunded");
                }
            }
            //如果单据类型为“AO”或者”多付款退款单”或者“补偿退款单”，并且对应的“收款单”的状态为”已经审核”，则取消审核失败。
            else if (entity.OrderType == RefundOrderType.AO
                || entity.OrderType == RefundOrderType.OverPayment
               )
            {
                List<SOIncomeInfo> soIncomeList = ObjectFactory<SOIncomeProcessor>.Instance.GetListByCriteria(null
                                                                                                                , entity.OrderSysNo
                                                                                                                , (SOIncomeOrderType)((int)entity.OrderType)
                                                                                                                , new List<SOIncomeStatus> { SOIncomeStatus.Confirmed });
                if (soIncomeList != null && soIncomeList.Count > 0)
                {
                    ThrowBizException("SOIncomeRefund_CancelAudit_FinancialAlreadyConfirmed");
                }
            }
            // 如果单据类型为“RO_Balance”, 并且对应的RO_Balance的状态为”已经作废”或者”已经退款”，则取消审核失败。
            else if (entity.OrderType == RefundOrderType.RO_Balance)
            {
                if (entity.RefundPayType == RefundPayType.CashRefund)
                {
                    ThrowBizException("SOIncomeRefund_CancelAudit_ROBalanceCashRefundCanNotCancel");
                }
                var roBalanceEntity = ExternalDomainBroker.GetRefundBalanceBySysNo(entity.OrderSysNo.Value);
                if (roBalanceEntity.Status != RefundBalanceStatus.WaitingRefund)
                {
                    ThrowBizException("SOIncomeRefund_CancelAudit_ROBalanceAbandonOrRefunded");
                }
            }
            //已生成礼品卡，不能取消审核
            if (entity.Status != RefundStatus.Abandon
                && entity.RefundCashAmt > 0
                && entity.RefundPayType == RefundPayType.GiftCardRefund
                && (entity.OrderType == RefundOrderType.AO || entity.OrderType == RefundOrderType.OverPayment))
            {
                ThrowBizException("SOIncomeRefund_CancelAudit_GeneratedGiftCard");
            }
        }

        /// <summary>
        /// 获得单个财务退款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SOIncomeRefundInfo LoadBySysNo(int sysNo)
        {
            SOIncomeRefundInfo refundInfo = m_SOIncomeRefundDA.LoadBySysNo(sysNo);
            if (refundInfo != null)
            {
                return refundInfo;
            }
            ThrowBizException("SOIncomeRefund_RecordNotExist", sysNo);
            return null;
        }

        /// <summary>
        /// 获取财务退款单列表
        /// </summary>
        /// <param name="orderType">业务单据类型</param>
        /// <param name="status">退款单状态</param>
        /// <param name="orderSysNo">业务单据系统编号</param>
        /// <returns></returns>
        public virtual List<SOIncomeRefundInfo> GetListByCriteria(SOIncomeRefundInfo query)
        {
            return m_SOIncomeRefundDA.GetListByCriteria(query);
        }



        /// <summary>
        /// 检查非空字段
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CheckRequiredFields(SOIncomeRefundInfo entity)
        {
            #region 如果是银行转账退款且物流拒收，则银行名称、支行名称、银行卡号、持卡人不能为空

            if (entity.RefundPayType == RefundPayType.BankRefund)
            {
                if (!entity.HaveAutoRMA.HasValue)
                {
                    if (string.IsNullOrEmpty(entity.BankName))
                    {
                        ThrowBizException("SOIncomeRefund_BankRequired");
                    }
                    if (string.IsNullOrEmpty(entity.BranchBankName))
                    {
                        ThrowBizException("SOIncomeRefund_BranchBankRequired");
                    }
                    if (string.IsNullOrEmpty(entity.CardNumber))
                    {
                        ThrowBizException("SOIncomeRefund_CardNumberRequired");
                    }
                    if (string.IsNullOrEmpty(entity.CardOwnerName))
                    {
                        ThrowBizException("SOIncomeRefund_CardOwnerRequired");
                    }
                }
            }

            #endregion 如果是银行转账退款且物流拒收，则银行名称、支行名称、银行卡号、持卡人不能为空

            #region 如果是邮局转账退款，则邮政地址、邮政编码、收款人不能为空且格式要正确

            else if (entity.RefundPayType == RefundPayType.PostRefund)
            {
                if (string.IsNullOrEmpty(entity.PostAddress))
                {
                    ThrowBizException("SOIncomeRefund_PostAddressRequired");
                }
                if (string.IsNullOrEmpty(entity.PostCode))
                {
                    ThrowBizException("SOIncomeRefund_ZipCodeRequired");
                }
                if (!Regex.IsMatch(entity.PostCode, @"^\d{6}$"))
                {
                    ThrowBizException("SOIncomeRefund_ZipCodeInvalid");
                }
                if (string.IsNullOrEmpty(entity.ReceiverName))
                {
                    ThrowBizException("SOIncomeRefund_ReceiverRequired");
                }
            }

            #endregion 如果是邮局转账退款，则邮政地址、邮政编码、收款人不能为空且格式要正确

            #region 如果是网关转账退款，则银行名称不能为空

            else if (entity.RefundPayType == RefundPayType.NetWorkRefund)
            {
                //if (string.IsNullOrEmpty(entity.BankName))
                //{
                //    ThrowBizException("SOIncomeRefund_BankRequired");
                //}
            }

            #endregion 如果是网关转账退款，则银行名称不能为空
        }

        /// <summary>
        /// 审核RMA物流拒收
        /// </summary>
        /// <param name="entity"></param>
        public virtual void AuditAutoRMA(int sysNo)
        {
            //使用DB中的数据验证业务逻辑
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);

            //RMA物流拒收并且状态为"待RMA退款"
            if (entity.HaveAutoRMA == false)
            {
                ThrowBizException("SOIncomeRefund_AuditAutoRMA_NotShipRejected");
            }
            if (entity.Status != RefundStatus.WaitingRefund)
            {
                ThrowBizException("SOIncomeRefund_AuditAutoRMA_NotWaitingRefund");
            }

            //根据退款单号查询退款金额，并计算积分
            var roEntity = ExternalDomainBroker.GetRefundBySysNo(entity.OrderSysNo.Value);

            entity.RefundPoint = roEntity.PointAmt - roEntity.DeductPointFromAccount;
            entity.RefundCashAmt = roEntity.CashAmt;

            entity.Status = RefundStatus.Origin;

            //TODO:重写方法，判断第三方订单支付方式，比如如果是淘宝订单该作何处理
            AuditAutoRMAForThirdSO(entity);

            m_SOIncomeRefundDA.Update(entity);
        }

        /// <summary>
        /// 第三方平台订单审核RMA物流拒收
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AuditAutoRMAForThirdSO(SOIncomeRefundInfo entity)
        {
            //TODO:重写方法，判断第三方订单支付方式，比如如果是淘宝订单该作何处理
            //中蛋逻辑：如果是淘宝订单，则将状态更新为已审核状态。
        }

        /// <summary>
        /// 退款转礼品卡成功则像客户发送一封邮件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="giftCardResultCode"></param>
        protected virtual void SendGiftCardEmail(SOIncomeRefundInfo entity, string giftCardResultCode)
        {
            int code = 0;
            if (int.TryParse(giftCardResultCode, out code) && code == 0)
            {
                var customer = ExternalDomainBroker.GetCustomerBasicInfo(entity.CustomerSysNo.Value);

                KeyValueVariables vars = new KeyValueVariables();
                vars.Add("CustomerID", customer.CustomerID);
                vars.Add("TotalValue", entity.RefundCashAmt.Value);
                vars.Add("ExpireYear", DateTime.Now.AddYears(2).Year);
                vars.Add("ExpireMonth", DateTime.Now.Month);
                vars.Add("ExpireDay", DateTime.Now.Day);
                vars.Add("Year", DateTime.Now.Year);
                EmailHelper.SendEmailByTemplate(customer.Email, "Refund_GiftCard_Notify", vars, true, customer.FavoriteLanguageCode);
            }
            else
            {
                //记录操作日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("SOIncomeRefund_Log_GiftCardRefundFailed", entity.SOSysNo.Value)
                    , BizLogType.AuditRefund_Update
                    , entity.SysNo.Value
                    , entity.CompanyCode);

                ThrowBizException("SOIncomeRefund_Log_GiftCardRefundFailed", entity.SOSysNo.Value);
            }
        }

        /// <summary>
        /// 检查支付方式是否可以支持现金退款
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        public bool CheckPayTypeCanCashRefund(int payType)
        {
            ////是否可以支持现金退款，原来是直接写死在代码中，现在通过配置来解决。
            //string cfg = AppSettingManager.GetSetting("Invoice", "SOIncomeRefundCanCashRefundPayType");
            //if (!string.IsNullOrEmpty(cfg))
            //{
            //    String[] payTypeSysNos = cfg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    return payTypeSysNos.Contains(payType.ToString());
            //}
            //return false;
            return true;
        }

        #region RMA Domain 要用到的业务

        /// <summary>
        /// RO审核通过
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AuditForRO(int sysNo)
        {
            PreCheckForAuditForRO(sysNo);

            m_SOIncomeRefundDA.UpdateStatus(sysNo, null, RefundStatus.Audit, DateTime.Now);
        }

        /// <summary>
        /// RO审核通过预先检查
        /// </summary>
        /// <param name="sysNo"></param>
        protected virtual void PreCheckForAuditForRO(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);
            if (entity.Status != RefundStatus.WaitingRefund)
            {
                ThrowBizException("SOIncomeRefund_NotWaitRefundAuditFailed");
            }
        }

        /// <summary>
        /// PO提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public void SubmitAuditForRO(int sysNo)
        {
            PreCheckForSubmitAuditForRO(sysNo);

            m_SOIncomeRefundDA.UpdateStatus(sysNo, null, RefundStatus.Origin, null);
        }

        /// <summary>
        /// RO提交审核预先检查
        /// </summary>
        /// <param name="sysNo"></param>
        protected virtual void PreCheckForSubmitAuditForRO(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);
            if (entity.Status != RefundStatus.WaitingRefund)
            {
                ThrowBizException("SOIncomeRefund_CanNotSubmitAudit");
            }
        }

        /// <summary>
        /// RO取消提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public void CancelSubmitAuditForRO(int sysNo)
        {
            PreCheckCancelSubmitAuditForRO(sysNo);

            m_SOIncomeRefundDA.UpdateStatus(sysNo, null, RefundStatus.WaitingRefund, null);
        }

        /// <summary>
        /// RO取消提交审核预先检查
        /// </summary>
        /// <param name="sysNo"></param>
        protected virtual void PreCheckCancelSubmitAuditForRO(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);
            if (entity.Status != RefundStatus.Origin && entity.OrderType != RefundOrderType.RO)
            {
                ThrowBizException("SOIncomeRefund_CanNotCancelSubmitAudit");
            }
        }

        /// <summary>
        /// RO作废退款单
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AbandonForRO(int sysNo)
        {
            PreCheckAbandonForRO(sysNo);

            m_SOIncomeRefundDA.UpdateStatus(sysNo, null, RefundStatus.Abandon, null);
        }

        /// <summary>
        /// RO作废退款单预先检查
        /// </summary>
        /// <param name="sysNo"></param>
        protected virtual void PreCheckAbandonForRO(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);
            if (!(entity.Status == RefundStatus.WaitingRefund || entity.Status == RefundStatus.Origin))
            {
                ThrowBizException("SOIncomeRefund_NotWaitRefundAbandonFailed");
            }
        }

        /// <summary>
        /// ROBalance作废退款单
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AbandonForROBalance(int sysNo)
        {
            PreCheckAbandonForROBalance(sysNo);

            m_SOIncomeRefundDA.UpdateStatus(sysNo, null, RefundStatus.Abandon, null);
        }

        /// <summary>
        /// ROBalance作废退款单预先检查
        /// </summary>
        /// <param name="sysNo"></param>
        protected virtual void PreCheckAbandonForROBalance(int sysNo)
        {
            SOIncomeRefundInfo entity = LoadBySysNo(sysNo);
            if (entity.Status != RefundStatus.Origin)
            {
                ThrowBizException("SOIncomeRefund_CanNotAbandon");
            }
        }

        #endregion RMA Domain 要用到的业务

        #region SO Domain 要用到的业务

        /// <summary>
        /// 团购Job调用，用于创建AO单
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="note">退款备注</param>
        /// <param name="refundReason">退款原因系统编号，编号来自OverseaServiceManagement.dbo.refundReason</param>
        public virtual void CreateAOForJob(int soSysNo, RefundPayType refundPayType, string note, int? refundReason)
        {
            //0.验证
            SOBaseInfo soInfo = ExternalDomainBroker.GetSOBaseInfo(soSysNo);
            if (soInfo == null)
            {
                ThrowBizException("SOIncomeRefund_SONotFound", soSysNo);
            }
            if (soInfo.Status != SOStatus.Origin && soInfo.Status != SOStatus.WaitingOutStock)
            {   //不为Origin，WaitingOutStock 状态
                ThrowBizException("SOIncomeRefund_CreateAOForGroupJob_SOStatusInvalid", soSysNo);
            }

            var validSOIncome = ObjectFactory<SOIncomeProcessor>.Instance.GetValid(soSysNo, SOIncomeOrderType.SO);
            if (validSOIncome == null)
            {
                ThrowBizException("SOIncomeRefund_CreateAOForGroupJob_ValidSOIncomeNotFound", soSysNo);
            }

            //1.Set data
            //CRL18174:团购订单退款调整,团购订单退款需视不同的支付方式，采用不同的退款类型
            //如果团购订单的退款类型为空，则默认为“退入余额帐户”方式
            //如果退款方式为“网关直接退款"，则需要填写“银行名称”
            string bankName = refundPayType == RefundPayType.NetWorkRefund ? ExternalDomainBroker.GetPayType(soInfo.PayTypeSysNo.Value).PayTypeName : "";

            SOIncomeRefundInfo soIncomeRefund = new SOIncomeRefundInfo
            {
                OrderSysNo = soSysNo,
                SOSysNo = soSysNo,
                OrderType = RefundOrderType.AO,
                Status = RefundStatus.Origin,
                Note = note,
                RefundPayType = refundPayType,
                RefundReason = refundReason,
                BankName = bankName
            };
            soIncomeRefund.CompanyCode = soInfo.CompanyCode;
            //AO退 除了礼品卡的全部金额，包括积分、余额、现金
            decimal giftCardPayAmt = validSOIncome.GiftCardPayAmt ?? 0m;
            soIncomeRefund.RefundCashAmt = validSOIncome.OrderAmt - giftCardPayAmt;
            soIncomeRefund.RefundGiftCard = giftCardPayAmt;
            soIncomeRefund.RefundPoint = 0;

            //Negative SOIncome
            validSOIncome.CompanyCode = soInfo.CompanyCode;
            validSOIncome.OrderType = SOIncomeOrderType.AO;
            validSOIncome.OrderAmt = -validSOIncome.OrderAmt;
            validSOIncome.PointPay = -validSOIncome.PointPay;
            validSOIncome.GiftCardPayAmt = -validSOIncome.GiftCardPayAmt;
            validSOIncome.IncomeAmt = -validSOIncome.IncomeAmt;

            PreCheckForCreateAOForJob(soIncomeRefund);

            ObjectFactory<SOIncomeProcessor>.Instance.Create(validSOIncome);

            ObjectFactory<ISOIncomeRefundDA>.Instance.Create(soIncomeRefund);
        }

        /// <summary>
        /// 为团购Job创建AO单退款记录检查逻辑
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForCreateAOForJob(SOIncomeRefundInfo entity)
        {
            if (entity.SOSysNo < 1)
            {
                ThrowBizException("SOIncomeRefund_CreateAOForGroupJob_SOSysNoInvalid");
            }
            if (entity.OrderSysNo < 1)
            {
                ThrowBizException("SOIncomeRefund_CreateAOForGroupJob_OrderSysNoInvalid");
            }
            //如果单据类型是AO或多付款退款,则不允许"转积分退款"(现在已经不会再创建这种数据源了)
            if ((entity.OrderType == RefundOrderType.AO || entity.OrderType == RefundOrderType.OverPayment)
                && entity.RefundPayType == RefundPayType.TransferPointRefund)
            {
                ThrowBizException("SOIncomeRefund_CanNotTransferPoint");
            }
            CheckRequiredFields(entity);
        }

        #endregion SO Domain 要用到的业务

        #region Helper Methods

        protected void ThrowBizException(string msgKeyName, params object[] args)
        {
            string msg = GetMessageString(msgKeyName, args);
            throw new BizException(msg);
        }

        protected string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.SOIncomeRefund, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}