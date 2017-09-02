//using System;
//using ECCentral.Portal.Basic.Utilities;
//using ECCentral.Portal.UI.IM.Models;
//using ECCentral.QueryFilter.Common;
//using ECCentral.QueryFilter.IM;
//using Newegg.Oversea.Silverlight.Controls;
//using Newegg.Oversea.Silverlight.ControlPanel.Core;
//using System.Collections.Generic;
//using ECCentral.BizEntity.IM;
//using ECCentral.QueryFilter;
//using ECCentral.BizEntity.Common;

//namespace ECCentral.Portal.UI.IM.Facades
//{
//    public class BrandRecommendedFacade
//    {
//         private readonly RestClient restClient;
//         private const string GetBrandRecommendedListUrl = "/IMService/BrandRecommended/GetBrandRecommendedList";
//         private const string UpdateBrandRecommendedUrl = "/IMService/BrandRecommended/UpdateBrandRecommended";
        
//        /// <summary>
//        /// IMService服务基地址
//        /// </summary>
//        protected string ServiceBaseUrl
//        {
//            get
//            {
//                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
//            }
//        }

//        public BrandRecommendedFacade()
//        {
//            restClient = new RestClient(ServiceBaseUrl);
//        }

//        public BrandRecommendedFacade(IPage page)
//        {
//            restClient = new RestClient(ServiceBaseUrl, page);

//        }
//        public void GetCategoryRelatedByQuery(BrandRecommendedQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
//        {
//            BrandRecommendedQueryFilter query = new BrandRecommendedQueryFilter() {BrandType=model.BrandType,LevelCode=model.LevelCode,LevelCodeParent=model.LevelCodeParent };
//            query.PagingInfo = new PagingInfo() { PageIndex = PageIndex, PageSize = PageSize, SortBy = SortField };
            
//            restClient.QueryDynamicData(GetBrandRecommendedListUrl, query, (obj, args) =>
//            {
//                if (args.FaultsHandle())
//                {
//                    return;
//                }
//                callback(obj, args);
//            });
//        }
//        public void UpdateBrandRecommended(BrandRecommendedVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
//        {
//            BrandRecommendedInfo info = new BrandRecommendedInfo()
//            {
//                BrandRank = model.BrandRank,
//                Sysno = model.Sysno,
//                User = new UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
//            };
//            restClient.Update(UpdateBrandRecommendedUrl, info, callback);
//        }
//    }
//}
