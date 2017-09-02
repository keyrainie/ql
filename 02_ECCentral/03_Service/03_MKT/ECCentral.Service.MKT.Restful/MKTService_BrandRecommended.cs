using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using System.Data;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.QueryFilter;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/BrandRecommended/GetCategory1List", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCategory1List()
        {

            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetCategory1List();
            return new QueryResult
            {
                Data = datatable,
                TotalCount = 0
            };
        }

        [WebInvoke(UriTemplate = "/BrandRecommended/GetCategory2List", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCategory2List()
        {
            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetCategory2List();
            return new QueryResult
            {
                Data = datatable,
                TotalCount = 0
            };
        }
        [WebInvoke(UriTemplate = "/BrandRecommended/GetBrandRecommendedList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetBrandRecommendedList(BrandRecommendedQueryFilter query)
        {
            int totalCount;
            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetBrandRecommendedList(query, out totalCount);
            return new QueryResult
            {
                Data = datatable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/BrandRecommended/UpdateBrandRecommended", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateBrandRecommended(BrandRecommendedInfo info)
        {
            ObjectFactory<BrandRecommendedAppService>.Instance.UpdateBrandRecommended(info);
        }


        [WebInvoke(UriTemplate = "/BrandRecommended/CreateBrandRecommended", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateBrandRecommended(BrandRecommendedInfo info)
        {
            ObjectFactory<BrandRecommendedAppService>.Instance.CreateBrandRecommended(info);
        }
    }
}
