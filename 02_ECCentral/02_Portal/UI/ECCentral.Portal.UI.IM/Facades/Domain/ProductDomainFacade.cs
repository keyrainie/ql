using System;
using System.Linq;
using System.Collections.Generic;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.Restful.RequestMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductDomainFacade
    {
        private readonly RestClient restClient;

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

        public ProductDomainFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductDomainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetUserListName(string userSysNoList, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductDomain/GetUserListName/{0}/{1}", CPApplication.Current.CompanyCode, userSysNoList);
            restClient.Query<string>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void QueryProductDomain(ProductDomainQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductDomainFilter filter;
            filter = model.ConvertVM<ProductDomainQueryVM, ProductDomainFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/ProductDomain/QueryProductDomain";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
               
                callback(obj, args);
            });
        }

        public void LoadDomainForListing(EventHandler<RestClientEventArgs<List<ProductDomain>>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductDomain/List/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<ProductDomain>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void LoadDomainCategorys(int productDomainSysNo, EventHandler<RestClientEventArgs<List<ProductDepartmentCategory>>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductDomain/LoadDomainCategorys/{0}", productDomainSysNo);
            restClient.Query<List<ProductDepartmentCategory>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CreateDomain(ProductDomainVM vm, EventHandler<RestClientEventArgs<ProductDomain>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/CreateDomain";
            var msg = vm.ConvertVM<ProductDomainVM, ProductDomain>();
            msg.ProductDomainName.Content = vm.ProductDomainName;
            var selected = vm.DepartmentMerchandiserListForUI.Where(p => p.IsChecked).ToList();
            msg.DepartmentMerchandiserSysNoList = selected.Select(p => p.SysNo).ToList();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<ProductDomain>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void UpdateDomain(ProductDomainVM vm, EventHandler<RestClientEventArgs<ProductDomain>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/UpdateDomain";
            var msg = vm.ConvertVM<ProductDomainVM, ProductDomain>();
            msg.ProductDomainName.Content = vm.ProductDomainName;
            var selected = vm.DepartmentMerchandiserListForUI.Where(p => p.IsChecked).ToList();
            msg.DepartmentMerchandiserSysNoList = selected.Select(p => p.SysNo).ToList();

            restClient.Update<ProductDomain>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void DeleteDomain(int domainSysNo, int? departmentCategorySysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/DeleteDomain";
            var req = new DeleteProductDomainReq
            {
                ProductDomainSysNo = domainSysNo,
                DepartmentCategorySysNo = departmentCategorySysNo
            };
            restClient.Delete(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CreateDomainCategory(ProductDepartmentCategoryVM vm, EventHandler<RestClientEventArgs<ProductDepartmentCategory>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/CreateDepartmentCategory";
            var msg = vm.ConvertVM<ProductDepartmentCategoryVM, ProductDepartmentCategory>();            
            restClient.Create<ProductDepartmentCategory>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void UpdateDomainCategory(ProductDepartmentCategoryVM vm, EventHandler<RestClientEventArgs<ProductDepartmentCategory>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/UpdateDepartmentCategory";
            var msg = vm.ConvertVM<ProductDepartmentCategoryVM, ProductDepartmentCategory>();
            restClient.Update<ProductDepartmentCategory>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void DeleteDomainCategory(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/DeleteDepartmentCategory";            
            restClient.Delete(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void BatchUpdatePM(BatchUpdatePMReq req, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductDomain/BatchUpdatePM";            
            restClient.Update<object>(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }        
    }
}
