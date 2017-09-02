using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {

        /// <summary>
        /// 查询供应商退款单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/QueryVendorRefundList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorRMARefundList(VendorRMARefundQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IVendorRefundQueryDA>.Instance.QueryRMARefundList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 加载供应商退款单信息
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/LoadVendorRefundInfo/{refundSysNo}", Method = "GET")]
        public VendorRefundInfo LoadVendorRefundInfo(string refundSysNo)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.LoadVendorRefundInfo(Convert.ToInt32(refundSysNo));
        }

        /// <summary>
        /// 审核通过供应商退款单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/ApproveVendorRefundInfo", Method = "PUT")]
        public VendorRefundInfo ApproveVendorRefundInfo(VendorRefundInfo info)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.ApproveVendorRefundInfo(info);
        }

        /// <summary>
        /// 审核拒绝供应商退款单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/RejectVendorRefundInfo", Method = "PUT")]
        public VendorRefundInfo RejectVendorRefundInfo(VendorRefundInfo info)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.RejectVendorRefundInfo(info);
        }

        /// <summary>
        /// 更新供应商退款单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/UpdateVendorRefundInfo", Method = "PUT")]
        public VendorRefundInfo UpdateVendorRefundInfo(VendorRefundInfo info)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.UpdateVendorRefundInfo(info);
        }
        [WebInvoke(UriTemplate = "/VendorRefund/GetVendorPayBalanceByVendorSysNo", Method = "POST")]
        public decimal GetPayBalanceByVendorSysNo(string vendorSysNo)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.GetVendorPayBalanceByVendorSysNo(Convert.ToInt32(vendorSysNo));
        }

        /// <summary>
        /// 提交PMCC审核
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorRefund/SubmitToPMCC", Method = "PUT")]
        public VendorRefundInfo SubmitToPMCC(VendorRefundInfo info)
        {
            return ObjectFactory<VendorRefundAppService>.Instance.SubmitToPMCC(info);
        }


    }
}
