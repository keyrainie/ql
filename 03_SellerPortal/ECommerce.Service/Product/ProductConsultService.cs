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
    public class ProductConsultService
    {
        /// <summary>
        /// Query查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public static QueryResult<ProductConsultQueryBasicInfo> QueryProductConsultBasicInfoList(ProductConsultQueryFilter queryCriteria)
        {
            int totalCount = 0;
            QueryResult<ProductConsultQueryBasicInfo> result = new QueryResult<ProductConsultQueryBasicInfo>();

            List<ProductConsultQueryBasicInfo> list = ProductConsultDA.QueryProductConsultBasicInfoList(queryCriteria, out totalCount);

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
        public static void BatchSetProductConsultValid(List<int> items, string currentUser)
        {
            ProductConsultDA.BatchSetProductConsultStatus(items, "A", currentUser);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items">The items.</param>
        public static void BatchSetProductConsultInvalid(List<int> items, string currentUser)
        {
            ProductConsultDA.BatchSetProductConsultStatus(items, "D", currentUser);
        }

        /// <summary>
        /// 批量阅读
        /// </summary> 
        /// <param name="items">The items.</param>
        public static void BatchSetProductConsultRead(List<int> items, string currentUser)
        {
            ProductConsultDA.BatchSetProductConsultStatus(items, "E", currentUser);
        }
        /// <summary>
        /// 根据编号，加载相应的评咨询
        /// </summary>
        /// <param name="sysNo">The item system no.</param>
        /// <returns></returns>
        public static ProductConsultInfo LoadProductConsultWithoutReply(int sysNo)
        {
            ProductConsultInfo item = ProductConsultDA.LoadProductConsult(sysNo);

            return item;
        }
        /// <summary>
        /// 添加产品评论回复
        /// </summary>
        /// <param name="item">The item.</param>
        public static void AddProductConsultReply(ProductConsultReplyInfo item)
        {
            ProductConsultDA.AddProductConsultReply(item);
        }

        /// <summary>
        /// 根据咨询编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <param name="sellerSysNo">The seller system no.</param>
        /// <returns></returns>
        public static QueryResult<ProductConsultReplyInfo> GetProductConsultFactoryReply(int sysNo, int? sellerSysNo)
        {
            QueryResult<ProductConsultReplyInfo> result = new QueryResult<ProductConsultReplyInfo>();

            var list = ProductConsultDA.GetProductConsultFactoryReply(sysNo);
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
