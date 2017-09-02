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
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class SegmentQueryFacade
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

        public SegmentQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询中文词库
        /// </summary>
        /// <param name="callback"></param>
        public void QuerySegment(SegmentQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QuerySegment";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        public void ExportExcelFile(SegmentQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QuerySegment";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载中文词库
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadSegmentInfo(int sysNo, EventHandler<RestClientEventArgs<SegmentInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadSegmentInfo";
            restClient.Query<SegmentInfo>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddSegmentInfo(SegmentInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddSegmentInfo";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新中文词库
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateSegmentInfo(SegmentInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/UpdateSegmentInfo";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetSegmentInfosValid(List<SegmentInfo> items, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/SetSegmentInfosValid";
            
            items.ForEach(p=> {
                p.CurrentUser = CPApplication.Current.LoginUser.LoginName;
            });
            restClient.Update<List<int>>(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetSegmentInfosInvalid(List<SegmentInfo> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/SetSegmentInfosInvalid";
            items.ForEach(p =>
            {
                p.CurrentUser = CPApplication.Current.LoginUser.LoginName;
            });
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void DeleteSegmentInfos(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/DeleteSegmentInfos";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量上传
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void BatchImportSegment(string fileIdentity, EventHandler<RestClientEventArgs<SegmentInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchImportSegment";
            restClient.Create<SegmentInfo>(relativeUrl, fileIdentity, callback);
        }
    }
}
