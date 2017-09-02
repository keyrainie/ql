using System.ServiceModel.Web;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.Service.SO.IDataAccess;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 查询运单 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/QueryPackageCoverSearch", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPackageCoverSearch(SOPackageCoverSearchFilter filter)
        {
            return QueryList<SOPackageCoverSearchFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.PackageCoverSearchQuery);
        }

        /// <summary>
        /// 查询出货单
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryOutStock", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOutStock(SOOutStockQueryFilter filter)
        {
            return QueryList<SOOutStockQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.OutStockQuery);
        }


        [WebInvoke(UriTemplate = "/SODeliveryDiff/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryDiffSODelivery(SODeliveryDiffFilter filter)
        {
            return QueryList<SODeliveryDiffFilter>(filter, ObjectFactory<ISOLogisticDA>.Instance.QueryDiffSODelivery);
        }


        [WebInvoke(UriTemplate = "/DeliveryExp/Mark", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public void MarkDeliveryExp(DeliveryExpMarkEntity info)
        {
            ObjectFactory<SOLogisticsAppService>.Instance.MarkDeliveryExp(info);
        }



    }
}

