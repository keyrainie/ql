using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Invoice.Refund;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(RefundPrepayCardProcessor))]
    public class RefundPrepayCardProcessor
    {
        private ISOIncomeRefundDA m_SOIncomeRefundDA = ObjectFactory<ISOIncomeRefundDA>.Instance;
        private INetPayDA m_NetPayDA = ObjectFactory<INetPayDA>.Instance;
        private ISOIncomeDA m_SOIncomeDA = ObjectFactory<ISOIncomeDA>.Instance;
        private IRefundPointDA m_RefundPointDA = ObjectFactory<IRefundPointDA>.Instance;

        public virtual int RefundPrepayCard(RefundPrepayCardInfo info)
        {
            SOIncomeRefundInfo entity = m_SOIncomeRefundDA.GetSOIncomeRefundByID(info.SOIncomeBankInfoSysNo.Value, info.CompanyCode);
            string tNumber = string.Empty;
            decimal refundAmt = 0M, payAmt = 0M;
            int soSysNo = entity.SOSysNo.Value;
            string message = string.Empty;
            int result = 0;

            #region 业务验证
            if (entity == null)
            {
                ThrowBizException("SOIncomeRefund_NoRefundAudit", info.SOIncomeBankInfoSysNo.Value);
            }
            if (entity.Status != RefundStatus.Audit)
            {
                ThrowBizException("SOIncomeRefund_RefundStatusError", entity.OrderSysNo);
            }
            NetPayInfo netPay = null;
            SOBaseInfo so = ExternalDomainBroker.GetSOBaseInfo(entity.SOSysNo.Value);
            //如果是子单，则需要查询母单对应的NetPay信息
            if (so.SOSplitMaster.HasValue)
            {
                soSysNo = so.SOSplitMaster.Value;
                netPay = m_NetPayDA.GetValidBySOSysNo(soSysNo);
                if (netPay == null)
                {
                    ThrowBizException("SOIncomeRefund_NoMasterNetPay", entity.SOSysNo, soSysNo);
                }
                if (string.IsNullOrEmpty(netPay.ExternalKey))
                {
                    ThrowBizException("SOIncomeRefund_NoMasterExternalKey", entity.SOSysNo, soSysNo);
                }
                if (netPay.ExternalKey.Trim().Length == 0)
                {
                    ThrowBizException("SOIncomeRefund_MasterExternalKeyLengthError", entity.SOSysNo, soSysNo);
                }

            }

            //获取子单的NetPay信息
            netPay = m_NetPayDA.GetValidBySOSysNo(entity.SOSysNo.Value);
            if (netPay == null)
            {
                ThrowBizException("SOIncomeRefund_NoNetPay", entity.SOSysNo.Value);
            }
            //如果不是子单，则需要验证支付流水号
            if (!so.SOSplitMaster.HasValue)
            {
                if (string.IsNullOrEmpty(netPay.ExternalKey))
                {
                    ThrowBizException("SOIncomeRefund_NoExternalKey", entity.SOSysNo.Value);
                }
                if (netPay.ExternalKey.Trim().Length == 0)
                {
                    ThrowBizException("SOIncomeRefund_ExternalKeyLengthError", entity.SOSysNo.Value);
                }
            }

            tNumber = netPay.ExternalKey.Trim();

            List<SOIncomeInfo> list = m_SOIncomeDA.GetListByCriteria(null, entity.OrderSysNo, (SOIncomeOrderType)entity.OrderType, new List<SOIncomeStatus>());
            if (list == null || list.Count == 0)
            {
                ThrowBizException("SOIncomeRefund_NoIncomeInfo", entity.OrderSysNo);
            }

            SOIncomeInfo soIncome = list.Find(e => e.Status != SOIncomeStatus.Abandon);
            if (soIncome == null)
            {
                ThrowBizException("SOIncomeRefund_NoIncomeInfo", entity.OrderSysNo);
            }
            if (soIncome.Status == SOIncomeStatus.Confirmed)
            {
                ThrowBizException("SOIncomeRefund_HasConfirmed", entity.OrderSysNo);
            }
            #endregion

            refundAmt = entity.HaveAutoRMA.Value ? soIncome.IncomeAmt.Value : entity.RefundCashAmt.Value;
            payAmt = netPay.PayAmount.Value;

            //如果存在多笔退款，则payamt应该取最后一次退款时的payamt
            if (m_SOIncomeRefundDA.GetRefundOrder(entity.SOSysNo.Value, entity.CompanyCode) > 1)
            {
                payAmt = m_SOIncomeRefundDA.GetPayAmountBeHis(entity.SOSysNo.Value, entity.CompanyCode);
            }

            payAmt = payAmt - refundAmt;

            #region 写请求日志
            RefundPointInfo log = new RefundPointInfo
            {
                RefundStatus = RefundPointStatus.Origin,
                OrderType = entity.OrderType,
                ReferenceID = string.Format("{0}.{1}", entity.SysNo.Value, soIncome.SysNo.Value),
                PayAmt = payAmt,
                InUser = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo).UserDisplayName,
                EditDate = DateTime.Now,
                CurrencySysNo = 1,
                SOSysNo = entity.SOSysNo,
                OrderAmt = so.SOTotalAmount,
                ExternalKey = netPay.ExternalKey,
                CompanyCode = entity.CompanyCode,
                PayTypeSysNo = so.PayTypeSysNo.Value,
                CreateUserSysNo = info.UserSysNo,
                OrderSysNo = entity.OrderSysNo
            };

            int logSysNo = m_RefundPointDA.Insert(log);
            log.SysNo = logSysNo;
            log.EditDate = DateTime.Now;
            log.EditUser = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo).UserDisplayName;
            #endregion

            #region 请求ThirdPartService，代理访问神州运通退款服务

            try
            {
                //退款状态：1.退款成功 2.退款失败（会直接抛出异常，不作特殊处理）3.神州运通需要进行人工处理
                result = ExternalDomainBroker.RefundPrepayCard(refundAmt, soSysNo, tNumber, log.ReferenceID);
                //退款成功
                if (result == 1)
                {
                    log.RefundStatus = RefundPointStatus.Success;
                    log.ResponseContent = string.Empty;
                }
                //神州运运通需要进行人工处理，处理完成后调用Newegg的OPenAPI进行状态回写
                else if (result == 3)
                {
                    log.RefundStatus = RefundPointStatus.Processing;
                    message = log.ResponseContent = "神州运通需要进行人工处理";
                }

                m_RefundPointDA.Update(log);

                if (result == 1)
                {
                    //更新收款单
                    m_RefundPointDA.UpdateSOIncome(log);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                ExceptionHelper.HandleException(ex);
                log.RefundStatus = RefundPointStatus.Failure;
                log.ResponseContent = string.Format("{0}|{1}", ex.Message, ex.StackTrace);
                //string errorInfo = string.Format("订单{0}远程异常:{1}", entity.OrderSysNo, ex.Message);
                string errorInfo = GetMessageString("SOIncomeRefund_OrderRemoteError", entity.OrderSysNo, ex.Message);
                throw new BizException(errorInfo);
            }

            #endregion

            return result;
        }


        #region Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.SOIncomeRefund, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}
