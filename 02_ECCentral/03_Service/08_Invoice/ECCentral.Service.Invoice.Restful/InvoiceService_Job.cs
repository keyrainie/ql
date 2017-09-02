using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// Job调整顾客积分预检查
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/AdjustPointPreCheck", Method = "PUT")]
        public void AdjustPointPreCheck(AdjustPointRequest request)
        {
            ObjectFactory<InvoiceJobAppService>.Instance.AdjustPointPreCheck(request);
        }

        /// <summary>
        /// Job调整顾客积分
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/AdjustPoint", Method = "PUT")]
        public void AdjustPoint(AdjustPointRequest request)
        {
            ObjectFactory<InvoiceJobAppService>.Instance.AdjustPoint(request);
        }

        /// <summary>
        /// 批量创建跟踪单(Job调用)
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/BatchCreateTracking", Method = "PUT")]
        public string BatchCreateTracking(List<ECCentral.BizEntity.Invoice.TrackingInfo> trackingInfoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchCreateTrackingInfo(trackingInfoList);
        }

        /// <summary>
        /// 同步交易对账单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Job/SyncTradeBillTrade/{date}", Method = "GET")]
        public bool SyncTradeBillTrade(string date)
        {
            return ObjectFactory<InvoiceJobAppService>.Instance.SyncTradeBill("1", date);
        }

        /// <summary>
        /// 查询财付通退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/GetSysNoListByRefund", Method = "GET")]
        public List<int> GetSysNoListByRefund()
        {
            return ObjectFactory<SOIncomeAppService>.Instance.GetSysNoListByRefund();
        }
        [WebInvoke(UriTemplate = "/Job/QueryRefund/{sysNo}", Method = "GET")]
        public void QueryRefund(string sysNo)
        {
            ObjectFactory<SOIncomeAppService>.Instance.QueryRefund(int.Parse(sysNo));
        }
    }
}