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

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 根据query得到相关商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductRelated/GetProductRelatedByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductRelatedByQuery(ProductRelatedQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductRelatedDA>.Instance.GetProductRelatedByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// Create ItemRelated
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductRelated/CreateProductRelated", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual ProductRelatedInfo CreateProductRelated(ProductRelatedInfo info)
        {
            var entity = ObjectFactory<ProductRelateAppService>.Instance.CreateProductRelated(info);
            return entity;
        }
       
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/ProductRelated/DeleteProductRelated", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public virtual void DeleteProductRelated(List<string> list)
        {
                ObjectFactory<ProductRelateAppService>.Instance.DeleteItemRelated(list);
           
        }
        [WebInvoke(UriTemplate = "/ProductRelated/UpdateProductRelatePriority", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void UpdateProductRelatePriority(List<ProductRelatedInfo> list)
        {
            ObjectFactory<ProductRelateAppService>.Instance.UpdateProductRelatePriority(list);
        }

        /// <summary>
        /// 批量设置相关商品
        /// </summary>
        /// <param name="listInfo"></param>
        [WebInvoke(UriTemplate = "/ProductRelated/CreateItemRelatedByList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void CreateItemRelatedByList(List<ProductRelatedInfo> listInfo)
        {
            ObjectFactory<ProductRelateAppService>.Instance.CreateItemRelatedByList(listInfo);
        }
    }
}
