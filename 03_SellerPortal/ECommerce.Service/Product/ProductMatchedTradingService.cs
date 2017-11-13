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
    public class ProductMatchedTradingService
    {
        /// <summary>
        /// Query查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public static QueryResult<ProductMatchedTradingQueryBasicInfo> QueryProductMatchedTradingBasicInfoList(ProductMatchedTradingQueryFilter queryCriteria)
        {
            int totalCount = 0;
            QueryResult<ProductMatchedTradingQueryBasicInfo> result = new QueryResult<ProductMatchedTradingQueryBasicInfo>();

            List<ProductMatchedTradingQueryBasicInfo> list = ProductMatchedTradingDA.QueryProductMatchedTradingBasicInfoList(queryCriteria, out totalCount);

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
        /// 批量审核
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductMatchedTradingValid(List<int> items, string currentUser)
        {
            ProductMatchedTradingDA.BatchSetProductMatchedTradingStatus(items, "A", currentUser);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductMatchedTradingInvalid(List<int> items, string currentUser)
        {
            ProductMatchedTradingDA.BatchSetProductMatchedTradingStatus(items, "D", currentUser);
        }

        /// <summary>
        /// 批量阅读
        /// </summary> 
        /// <param name="items">The items.</param>
        public static void BatchSetProductMatchedTradingRead(List<int> items, string currentUser)
        {
            ProductMatchedTradingDA.BatchSetProductMatchedTradingStatus(items, "E", currentUser);
        }
        /// <summary>
        /// 根据编号，加载相应的评咨询
        /// </summary>
        /// <param name="sysNo">The item system no.</param>
        /// <returns></returns>
        public static ProductMatchedTradingInfo LoadProductMatchedTradingWithoutReply(int sysNo)
        {
            ProductMatchedTradingInfo item = ProductMatchedTradingDA.LoadProductMatchedTrading(sysNo);

            return item;
        }
        /// <summary>
        /// 添加产品评论回复
        /// </summary>
        /// <param name="item">The item.</param>
        public static void AddProductMatchedTradingReply(ProductMatchedTradingReplyInfo item)
        {
            ProductMatchedTradingDA.AddProductMatchedTradingReply(item);
        }

        /// <summary>
        /// 根据咨询编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <param name="sellerSysNo">The seller system no.</param>
        /// <returns></returns>
        public static QueryResult<ProductMatchedTradingReplyInfo> GetProductMatchedTradingFactoryReply(int sysNo, int? sellerSysNo)
        {
            QueryResult<ProductMatchedTradingReplyInfo> result = new QueryResult<ProductMatchedTradingReplyInfo>();

            var list = ProductMatchedTradingDA.GetProductMatchedTradingFactoryReply(sysNo);
            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                TotalCount = list == null ? 0 : list.Count,
            };

            return result;
        }
    }
}
