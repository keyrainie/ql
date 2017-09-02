using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductReviewDA
    {
        #region 产品评论

        /// <summary>
        /// 产品评论的批量审核状态
        /// </summary>
        /// <param name="items"></param>
        void BatchSetProductReviewStatus(List<int> items, string status);

        ProductReview CreateProductReview(ProductReview item);

        void UpdateProductReview(ProductReview item);
        
        /// <summary>
        /// 删除评论相关图片
        /// </summary>
        /// <param name="image"></param>
        void DeleteProductReviewImage(string image);

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateProductReviewVendorReplyStatus(ProductReviewReply item);

        /// <summary>
        /// 根据评论编号，加载相应的评论回复。
        /// </summary>
        /// <param name="itemID"></param>
        ProductReview LoadProductReview(int itemID);

        /// <summary>
        /// 更新评论之后更新Homepage中的记录
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="type"></param>
        void UpdateHomepageForProductReview(int sysNo, string type);

        /// <summary>
        /// 更新评论之后删除Homepage中的记录
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="type"></param>
        void DeleteHomepageForProductReview(int sysNo, string type);

        /// <summary>
        /// 加载商品评论
        /// </summary>
        /// <returns></returns>
        List<ProductReview> GetProductReview();

        ProductReview GetProductReview(int productID);

        void AuditApproveProductReview(int commentID);

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="commentID"></param>
        void AuditRefuseProductReview(int commentID);

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="commentID"></param>
        void SubmitProductReview(int commentID);
        #endregion

        #region 产品评论—回复
        /// <summary>
        /// 根据讨论论编号，加载相应的讨论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        List<ProductReviewReply> GetProductReviewReplyList(int sysNo);

        /// <summary>
        /// 根据评论编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        List<ProductReviewReply> GetProductReviewFactoryReply(int sysNo);

        /// <summary>
        /// 批量设置产品评论—回复的状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="status"></param>
        void BatchSetProductReviewReplyStatus(List<int> items, string status);

        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        ///1.	网友回复，需通过审核才展示。
        ///2.	厂商回复（通过Seller Portal），需通过审核才展示。
        ///3.	IPP系统中回复，默认直接展示。
        /// </summary>
        void AddProductReviewReply(ProductReviewReply item);

        /// <summary>
        /// 拒绝发布厂商回复
        /// </summary>
        /// <param name="itemID"></param>
        //void AuditRefuseProductReviewReply(int itemID);

        /// <summary>
        /// 审核产品评论回复，并在website中展示。
        /// </summary>
        /// <param name="itemID"></param>
        //void AuditApproveProductReviewReply(int itemID);

        /// <summary>
        /// 作废评论回复，不展示在website中。
        /// </summary>
        /// <param name="itemID"></param>
       //void VoidProductReviewReply(int itemID);
        #endregion
    }
}
