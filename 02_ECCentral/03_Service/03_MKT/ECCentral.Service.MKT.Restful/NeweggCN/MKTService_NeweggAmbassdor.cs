using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private NeweggAmbassadorAppService _neweggAmbassadorAppService = ObjectFactory<NeweggAmbassadorAppService>.Instance;


        /// <summary>
        /// 获取大使信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/QueryAmbassadorBasicInfo", Method = "POST")]
        public virtual QueryResult QueryAmbassadorBasicInfo(NeweggAmbassadorQueryFilter filter)
        {
            int totalCount = 0;
            var ds = ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryAmbassadorBasicInfo(filter, out totalCount);
            var dtAmbassadorNews = ds.Tables[0];

            QueryResult queryResult = new QueryResult();
            queryResult.Data = dtAmbassadorNews;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        /// <summary>
        /// 更新泰隆优选大使的状态信息，即激活或取消泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/MaintainNeweggAmbassadorStatus", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual void MaintainNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            _neweggAmbassadorAppService.MaintainNeweggAmbassadorStatus(batchInfo);
        }

        /// <summary>
        /// 尝试更新泰隆优选大使的状态，返回需要确认的泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/TryUpdateAmbassadorStatus", Method = "PUT")]
        public virtual NeweggAmbassadorBatchInfo TryUpdateAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            return _neweggAmbassadorAppService.TryUpdateAmbassadorStatus(batchInfo);
        }

        /// <summary>
        /// 取消申请。
        /// </summary>
        /// <param name="batchInfo"></param>、
        [WebInvoke(UriTemplate = "/NeweggAmbassador/CancelRequestNeweggAmbassadorStatus", Method = "PUT")]
        public virtual void CancelRequestNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            _neweggAmbassadorAppService.CancelRequestNeweggAmbassadorStatus(batchInfo);
        }

        /// <summary>
        /// 获取代购订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/QueryAmbassadorPurchaseOrderInfo", Method = "POST")]
        public virtual QueryResultList QueryAmbassadorPurchaseOrderInfo(NeweggAmbassadorOrderQueryFilter filter)
        {
            return GetQueryResult(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryPurchaseOrderInfo);
        }

        /// <summary>
        /// 获取推荐订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/QueryAmbassadorRecommendOrderInfo", Method = "POST")]
        public virtual QueryResultList QueryAmbassadorRecommendOrderInfo(NeweggAmbassadorOrderQueryFilter filter)
        {
            return GetQueryResult(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryRecommendOrderInfo);
        }

        /// <summary>
        /// 获取积分发放信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/QueryPointInfo", Method = "POST")]
        public virtual QueryResultList QueryPointInfo(NeweggAmbassadorOrderQueryFilter filter)
        {

            return GetQueryResult(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryPointInfo);
        }

        /// <summary>
        /// 导出代购订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/ExportAmbassadorPurchaseOrderInfo", Method = "POST")]
        public virtual QueryResult ExportAmbassadorPurchaseOrderInfo(NeweggAmbassadorOrderQueryFilter filter)
        {
            return Export(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryPurchaseOrderInfo);
        }

        /// <summary>
        /// 导出推荐订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/ExportAmbassadorRecommendOrderInfo", Method = "POST")]
        public virtual QueryResult ExportAmbassadorRecommendOrderInfo(NeweggAmbassadorOrderQueryFilter filter)
        {
            return Export(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryRecommendOrderInfo);
        }

        /// <summary>
        /// 导出积分发放信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NeweggAmbassador/ExportPointInfo", Method = "POST")]
        public virtual QueryResult ExportPointInfo(NeweggAmbassadorOrderQueryFilter filter)
        {

            return Export(filter, ObjectFactory<INeweggAmbassadorQueryDA>.Instance.QueryPointInfo);
        }

        private delegate DataSet QueryResultHandler(NeweggAmbassadorOrderQueryFilter filter, out int totalCount);

        private QueryResultList GetQueryResult(NeweggAmbassadorOrderQueryFilter filter, QueryResultHandler fun)
        {
            int totalCount;
            DataSet ds = fun(filter, out totalCount);
            DataTable firstTableInfo = ds.Tables[0];//第一个DataTable
            DataTable secondTableInfo = ds.Tables[1];//第二个DataTable
            QueryResult listInfo = new QueryResult();
            listInfo.Data = firstTableInfo;
            listInfo.TotalCount = totalCount;

            QueryResult summaryResult = new QueryResult();
            summaryResult.Data = secondTableInfo;

            QueryResultList result = new QueryResultList();
            result.Add(listInfo);
            result.Add(summaryResult);
            return result;
        }

        private QueryResult Export(NeweggAmbassadorOrderQueryFilter filter, QueryResultHandler fun)
        {
            int totalCount;
            DataSet ds = fun(filter, out totalCount);
            DataTable firstTableInfo = ds.Tables[0];//第一个DataTable
            QueryResult listInfo = new QueryResult();
            listInfo.Data = firstTableInfo;
            listInfo.TotalCount = totalCount;

            return listInfo;
        }


    }
}
