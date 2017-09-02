using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM.Product.Request;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductPriceRequestQueryFacade
    {
        #region 字段以及构造函数
        private readonly IPage viewPage;
        private readonly RestClient restClient;
        const string UPdateRelativeUrl = "/IMService/Product/AuditProductPriceRequest";
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductPriceRequestQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductPriceRequestQueryFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 查询价格申请
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryProductPriceRequestList(ProductPriceRequestQueryVM model
            , int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductPriceRequestQueryFilter filter;
            filter = model.ConvertVM<ProductPriceRequestQueryVM, ProductPriceRequestQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Product/QueryProductPriceRequesList";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (!(args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public virtual void AuditProductPriceRequest(List<ProductPriceRequestInfo> entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(UPdateRelativeUrl, entity, callback);
        }

        public void ImPortExecl(ProductPriceRequestQueryVM model, ColumnSet[] columns)
        {
            ProductPriceRequestQueryFilter query;
            query = model.ConvertVM<ProductPriceRequestQueryVM, ProductPriceRequestQueryFilter>();
            query.PagingInfo = null;
            restClient.ExportFile("/IMService/Product/QueryProductPriceRequesList", query, columns);
        }
    }
}
