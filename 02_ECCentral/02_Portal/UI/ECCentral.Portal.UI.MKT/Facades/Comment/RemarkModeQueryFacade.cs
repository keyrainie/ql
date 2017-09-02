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
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class RemarkModeQueryFacade
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

        public RemarkModeQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 公告及促销评论模式
        /// <summary>
        /// 加载公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadRemarkMode(string companyCode, EventHandler<RestClientEventArgs<RemarkMode>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadRemarkMode";
            restClient.Query<RemarkMode>(relativeUrl, companyCode, callback);
        }

        /// <summary>
        /// 保存公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateRemarkMode(RemarkMode item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateRemarkMode";
            restClient.Update(relativeUrl, item, callback);
        }
        
        #endregion

        #region 咨询模式

        /// <summary>
        /// 批量保存咨询评论对应的评论模式
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        //public void UpdateConsultRemarkModeList(List<RemarkMode> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/UpdateConsultRemarkModeList";
        //    restClient.Update(relativeUrl, items, callback);
        //}
        #endregion

        /// <summary>
        /// 查询咨询评论对应的评论模式列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryRemarkModeList(RemarkModeQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryRemarkModeList";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportRemarkModeExcelFile(RemarkModeQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryRemarkModeList";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        #region 评论模式

        /// <summary>
        /// 查询咨询评论对应的评论模式列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void QueryCommentRemarkMode(RemarkModeQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/QueryRemarkModeList";
        //    restClient.Query(relativeUrl, filter, callback);
        //}

        /// <summary>
        /// 批量保存评论模式评论对应的评论模式
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        //public void UpdateCommentRemarkModeList(List<RemarkMode> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/UpdateCommentRemarkModeList";
        //    restClient.Update(relativeUrl, items, callback);
        //}
        #endregion

        #region 讨论模式


        /// <summary>
        /// 查询咨询评论对应的评论模式列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void QueryDiscussRemarkMode(RemarkModeQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/QueryRemarkModeList";
        //    restClient.Update(relativeUrl, filter, callback);
        //}

        /// <summary>
        /// 批量保存讨论模式评论对应的评论模式
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        //public void UpdateDiscussRemarkModeList(List<RemarkMode> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/UpdateDiscussRemarkModeList";
        //    restClient.Update(relativeUrl, items, callback);
        //}
        #endregion

        /// <summary>
        /// 批量保存公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateOtherRemarkMode(List<RemarkMode> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateOtherRemarkMode";
            restClient.Update(relativeUrl, items, callback);
        }
    }
}
