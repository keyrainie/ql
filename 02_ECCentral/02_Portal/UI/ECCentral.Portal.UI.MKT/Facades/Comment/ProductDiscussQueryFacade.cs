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

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductDiscussQueryFacade
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

        public ProductDiscussQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 产品讨论
        /// <summary>
        /// 查询产品讨论列表
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductDiscuss(ProductDiscussQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductDiscuss";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ProductDiscussQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductDiscuss";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 添加产品讨论
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void CreateProductDiscuss(ProductDiscussDetail item, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/CreateProductDiscuss";
        //    restClient.Create(relativeUrl, item, callback);
        //}

        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EditProductDiscuss(ProductDiscussDetail item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/EditProductDiscuss";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadProductDiscuss(int sysNo, EventHandler<RestClientEventArgs<ProductDiscussDetail>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadProductDiscuss";
            restClient.Query<ProductDiscussDetail>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchRefuseProductDiscuss(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchRefuseProductDiscuss";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 产品讨论审核通过后显示在website页面中
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchApproveProductDiscuss(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchApproveProductDiscuss";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        ///批量阅读
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchReadProductDiscuss(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchReadProductDiscuss";
            restClient.Update(relativeUrl, items, callback);
        }
        #endregion

        #region 产品讨论回复

        /// <summary>
        /// 添加产品讨论回复
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void CreateProductDiscussReply(ProductDiscussReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/CreateProductDiscussReply";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 查询产品讨论回复
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductDiscussReply(ProductDiscussReplyQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductDiscussReply";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }


        public void ExportReplyExcelFile(ProductDiscussReplyQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductDiscussReply";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
        /// <summary>
        /// 作废产品评论回复,批量屏蔽
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchVoidProductDiscussReply(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchVoidProductDiscussReply";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 产品讨论回复审核通过后显示在website页面中,批量审核
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchApproveProductDiscussReply(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchApproveProductDiscussReply";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        ///产品讨论回复批量阅读
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchReadProductDiscussReply(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchReadProductDiscussReply";
            restClient.Update(relativeUrl, items, callback);
        }
        #endregion
    }
}
