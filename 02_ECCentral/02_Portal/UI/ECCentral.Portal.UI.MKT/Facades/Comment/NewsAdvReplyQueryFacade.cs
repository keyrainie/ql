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
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class NewsAdvReplyQueryFacade
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

        public NewsAdvReplyQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 加载编辑人员列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetNewAdvReplyCreateUsers(EventHandler<RestClientEventArgs<List<ECCentral.BizEntity.Common.UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/GetNewAdvReplyCreateUsers";
            restClient.Query<List<ECCentral.BizEntity.Common.UserInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="callback"></param>
        public void LoadNewsAdvReply(int sysNo, EventHandler<RestClientEventArgs<NewsAdvReply>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/LoadNewsAdvReply";
            restClient.Query<NewsAdvReply>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 查询公告及促销评论
        /// </summary>
        /// <param name="callback"></param>
        public void QueryNewsAdvReply(NewsAdvReplyQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/NewsInfo/QueryNewsAdvReply";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        public void ExportExcelFile(NewsAdvReplyQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/NewsInfo/QueryNewsAdvReply";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 删除相关图片列表
        /// </summary>
        /// <param name="image"></param>
        /// <param name="callback"></param>
        public void DeleteProductReviewImage(string image, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/DeleteProductReviewImage";
            restClient.Update(relativeUrl, image, callback);
        }


        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateNewsAdvReply(NewsAdvReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/UpdateNewsAdvReply";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 回复公告及促销评论
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void CreateNewsAdvReply(NewsAdvReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/CreateNewsAdvReply";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void BatchSetNewsAdvReplyShow(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/BatchSetNewsAdvReplyShow";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void BatchSetNewsAdvReplyHide(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/BatchSetNewsAdvReplyHide";
            restClient.Update(relativeUrl, items, callback);
        } 

        /// <summary>
        /// 单个更新状态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateNewsAdvReplyStatus(NewsAdvReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/UpdateNewsAdvReply";
            restClient.Update(relativeUrl, item, callback);
        } 
    }
}
