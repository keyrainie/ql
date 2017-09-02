using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.SZPointAlliance;

namespace ECCentral.Service.Invoice.BizProcessor
{
    /// <summary>
    /// 统一管理需要调用外部Domain的接口方法
    /// </summary>
    internal static class ExternaSystemBroker
    {
        internal static string EIMSPayPayItem(int payItemSysNo, int orderSysNo, decimal receiveAmount, string companyCode, string userID)
        {
            //TODO:调用EIMS系统接口，支付付款单。
            var msg = new EIMSPayMessage()
            {
                AcctInvoiceNumber = payItemSysNo,
                InvoiceNumber = orderSysNo,
                InvoiceStatus = (int)InvoiceStatus.Audited,
                PayStatus = (int)PayableStatus.FullPay,
                PostUser = ServiceContext.Current.UserSysNo.ToString(),
                ReceiveAmount = receiveAmount,
                ReceiveDate = DateTime.Now,
                CompanyCode = companyCode,
                UserID = userID
            };
            EventPublisher.Publish<EIMSPayMessage>(msg);
            return "";
        }

        internal static string EIMSCancelPayItem(int payItemSysNo, int orderSysNo, string companyCode, string userID)
        {
            //TODO:调用EIMS系统服务取消支付付款单
            var msg = new EIMSCancelPayMessage
            {
                AcctinvoiceNumber = payItemSysNo,
                EIMSInvoiceNumber = orderSysNo,
                InvoiceStatus = (int)InvoiceStatus.Origin,
                PayStatus = (int)PayableStatus.Abandon,
                CompanyCode = companyCode,
                UserID = userID

            };
            EventPublisher.Publish<EIMSCancelPayMessage>(msg);
            return "";
        }

        internal static int RefundPrepayCard(decimal refundAmount, int soSysNo, string tNumber, string refundKey)
        {
            //TODO:调用神州退预付卡接口
            var msg = new SZPointAllianceRequestMessage()
            {
                RefundAmount = refundAmount,
                RefundDescription = string.Empty,
                RefundKey = refundKey,
                RefundType = PointAllianceRefundType.PrepaidCard,
                SOSysNo = soSysNo,
                TNumber = tNumber
            };
            EventPublisher.Publish<SZPointAllianceRequestMessage>(msg);
            return 0;
        }
    }
}