//using System;
//using System.Collections.Generic;
//using System.ServiceModel.Web;
//using ECCentral.BizEntity;
//using ECCentral.BizEntity.IM;
//using ECCentral.QueryFilter.IM;
//using ECCentral.Service.IM.IDataAccess;
//using ECCentral.Service.Utility;
//using ECCentral.Service.Utility.WCF;
//using ECCentral.QueryFilter;
//using ECCentral.Service.IM.AppService;
//namespace ECCentral.Service.IM.Restful
//{
//    public partial class IMService
//    {
//        [WebInvoke(UriTemplate = "/BrandRecommended/GetCategory1List", Method = "POST")]
//        [DataTableSerializeOperationBehavior]
//        public QueryResult GetCategory1List()
//        {

//            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetCategory1List();
//            return new QueryResult 
//            {
//                Data=datatable,
//                TotalCount=0
//            };
//        }

//        [WebInvoke(UriTemplate = "/BrandRecommended/GetCategory2List", Method = "POST")]
//        [DataTableSerializeOperationBehavior]
//        public QueryResult GetCategory2List()
//        {
//            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetCategory2List();
//            return new QueryResult
//            {
//                Data = datatable,
//                TotalCount = 0
//            };
//        }
//        [WebInvoke(UriTemplate = "/BrandRecommended/GetBrandRecommendedList", Method = "POST")]
//        [DataTableSerializeOperationBehavior]
//        public QueryResult GetBrandRecommendedList(BrandRecommendedQueryFilter query)
//        {
//            int totalCount;
//            var datatable = ObjectFactory<IBrandRecommendedDA>.Instance.GetBrandRecommendedList(query,out totalCount);
//            return new QueryResult
//            {
//                Data = datatable,
//                TotalCount = totalCount
//            };

//        }
//        [WebInvoke(UriTemplate = "/BrandRecommended/UpdateBrandRecommended", Method = "PUT")]
//        [DataTableSerializeOperationBehavior]
//        public void UpdateBrandRecommended(BrandRecommendedInfo info)
//        {
//            ObjectFactory<BrandRecommendedAppService>.Instance.UpdateBrandRecommended(info);
//        }
//    }
//}
