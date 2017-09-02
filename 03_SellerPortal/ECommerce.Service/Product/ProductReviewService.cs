using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Product;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Service.Product
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProductReviewService
    {
        /// <summary>
        /// Queries the product review basic information list.
        /// </summary>
        /// <param name="queryCriteria">The query criteria.</param>
        /// <returns></returns>
        public static QueryResult<ProductReviewQueryBasicInfo> QueryProductReviewBasicInfoList(ProductReviewQueryFilter queryCriteria)
        {
            int totalCount = 0;
            QueryResult<ProductReviewQueryBasicInfo> result = new QueryResult<ProductReviewQueryBasicInfo>();

            List<ProductReviewQueryBasicInfo> list =
                ProductReviewDA.QueryProductReviewBasicInfoList(queryCriteria, out totalCount);

            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = queryCriteria.PageIndex,
                PageSize = queryCriteria.PageSize,
                TotalCount = totalCount,
            };

            return result;
        }

        /// <summary>
        /// 批量审核产品评论
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewValid(List<int> items, string currentUser)
        {
            ProductReviewDA.BatchSetProductReviewStatus(items, "A", currentUser);
        }

        /// <summary>
        /// 批量作废产品评论
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewInvalid(List<int> items, string currentUser)
        {
            ProductReviewDA.BatchSetProductReviewStatus(items, "D", currentUser);
        }

        /// <summary>
        /// 批量阅读产品评论
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewRead(List<int> items, string currentUser)
        {
            ProductReviewDA.BatchSetProductReviewStatus(items, "E", currentUser);
        }

        /// <summary>
        /// 保存备注
        /// </summary>
        /// <param name="item">The item.</param>
        public static void SaveProductReviewRemark(ProductReviewInfo item)
        {
            using (ITransaction transaction = TransactionManager.Create())
            {
                if (item.IsIndexHotReview == "Y")//操作首页热评，更新或新建
                {
                    ProductReviewDA.UpdateHomepageForProductReview(item.SysNo.Value, "H");
                }
                else
                {
                    ProductReviewDA.DeleteHomepageForProductReview(item.SysNo.Value, "H");
                }
                if (item.IsServiceHotReview == "Y")//操作首页服务热评，更新或新建
                {
                    ProductReviewDA.UpdateHomepageForProductReview(item.SysNo.Value, "S");
                }
                else
                {
                    ProductReviewDA.DeleteHomepageForProductReview(item.SysNo.Value, "S");
                }


                ProductReviewDA.UpdateProductReview(item);

                transaction.Complete();
            }
        }

        /// <summary>
        /// Loads the product review without reply.
        /// </summary>
        /// <param name="sysNo">The item system no.</param>
        /// <returns></returns>
        public static ProductReviewInfo LoadProductReviewWithoutReply(int sysNo)
        {
            ProductReviewInfo item = ProductReviewDA.LoadProductReview(sysNo);//评论主题

            return item;
        }


        /// <summary>
        /// Loads the product review with reply.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static ProductReviewInfo LoadProductReviewWithReply(int sysNo)
        {
            ProductReviewInfo item = ProductReviewDA.LoadProductReview(sysNo);//评论主题
            if (item != null)
            {
                item.ProductReviewReplyList = ProductReviewDA.GetProductReviewReplyList(sysNo);//评论回复列表
                item.VendorReplyList = ProductReviewDA.GetProductReviewFactoryReply(sysNo);//厂商回复
            }

            return item;
        }


        /// <summary>
        /// Gets the product review factory reply.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <param name="sellerSysNo">The seller system no.</param>
        /// <returns></returns>
        public static QueryResult<ProductReviewReplyInfo> GetProductReviewFactoryReply(int sysNo, int? sellerSysNo)
        {
            QueryResult<ProductReviewReplyInfo> result = new QueryResult<ProductReviewReplyInfo>();

            var list = ProductReviewDA.GetProductReviewFactoryReply(sysNo);
            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                TotalCount = list == null ? 0 : list.Count,
            };

            return result;
        }

        /// <summary>
        /// Deletes the product review image.
        /// </summary>
        /// <param name="image">The image.</param>
        public static void DeleteProductReviewImage(string image)
        {
            ProductReviewDA.DeleteProductReviewImage(image);
        }


        /// <summary>
        /// 批量审核通过产品评论回复
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewReplyValid(List<int> items)
        {
            ProductReviewDA.BatchSetProductReviewReplyStatus(items, "A");
        }

        /// <summary>
        /// 批量作废产品评论回复
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewReplyInvalid(List<int> items)
        {
            ProductReviewDA.BatchSetProductReviewReplyStatus(items, "D");
        }

        /// <summary>
        /// 批量阅读产品评论回复
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductReviewReplyRead(List<int> items)
        {
            ProductReviewDA.BatchSetProductReviewReplyStatus(items, "E");
        }

        /// <summary>
        /// 添加产品评论回复
        /// </summary>
        /// <param name="item">The item.</param>
        public static void AddProductReviewReply(ProductReviewReplyInfo item)
        {
            ProductReviewDA.AddProductReviewReply(item);
        }
    }
}
