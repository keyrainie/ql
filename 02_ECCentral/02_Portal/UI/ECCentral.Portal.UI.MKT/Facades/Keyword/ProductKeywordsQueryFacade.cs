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
using ECCentral.QueryFilter.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductKeywordsQueryFacade
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

        public ProductKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询产品页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetProductKeywordsEditUserList(string companyCode, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetProductKeywordsEditUserList";
            restClient.Query<List<UserInfo>>(relativeUrl, companyCode, callback);
        }

        /// <summary>
        /// 查询产品页面关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryProductPageKeywords(ProductKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryProductPageKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ProductKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryProductPageKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 添加产品页面关键字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void AddProductPageKeywords(ProductPageKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddProductPageKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 获取产品页面关键字   
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        //public void LoadProductPageKeywords(int sysNo, EventHandler<RestClientEventArgs<ProductPageKeywords>> callback)
        //{
        //    string relativeUrl = "/MKTService/KeywordsInfo/LoadProductPageKeywords";
        //    restClient.Query<ProductPageKeywords>(relativeUrl, sysNo, callback);
        //}

        /// <summary>
        /// 更新产品页面关键字   
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateProductPageKeywords(ProductPageKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/UpdateProductPageKeywords";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 删除或添加产品页面关键字   
        /// </summary>
        /// <param name="rsp"></param>
        /// <param name="callback"></param>
        public void BatchUpdateProductPageKeywords(ProductPageKeywordsRsp rsp, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchUpdateProductPageKeywords";
            restClient.Update(relativeUrl, rsp, callback);
        }

        /// <summary>
        /// 批量上传
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void BatchImportProductKeywords(string fileIdentity, EventHandler<RestClientEventArgs<ProductPageKeywords>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchImportProductKeywords";
            restClient.Create<ProductPageKeywords>(relativeUrl, fileIdentity, callback);
        }
        /// <summary>
        /// 得到某类下所有的属性
        /// </summary>
        /// <param name="Category3SysNo"></param>
        /// <param name="callback"></param>
        public void GetPropertyByCategory3SysNo(int Category3SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetPropertyByCategory3SysNo";
            restClient.QueryDynamicData(relativeUrl, Category3SysNo, callback);
        }
        /// <summary>
        /// 根据属性得到所有属性值
        /// </summary>
        /// <param name="PropertySysNo"></param>
        /// <param name="callback"></param>
        public void GetPropertyValueByPropertySysNo(int PropertySysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetPropertyValueByPropertySysNo";
            restClient.QueryDynamicData(relativeUrl, PropertySysNo, callback);
        }
    }
}
