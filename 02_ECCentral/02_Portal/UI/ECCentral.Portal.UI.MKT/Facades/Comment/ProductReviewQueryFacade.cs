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
using ECCentral.Portal.UI.MKT.Models;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductReviewQueryFacade
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

        public ProductReviewQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询产品评论
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductReview(ProductReviewQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReview";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(ProductReviewQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReview";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 查询产品评论回复
        /// </summary>
        /// <param name="callback"></param>
        public void QueryProductReviewReply(ProductReviewReplyQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReviewReply";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportReplyExcelFile(ProductReviewReplyQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReviewReply";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
            
        /// <summary>
        /// 加载产品评论及回复列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadProductReview(int sysNo, EventHandler<RestClientEventArgs<ProductReview>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadProductReview";
            restClient.Query<ProductReview>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 删除评论相关图片列表
        /// </summary>
        /// <param name="image"></param>
        /// <param name="callback"></param>
        public void DeleteProductReviewImage(string image, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/DeleteProductReviewImage";
            restClient.Update(relativeUrl, image, callback);
        }

        /// <summary>
        /// 添加产品评论回复
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void AddProductReviewReply(ProductReviewReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/AddProductReviewReply";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 保存备注
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void SaveProductReviewRemark(ProductReview item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/SaveProductReviewRemark";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 回复产品评论邮件操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateProductReviewMailLog(ProductReview item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateProductReviewMailLog";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateProductReviewVendorReplyStatus(ProductReviewReply item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateProductReviewVendorReplyStatus";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SubmitReplyToCSProcess(ECCentral.BizEntity.SO.SOComplaintCotentInfo info, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/SubmitReplyToCSProcess";
            restClient.Update(relativeUrl, info, callback);
        }

        #region 操作
        
        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewRead(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewRead";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewValid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewValid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量审核通过回复
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewReplyValid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewReplyValid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量作废回复
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewReplyInvalid(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewReplyInvalid";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量阅读回复
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchSetProductReviewReplyRead(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/BatchSetProductReviewReplyRead";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 获取产品评论相关的邮件日志
        /// </summary>
        /// <param name="refSysNo"></param>
        /// <param name="callback"></param>
        public void QueryProductReviewMailLog(int refSysNo, EventHandler<RestClientEventArgs<ProductReviewMailLog>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryProductReviewMailLog";
            restClient.Query<ProductReviewMailLog>(relativeUrl, refSysNo, callback);
        }

        public void CreateProductReview(ProductReviewVM vm, EventHandler<RestClientEventArgs<ProductReview>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/CreateProductReview";
             var model = vm.ConvertVM<ProductReviewVM, ProductReview>();
            restClient.Create<ProductReview>(relativeUrl, model, callback);
        }

        #endregion
    }
}
