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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class KeyWordsForProductQueryFacade
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

        public KeyWordsForProductQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询关键字对应商品   
        /// </summary>
        /// <param name="callback"></param>
        public void QueryKeyWordsForProduct(KeyWordsForProductQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryKeyWordsForProduct";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        public void ExportExcelFile(KeyWordsForProductQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryKeyWordsForProduct";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载关键字对应商品    
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadProductKeywordsInfo(int sysNo, EventHandler<RestClientEventArgs<HotSearchKeyWords>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadProductKeywordsInfo";
            restClient.Query<HotSearchKeyWords>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加关键字对应商品   
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddProductKeywords(ProductKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddProductKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新关键字对应商品   
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EditProductKeywords(ProductKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/EditProductKeywords";
            restClient.Update(relativeUrl, item, callback);
        }
        /// <summary>
        /// 屏蔽
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public  void ChangeProductKeywordsStatus(List<ProductKeywordsInfo> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/ChangeProductKeywordsStatus";
            restClient.Update(relativeUrl, list, callback);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void DeleteProductKeywords(List<int> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/DeleteProductKeywords";
            restClient.Delete(relativeUrl, list, callback);
        }
    }
}
