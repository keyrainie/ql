using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.BizEntity.Inventory;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 查询成本变价单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/QueryCostChangeList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCostChangeList(CostChangeQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<ICostChangeQueryDA>.Instance.QueryCostChangeList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 加载成本变价单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/LoadCostChangeInfo/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public CostChangeInfo LoadCostChangeInfo(string sysNo)
        {
            return ObjectFactory<CostChangeAppService>.Instance.LoadCostChangeInfo(Convert.ToInt32(sysNo));
        }

        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/CreateCostChange", Method = "PUT")]
        public CostChangeInfo CreateCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.CreateCostChange(info);
        }

        /// <summary>
        ///  更新成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/UpdateCostChange", Method = "PUT")]
        public CostChangeInfo UpdateCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.UpdateCostChange(info);
        }

        /// <summary>
        ///  提交审核成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/SubmitAuditCostChange", Method = "PUT")]
        public CostChangeInfo SubmitAuditCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.SubmitAuditCostChange(info);
        }

        /// <summary>
        ///   撤销提交成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/CancelSubmitAuditPOCostChange", Method = "PUT")]
        public CostChangeInfo CancelSubmitAuditPOCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.CancelSubmitAuditPOCostChange(info);
        }

        /// <summary>
        ///  拒绝并退回成本变价单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/RefuseCostChange", Method = "PUT")]
        public CostChangeInfo RefuseCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.RefuseCostChange(info);
        }

        /// <summary>
        ///  作废成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/AbandonCostChange", Method = "PUT")]
        public CostChangeInfo AbandonCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.AbandonCostChange(info);
        }

        /// <summary>
        ///  审核通过成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CostChange/AuditCostChange", Method = "PUT")]
        public CostChangeInfo AuditCostChange(CostChangeInfo info)
        {
            return ObjectFactory<CostChangeAppService>.Instance.AuditCostChange(info);
        }
    }
}
