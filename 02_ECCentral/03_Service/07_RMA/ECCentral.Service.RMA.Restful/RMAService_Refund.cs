using System;
using System.Collections.Generic;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.RMA.Restful
{    
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/Refund/Query", Method = "POST")]        
        public QueryResult QueryRefund(RefundQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IRefundQueryDA>.Instance.QueryRefund(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Refund/GetWaitingRegisters", Method = "POST")]        
        public QueryResult GetWaitingRegisters(int? soSysNo)
        {
            var dataTable = ObjectFactory<IRefundQueryDA>.Instance.GetWaitingRegisters(soSysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = dataTable.Rows.Count
            };
        }

        [WebInvoke(UriTemplate = "/Refund/GetWaitingSOForRefund", Method = "GET")]
        public List<int> GetWaitingSOForRefund()
        {
            return ObjectFactory<RefundAppService>.Instance.GetWaitingSOForRefund();
        }

        [WebInvoke(UriTemplate = "/Refund/Create", Method = "POST")]
        public RefundInfo CreateRefund(RefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.Create(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/Update", Method = "PUT")]
        public void UpdateRefund(RefundInfo refund)
        {
            ObjectFactory<RefundAppService>.Instance.Update(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/Calculate", Method = "PUT")]
        public RefundInfo Calculate(RefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.Calculate(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/SubmitAudit", Method = "PUT")]
        public RefundInfo SubmitAuditRefund(RefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.SubmitAudit(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/CancelSubmitAudit", Method = "PUT")]
        public RefundInfo CancelSubmitAuditRefund(RefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.CancelSubmitAudit(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/Abandon", Method = "PUT")]
        public RefundInfo AbandonRefund(int sysNo)
        {
            return ObjectFactory<RefundAppService>.Instance.Abandon(sysNo);
        }

        [WebInvoke(UriTemplate = "/Refund/Refund", Method = "PUT")]
        public RefundInfo RefundRMARefund(int sysNo)
        {
            return ObjectFactory<RefundAppService>.Instance.Refund(sysNo);
        }

        [WebInvoke(UriTemplate = "/Refund/CancelRefund", Method = "PUT")]
        public RefundInfo CancelRMARefund(int sysNo)
        {
            return ObjectFactory<RefundAppService>.Instance.CancelRefund(sysNo);
        }

        [WebInvoke(UriTemplate = "/Refund/UpdateFinanceNote", Method = "PUT")]
        public RefundInfo UpdateFinanceNote(RefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.UpdateFinanceNote(refund);
        }

        [WebInvoke(UriTemplate = "/Refund/Load/{sysNo}", Method = "GET")]
        public RefundDetailInfoRsp LoadRefundBySysNo(string sysNo)
        {
            string customerName;
            CustomerContactInfo contactInfo;
            PromotionCode_Customer_Log couponCodeLog;            

            int no = 0;
            if (int.TryParse(sysNo, out no))
            {
                RefundInfo refund = ObjectFactory<RefundAppService>.Instance.LoadBySysNo(no, out customerName, out contactInfo, out couponCodeLog);
                RefundDetailInfoRsp result = new RefundDetailInfoRsp
                {
                    RefundInfo = refund,
                    CustomerName = customerName,
                    CustomerContact = contactInfo,
                    CouponCodeLog = couponCodeLog
                };
                return result;
            }
            throw new ArgumentException("Invalid sysNo");
        }

        [WebInvoke(UriTemplate = "/Refund/GetRefundReaons", Method = "GET")]
        public List<CodeNamePair> GetRefundReaons()
        {            
            return ObjectFactory<RefundAppService>.Instance.GetRefundReasons();
        }

        [WebInvoke(UriTemplate = "/Refund/GetShipFee", Method = "POST")]
        public ShippingFeeRsp GetShipFee(RefundInfo refund)
        {
            decimal totalAmt, premiumAmt, shippingCharge, payPrice, historyRefund;
            ObjectFactory<RefundAppService>.Instance.GetShipFee(refund, out totalAmt, out premiumAmt, out shippingCharge, out payPrice, out historyRefund);
            return new ShippingFeeRsp
            {
                TotalAmt = totalAmt,
                HistoryRefund = historyRefund,
                PayPrice = payPrice,
                PremiumAmt = premiumAmt,
                ShippingCharge = shippingCharge
            };            
        }


        [WebInvoke(UriTemplate = "/Refund/CreateRefundForRMAAuto", Method = "POST")]
        public List<RefundInfo> CreateRefundForRMAAuto(AutoRefundInfo refund)
        {
            return ObjectFactory<RefundAppService>.Instance.CreateRefundForRMAAuto(refund);
        }   

    }
}