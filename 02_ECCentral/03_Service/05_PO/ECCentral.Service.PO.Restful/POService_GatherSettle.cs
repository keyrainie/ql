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
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.PO.Restful.ResponseMsg;
using System.Data;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 代收结算单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GatherSettlement/QueryGatherSettlementList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryGatherSettlementList(GatherSettleQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IGatherSettleQueryDA>.Instance.Query(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/GatherSettlement/QueryGatherSettlementItemList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public GatherSettlementItemsQueryRsp QueryGatherSettlementItemList(GatherSettleItemsQueryFilter queryFilter)
        {
            int totalCount = 0;
            GatherSettlementItemsQueryRsp rep = new GatherSettlementItemsQueryRsp()
            {
                ResultList = ObjectFactory<IGatherSettlementDA>.Instance.QueryConsignSettlementProductList(queryFilter, out totalCount)
            };
            rep.TotalCount = totalCount;
            return rep;
        }

        /// <summary>
        /// 加载代收结算单信息
        /// </summary>
        /// <param name="gatherSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GatherSettlement/LoadGatherSettlement/{gatherSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public GatherSettlementInfo LoadGatherSettlementInfo(string gatherSysNo)
        {
            return ObjectFactory<GatherSettlementAppService>.Instance.LoadGatherSettlementInfo(Convert.ToInt32(gatherSysNo));
        }

        [WebInvoke(UriTemplate = "/GatherSettlement/CreateGatherSettlement", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public GatherSettlementInfo CreateGatherSettlementInfo(GatherSettlementInfo info)
        {
            return ObjectFactory<GatherSettlementAppService>.Instance.CreateGatherSettlementInfo(info);
        }

        /// <summary>
        /// 更新代收结算单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GatherSettlement/UpdateGatherSettlement", Method = "PUT")]
        public void UpdateGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.UpdateGatherSettlementInfo(info);
        }

        /// <summary>
        /// 作废代收结算单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/GatherSettlement/AbandonGatherSettlement", Method = "PUT")]
        public void AbandonGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.AbandonGatherSettlementInfo(info);
        }

        /// <summary>
        /// 审核代收结算单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/GatherSettlement/AuditGatherSettlement", Method = "PUT")]
        public void AuditGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.AuditGatherSettlementInfo(info);
        }

        /// <summary>
        /// 取消审核代收结算单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/GatherSettlement/CancelAuditGatherSettlement", Method = "PUT")]
        public void CancelAuditGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.CancelAuditGatherSettlementInfo(info);
        }


        /// <summary>
        /// 结算代收结算单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/GatherSettlement/SettleGatherSettlement", Method = "PUT")]
        public void SettleGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.SettleGatherSettlementInfo(info);
        }

        /// <summary>
        /// 取消结算代收结算单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/GatherSettlement/CancelSettleGatherSettlement", Method = "PUT")]
        public void CancelSettleGatherSettlementInfo(GatherSettlementInfo info)
        {
            ObjectFactory<GatherSettlementAppService>.Instance.CancelSettleGatherSettlementInfo(info);
        }

        /// <summary>
        /// 获取PO_Settle列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GatherSettlement/QuerySettleList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySettleList(SettleQueryFilter request)
        {
            int totalCount = 0;
            DataTable dt = ObjectFactory<IGatherSettleQueryDA>.Instance.QuerySettleList(request, out totalCount);
            dt = ObjectFactory<ConsignSettlementAppService>.Instance.GetSettleList(dt);

            QueryResult returnResult = new QueryResult()
            {
                Data = dt
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }
    }
}
