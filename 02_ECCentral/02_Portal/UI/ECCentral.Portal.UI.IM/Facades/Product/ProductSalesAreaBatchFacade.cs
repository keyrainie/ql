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
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductSalesAreaBatchFacade
    {
        private readonly RestClient restClient;
        private const string GetProductUrl = "/IMService/ProductSalesAreaBatch/GetProductByQuery";
        private const string GetStockUrl = "/IMService/Product/GetStockList";
        private const string GetAllProvinceUrl = "/IMService/ProductSalesAreaBatch/GetAllProvince";
        private const string UpdateProductSalesAreaInfoUrl = "/IMService/Product/UpdateProductSalesAreaInfoByList";
        private const string GetProductSalesAreaBatchListUrl = "/IMService/ProductSalesAreaBatch/GetProductSalesAreaBatchList";
        private const string RemoveItemSalesAreaListBatchUrl = "/IMService/ProductSalesAreaBatch/RemoveItemSalesAreaListBatch";
        private const string RemoveProvinceUrl = "/IMService/ProductSalesAreaBatch/RemoveProvince";
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

        public ProductSalesAreaBatchFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductSalesAreaBatchFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 根据query得到要设置区域商品的信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductByQuery(ProductSalesAreaBatchQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductSalesAreaBatchQueryFilter query = model.ConvertVM<ProductSalesAreaBatchQueryVM, ProductSalesAreaBatchQueryFilter>();
            query.PageInfo = new PagingInfo() {PageIndex=PageIndex,PageSize=PageSize,SortBy=SortField };
            restClient.QueryDynamicData(GetProductUrl, query, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });
          
        }
        /// <summary>
        /// 得到设置区域商品的信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductSalesAreaBatchList(ProductSalesAreaBatchQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductSalesAreaBatchQueryFilter query = model.ConvertVM<ProductSalesAreaBatchQueryVM, ProductSalesAreaBatchQueryFilter>();
            query.PageInfo = new PagingInfo() { PageIndex = PageIndex, PageSize = PageSize, SortBy = SortField };
            restClient.QueryDynamicData(GetProductSalesAreaBatchListUrl, query, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });

        }

        /// <summary>
        ///  获取所有仓库
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetWarehouseList(string companyCode, EventHandler<RestClientEventArgs<List<WarehouseInfo>>> callback)
        {
            restClient.Query(GetStockUrl, companyCode, callback);
        }
        /// <summary>
        /// 获取所有省份
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllProvince(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetAllProvinceUrl, null, (obj, arg) => 
            {
                callback(obj, arg);
            });
        }
        public void UpdateProductSalesAreaInfo(List<ProductInfo> listInfo,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(UpdateProductSalesAreaInfoUrl, listInfo, callback);
        }
        public void RemoveItemSalesAreaListBatch(List<ProductSalesAreaBatchInfo> listInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(RemoveItemSalesAreaListBatchUrl, listInfo, callback);
        }
        /// <summary>
        /// 取消省份
        /// </summary>
        /// <param name="info"></param>
        public void RemoveProvince(ProductSalesAreaBatchInfo info, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(RemoveProvinceUrl, info, callback);
        }
    }
}
