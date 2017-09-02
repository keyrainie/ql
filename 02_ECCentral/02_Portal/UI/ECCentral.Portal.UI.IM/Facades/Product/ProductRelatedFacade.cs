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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductRelatedFacade
    {
        private readonly RestClient restClient;
        private const string GetItemRelateUrl = "/IMService/ProductRelated/GetProductRelatedByQuery";
        private const string GetStockUrl = "/IMService/Product/GetStockList";
        private const string CreateItemRelateUrl = "/IMService/ProductRelated/CreateProductRelated";
        private const string DeleteItemRelateUrl = "/IMService/ProductRelated/DeleteProductRelated";
        private const string UpdateProductRelatePriorityUrl = "/IMService/ProductRelated/UpdateProductRelatePriority";
        private const string CreateItemRelatedByListUrl = "/IMService/ProductRelated/CreateItemRelatedByList";
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

        public ProductRelatedFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductRelatedFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 根据query得到相关商品信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductRelatedByQuery(ProductRelatedQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            ProductRelatedQueryFilter query = new ProductRelatedQueryFilter();
           int ProductSysNo;
           int RelatedProductSysNo;
           if (int.TryParse(model.RelatedProductSysNo, out RelatedProductSysNo))
           {
               query.RelatedProductSysNo = RelatedProductSysNo;
           }
           if (int.TryParse(model.ProductSysNo, out ProductSysNo))
           {
               query.ProductSysNo = ProductSysNo;
           }
           query.PMUserSysNo = Convert.ToInt32(model.PMUserSysNo);
            query.PageInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetItemRelateUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 创建相关商品信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateItemRelated(ProductRelatedVM model, EventHandler<RestClientEventArgs<ProductRelatedInfo>> callback)
        {

            ProductRelatedInfo info = new ProductRelatedInfo();
            info.Priority =Convert.ToInt32(model.Priority);
            info.ProductSysNo = Convert.ToInt32(model.ProductSysNo);
            info.RelatedProductSysNo = Convert.ToInt32(model.RelatedProductSysNo);
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            info.IsMutual = model.IsMutual;
            restClient.Create(CreateItemRelateUrl, info, callback);
        }
            /// <summary>
            /// Delete
            /// </summary>
            /// <param name="list"></param>
            /// <param name="callback"></param>
        public void DeleteItemRelated(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            restClient.Delete(DeleteItemRelateUrl, list,callback);
        }
        public void UpdateProductRelatePriority(List<ProductRelatedVM> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<ProductRelatedInfo> data = new List<ProductRelatedInfo>();
            int Priority;
            foreach (var item in list)
            {
                if (int.TryParse(item.Priority, out Priority))
                {
                    data.Add(new ProductRelatedInfo() { SysNo = item.SysNo, Priority = Convert.ToInt32(item.Priority) });
                }
               
            }
            restClient.Update(UpdateProductRelatePriorityUrl, data, callback);
        }

        /// <summary>
        /// 批量创建相关商品
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void CreateItemRelatedByList(List<ProductRelatedVM> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<ProductRelatedInfo> data = new List<ProductRelatedInfo>();
            foreach (var item in list)
            {
                data.Add(new ProductRelatedInfo() { Priority = Convert.ToInt32(item.Priority), ProductSysNo = Convert.ToInt32(item.ProductSysNo), RelatedProductSysNo = Convert.ToInt32(item.RelatedProductSysNo), IsMutual = item.IsMutual, CompanyCode = CPApplication.Current.CompanyCode, LanguageCode = CPApplication.Current.LanguageCode,ProductID=item.ProductID,RelatedProductID=item.RelatedProductID});
            }
            restClient.Create(CreateItemRelatedByListUrl, data, callback);
         }
    }
}
