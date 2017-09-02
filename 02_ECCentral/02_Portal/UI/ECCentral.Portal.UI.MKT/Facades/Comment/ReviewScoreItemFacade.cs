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

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ReviewScoreItemFacade
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

        public ReviewScoreItemFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 评分项定义查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryProductReviewScore(ReviewScoreItemQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReviewScore";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ReviewScoreItemQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReviewScore";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetReviewScoreValid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/SetReviewScoreValid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetReviewScoreInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/SetReviewScoreInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadReviewScoreItem(int sysNo, EventHandler<RestClientEventArgs<ReviewScoreItem>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadReviewScoreItem";
            restClient.Query<ReviewScoreItem>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加评分项定义
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void CreateReviewScoreItem(ReviewScoreItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/CreateReviewScoreItem";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新评分项定义
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateReviewScoreItem(ReviewScoreItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateReviewScoreItem";
            restClient.Update(relativeUrl, item, callback);
        }
    }
}
