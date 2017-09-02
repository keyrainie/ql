using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ProductReviewAppService))]
    public class ProductReviewAppService
    {

        #region 产品评论
        /// <summary>
        /// 批量审核产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewValid(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewValid(items);
        }

        /// <summary>
        /// 批量作废产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewInvalid(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewInvalid(items);
        }
        /// <summary>
        /// 批量阅读产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewRead(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewRead(items);
        }


        public virtual void DeleteProductReviewImage(string image)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.DeleteProductReviewImage(image);
        }

        /// <summary>
        /// 根据评论编号，加载相应的评论回复。
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public virtual ProductReview LoadProductReview(int itemID)
        {
            return ObjectFactory<ProductReviewProcessor>.Instance.LoadProductReview(itemID);
        }
        #endregion

        #region 产品评论—回复
        /// <summary>
        /// 批量审核通过产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyValid(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewReplyValid(items);
        }

        /// <summary>
        /// 批量作废产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyInvalid(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewReplyInvalid(items);
        }

        /// <summary>
        /// 批量阅读产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyRead(List<int> items)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.BatchSetProductReviewReplyRead(items);
        }

        /// <summary>
        /// 保存备注
        /// </summary>
        /// <param name="item"></param>
        public virtual void SaveProductReviewRemark(ProductReview item)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.SaveProductReviewRemark(item);
        }

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="item"></param>
        public virtual void SubmitReplyToCSProcess(ECCentral.BizEntity.SO.SOComplaintCotentInfo item)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.SubmitReplyToCSProcess(item);
        }


        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        ///1.	网友回复，需通过审核才展示。
        ///2.	厂商回复（通过Seller Portal），需通过审核才展示。
        ///3.	IPP系统中回复，默认直接展示。
        /// </summary>
        public virtual void AddProductReviewReply(ProductReviewReply item)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.AddProductReviewReply(item);
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductReviewVendorReplyStatus(ProductReviewReply item)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.UpdateProductReviewVendorReplyStatus(item);
        }

        /// <summary>
        /// 回复产品评论邮件操作
        /// </summary>
        /// <param name="log"></param>
        public virtual void UpdateProductReviewMailLog(ProductReview log)
        {
            ObjectFactory<ProductReviewProcessor>.Instance.UpdateProductReviewMailLog(log);
        }

        #endregion

    }
}
