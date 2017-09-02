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
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class HotKeywordsQueryFacade
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

        public HotKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询热门关键字 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryHotKeywords(HotKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryHotKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(HotKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryHotKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载热门关键字 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void LoadHotSearchKeywords(int sysNo, EventHandler<RestClientEventArgs<HotSearchKeyWords>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadHotSearchKeywords";
            restClient.Query<HotSearchKeyWords>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetHotKeywordsEditUserList(string companyCode, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetHotKeywordsEditUserList";
            restClient.Query<List<UserInfo>>(relativeUrl, companyCode, callback);
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void GetHotKeywordsListByPageType(HotSearchKeyWords item, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetHotKeywordsListByPageType";
            restClient.Query<List<string>>(relativeUrl, item, callback);
        }

        /// <summary>
        /// 添加热门关键字 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void AddHotSearchKeywords(HotSearchKeyWords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddHotSearchKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新热门关键字 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void EditHotSearchKeywords(HotSearchKeyWords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/EditHotSearchKeywords";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void ChangeHotSearchedKeywordsStatus(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/ChangeHotSearchedKeywordsStatus";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetHotKeywordsInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchSetHotKeywordsInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetHotKeywordsAvailable(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchSetHotKeywordsAvailable";
            restClient.Update(relativeUrl, items, callback);
        }
    }
}
