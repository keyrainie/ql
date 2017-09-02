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
    public class ProductLineFacade
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

        public ProductLineFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductLineFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        
        public void QueryProductLine(ProductLineQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductLineFilter filter;
            filter = model.ConvertVM<ProductLineQueryVM, ProductLineFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/ProductLine/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
               
                callback(obj, args);
            });
        }

        public void Load(EventHandler<RestClientEventArgs<ProductLineInfo>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductLine/Load", CPApplication.Current.CompanyCode);
            restClient.Query<ProductLineInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Create(ProductLineVM vm, EventHandler<RestClientEventArgs<ProductLineInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductLine/Create";
            var msg = vm.ConvertVM<ProductLineVM, ProductLineInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<ProductLineInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Update(ProductLineVM vm, EventHandler<RestClientEventArgs<ProductLineInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductLine/Update";
            var msg = vm.ConvertVM<ProductLineVM, ProductLineInfo>();
            restClient.Update<ProductLineInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Delete(int sysno, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductLine/Delete";
            restClient.Delete(relativeUrl, sysno, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void HasRightByPMUser(int pmusersysno,int c3sysno,int brandsysno, EventHandler<RestClientEventArgs<bool>> callback) 
        {
            string relativeUrl = "/IMService/ProductLine/HasRightByPMUser";
            ProductLineInfo entity = new ProductLineInfo();
            entity.C3SysNo = c3sysno;
            entity.BrandSysNo = brandsysno;
            entity.PMUserSysNo = pmusersysno;

            restClient.Query<bool>(relativeUrl, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void ExportExcelFile(ProductLineQueryVM model, ColumnSet[] columns)
        {
            string relativeUrl = "/IMService/ProductLine/Query";
            ProductLineFilter filter;
            filter = model.ConvertVM<ProductLineQueryVM, ProductLineFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        public void BatchUpdate(BatchUpdatePMVM view, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductLine/BatchUpdate";

            restClient.Update(relativeUrl, view, (obj, args) =>
            {
                callback(obj, args);
            });
        }
    }
}
