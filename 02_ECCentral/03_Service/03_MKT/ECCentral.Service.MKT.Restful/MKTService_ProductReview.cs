using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private ProductReviewAppService productReviewAppService = ObjectFactory<ProductReviewAppService>.Instance;

        #region 产品评论

        /// <summary>
        /// 批量审核产品评论
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewValid", Method = "PUT")]
        public virtual void BatchSetProductReviewValid(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewValid(items);
        }

        /// <summary>
        /// 批量作废产品评论
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewInvalid", Method = "PUT")]
        public virtual void BatchSetProductReviewInvalid(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewInvalid(items);
        }

        /// <summary>
        /// 批量阅读产品评论
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewRead", Method = "PUT")]
        public virtual void BatchSetProductReviewRead(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewRead(items);
        }

        /// <summary>
        /// 产品评论查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductReview", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductReview(ProductReviewQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductReview(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }


       /// <summary>
        /// 删除产品评论图片
       /// </summary>
       /// <param name="image"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/DeleteProductReviewImage", Method = "POST")]
        public void DeleteProductReviewImage(string image)
        {
            productReviewAppService.DeleteProductReviewImage(image);
        }

        [WebInvoke(UriTemplate = "/CommentInfo/CreateProductReview", Method = "POST")]
        public ProductReview CreateProductReview(ProductReview msg)
        {

            msg = ObjectFactory<IProductReviewDA>.Instance.CreateProductReview(msg);
            return msg;
        }

        #endregion

        #region 产品评论—回复
        /// <summary>
        /// 产品评论回复查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductReviewReply", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductReviewReply(ProductReviewReplyQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductReviewReply(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 根据评论编号，加载相应的评论回复。
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadProductReview", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ProductReview LoadProductReview(int sysNo)
        {
            return productReviewAppService.LoadProductReview(sysNo);
        }

        /// <summary>
        /// 批量审核通过产品评论回复
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewReplyValid", Method = "PUT")]
        public void BatchSetProductReviewReplyValid(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewReplyValid(items);
        }

        /// <summary>
        /// 批量作废产品评论回复
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewReplyInvalid", Method = "PUT")]
        public void BatchSetProductReviewReplyInvalid(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewReplyInvalid(items);
        }

        /// <summary>
        /// 批量阅读产品评论回复
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductReviewReplyRead", Method = "PUT")]
        public void BatchSetProductReviewReplyRead(List<int> items)
        {
            productReviewAppService.BatchSetProductReviewReplyRead(items);
        }

        /// <summary>
        /// 保存备注
        /// </summary>
        /// <param name="log"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/SaveProductReviewRemark", Method = "PUT")]
        public virtual void SaveProductReviewRemark(ProductReview log)
        {
            productReviewAppService.SaveProductReviewRemark(log);
        }

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/SubmitReplyToCSProcess", Method = "PUT")]
        public virtual void SubmitReplyToCSProcess(ECCentral.BizEntity.SO.SOComplaintCotentInfo item)
        {
            productReviewAppService.SubmitReplyToCSProcess(item);
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateProductReviewVendorReplyStatus", Method = "PUT")]
        public virtual void UpdateProductReviewVendorReplyStatus(ProductReviewReply item)
        {
            productReviewAppService.UpdateProductReviewVendorReplyStatus(item);
        }

        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        ///1.	网友回复，需通过审核才展示。
        ///2.	厂商回复（通过Seller Portal），需通过审核才展示。
        ///3.	IPP系统中回复，默认直接展示。
        /// </summary>
        [WebInvoke(UriTemplate = "/CommentInfo/AddProductReviewReply", Method = "POST")]
        public virtual void AddProductReviewReply(ProductReviewReply item)
        {
            productReviewAppService.AddProductReviewReply(item);
        }

        /// <summary>
        /// 回复产品评论邮件操作
        /// </summary>
        /// <param name="log"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateProductReviewMailLog", Method = "PUT")]
        public virtual void UpdateProductReviewMailLog(ProductReview log)
        {
            productReviewAppService.UpdateProductReviewMailLog(log);
        }
        #endregion

    }
}
