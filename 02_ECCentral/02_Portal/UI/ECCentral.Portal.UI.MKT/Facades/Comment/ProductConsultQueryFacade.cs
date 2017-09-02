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
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductConsultQueryFacade
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

        public ProductConsultQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 加载咨询管理
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadProductConsult(int sysNo, EventHandler<RestClientEventArgs<ProductConsult>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadProductConsult";
            restClient.Query<ProductConsult>(relativeUrl, sysNo, callback);
        }

        #region 咨询管理

        /// <summary>
        /// 查询咨询管理
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductConsult(ProductConsultQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductConsult";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ProductConsultQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductConsult";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
        ///// <summary>
        ///// 添加咨询管理回复
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="callback"></param>
        //public void CreateProductConsultReply(ProductConsultReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/CreateProductConsultReply";
        //    restClient.Create(relativeUrl, item, callback);
        //}

        ///// <summary>
        ///// 编辑咨询管理回复
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="callback"></param>
        //public void UpdateProductConsultReply(ProductConsult item, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/UpdateProductConsult";
        //    restClient.Update(relativeUrl, item, callback);
        //}

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultRead(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultRead";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultValid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultValid";
            restClient.Update(relativeUrl, items, callback);
        }

        #endregion

        #region 咨询管理回复
        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultReplyValid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultReplyValid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultReplyInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultReplyInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultReplyRead(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultReplyRead";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductConsultReplyTop(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductConsultReplyTop";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量取消置顶
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchCancelProductConsultReplyTop(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchCancelProductConsultReplyTop";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 查询咨询管理回复
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductConsultReply(ProductConsultReplyQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductConsultReply";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }


        public void ExportReplyExcelFile(ProductConsultReplyQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductConsultReply";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 添加咨询管理回复
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateProductConsultDetailReply(ProductConsultReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateProductConsultDetailReply";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 编辑咨询管理回复
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void UpdateProductConsultReply(ProductConsultReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/CommentInfo/UpdateProductConsultReply";
        //    restClient.Update(relativeUrl, item, callback);
        //}
        #endregion


        /// <summary>
        /// 获取关于咨询的所有回复，除去厂商回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <param name="callback"></param>
        public void GetProductConsultReplyList(int consultSysNo, EventHandler<RestClientEventArgs<List<ProductConsultReply>>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/GetProductConsultReplyList";
            restClient.Query<List<ProductConsultReply>>(relativeUrl, consultSysNo, callback);
        }

        /// <summary>
        /// 获取厂商关于咨询的回复列表
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <param name="callback"></param>
        public void GetProductConsultFactoryReplyList(int consultSysNo, EventHandler<RestClientEventArgs<List<ProductConsultReply>>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/GetProductConsultFactoryReplyList";
            restClient.Query<List<ProductConsultReply>>(relativeUrl, consultSysNo, callback);
        }

        /// <summary>
        /// 获取咨询相关的邮件日志
        /// </summary>
        /// <param name="refSysNo"></param>
        /// <param name="callback"></param>
        public void QueryProductConsultMailLog(int refSysNo, EventHandler<RestClientEventArgs<ProductReviewMailLog>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductConsultMailLog";
            restClient.Query<ProductReviewMailLog>(relativeUrl, refSysNo, callback);
        }

        /// <summary>
        /// 回复邮件操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateProductConsultMailLog(ProductReview item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateProductConsultMailLog";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 批准发布操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void ApproveProductConsultRelease(ProductConsultReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/ApproveProductConsultRelease";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 拒绝发布操作
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void RejectProductConsultRelease(ProductConsultReply sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/RejectProductConsultRelease";
            restClient.Update(relativeUrl, sysNo, callback);
        }
    }
}
