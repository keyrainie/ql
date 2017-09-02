using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.QueryFilter.IM;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 根据query得到商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductSalesAreaBatch/GetProductByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductByQuery(ProductSalesAreaBatchQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductSalesAreaBatchDA>.Instance.GetProductByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/ProductSalesAreaBatch/GetAllProvince", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetAllProvince()
        {
               var dataTable = ObjectFactory<IProductSalesAreaBatchDA>.Instance.GetAllProvince();
               return new QueryResult()
               {
                   TotalCount=0,
                   Data=dataTable
               };
        }
        /// <summary>
        /// 根据query得到有设置区域的商品
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductSalesAreaBatch/GetProductSalesAreaBatchList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductSalesAreaBatchList(ProductSalesAreaBatchQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductSalesAreaBatchDA>.Instance.GetProductSalesAreaBatchList(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 批量移除
        /// </summary>
        /// <param name="listInfo"></param>
        [WebInvoke(UriTemplate = "/ProductSalesAreaBatch/RemoveItemSalesAreaListBatch", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void RemoveItemSalesAreaListBatch(List<ProductSalesAreaBatchInfo> listInfo)
        {
            ObjectFactory<ProductSalesAreaBatchAppService>.Instance.RemoveItemSalesAreaListBatch(listInfo);
        }

        
        /// <summary>
        /// 移除省份
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductSalesAreaBatch/RemoveProvince", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void RemoveProvince(ProductSalesAreaBatchInfo info)
        {
            ObjectFactory<ProductSalesAreaBatchAppService>.Instance.RemoveProvince(info);
        }

    }
}
