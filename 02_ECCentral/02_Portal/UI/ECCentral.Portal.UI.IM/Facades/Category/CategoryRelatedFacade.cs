using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryRelatedFacade
    {
        private readonly RestClient restClient;
        private const string GetCategoryRelatedUrl = "/IMService/CategoryRelated/GetCategoryRelatedByQuery";
        private const string DeleteCategoryRelatedUrl = "/IMService/CategoryRelated/DeleteCategoryRelated";
        private const string CreateCategoryRelatedUrl = "/IMService/CategoryRelated/CreateCategoryRelated";
        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public CategoryRelatedFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryRelatedFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }



        public void GetCategoryRelatedByQuery(CategoryRelatedQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryRelatedQueryFilter query = new CategoryRelatedQueryFilter();
            query = model.ConvertVM<CategoryRelatedQueryVM, CategoryRelatedQueryFilter>();
            query.PageInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetCategoryRelatedUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void DeleteCategoryRelated(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(DeleteCategoryRelatedUrl, list, callback);
        }
        public void CreateCategoryRelated(CategoryRelatedVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryRelatedInfo info = new CategoryRelatedInfo();
            info.C3SysNo1 =Convert.ToInt32( model.C3SysNo1);
            info.C3SysNo2 =Convert.ToInt32( model.C3SysNo2);
            info.CreateUserSysNo = model.CreateUserSysNo;
            info.Priority = Convert.ToInt32(model.Priority);
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            restClient.Create(CreateCategoryRelatedUrl, info, callback);
        }
    }
}
