using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ECDynamicCategoryFacade
    {
         private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public ECDynamicCategoryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }       

        public void Query(ECCategoryQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<ECCategoryQueryVM, ECCategoryQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/ECCategory/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void Create(ECDynamicCategoryVM vm, EventHandler<RestClientEventArgs<ECDynamicCategory>> callback)
        {
            var entity = vm.ConvertVM<ECDynamicCategoryVM, ECDynamicCategory>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;            
            string relativeUrl = "/MKTService/ECDynamicCategory/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(ECDynamicCategoryVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<ECDynamicCategoryVM, ECDynamicCategory>();
            entity.Status = vm.IsActive ? DynamicCategoryStatus.Active : DynamicCategoryStatus.Deactive;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/ECDynamicCategory/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<ECDynamicCategory>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/Load/" + sysNo.ToString();
            restClient.Query<ECDynamicCategory>(relativeUrl, callback);
        }

        public void Delete(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECDynamicCategory/Delete";
            restClient.Delete(relativeUrl, sysNo, callback);
        }                        

        public void LoadTree(ECDynamicCategoryQueryVM vm, EventHandler<RestClientEventArgs<ECDynamicCategory>> callback)
        {
            ECDynamicCategoryQueryFilter filter = new ECDynamicCategoryQueryFilter();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.CategoryType = vm.CategoryType;
            filter.Status = vm.Status;
            string relativeUrl = "/MKTService/ECDynamicCategory/GetCategoryTree";
            restClient.Query<ECDynamicCategory>(relativeUrl, filter, callback);
        }

        public void QueryProductMapping(int categorySysNo,int pageIndex,int pageSize,string sortBy, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ECDynamicCategoryMappingQueryFilter filter = new ECDynamicCategoryMappingQueryFilter
            {
                CompanyCode = CPApplication.Current.CompanyCode,
                DynamicCategorySysNo = categorySysNo,
                PagingInfo = new PagingInfo { PageIndex = pageIndex, PageSize = pageSize, SortBy = sortBy }
            };
            string relativeUrl = "/MKTService/ECDynamicCategory/QueryMapping";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void CreateMapping(int categorySysNo, List<int> productSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECDynamicCategory/InsertCategoryProductMapping";
            ECDynamicCategoryMappingReq req = new ECDynamicCategoryMappingReq
            {
                DynamicCategorySysNo = categorySysNo,
                ProductSysNoList = productSysNoList
            };
            restClient.Create(relativeUrl, req, callback);
        }

        public void DeleteMapping(int categorySysNo, List<int> productSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECDynamicCategory/DeleteCategoryProductMapping";
            ECDynamicCategoryMappingReq req = new ECDynamicCategoryMappingReq
            {
                DynamicCategorySysNo = categorySysNo,
                ProductSysNoList = productSysNoList
            };
            restClient.Delete(relativeUrl, req, callback);
        }
    }
}
