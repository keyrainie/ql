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
        /// 根据query得到商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductExt/GetProductExtByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductExtByQuery(ProductExtQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductExtDA>.Instance.GetProductExtByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
       /// <summary>
       /// 批量设置是否可以退货
       /// </summary>
       /// <param name="list"></param>
       /// <param name="IsPermitRefund"></param>
        [WebInvoke(UriTemplate = "/ProductExt/UpdatePermitRefund", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void UpdatePermitRefund(List<ProductExtInfo> list)
        {
            ObjectFactory<ProductExtAppService>.Instance.UpdatePermitRefund(list);
        }

        [WebInvoke(UriTemplate = "/ProductExt/UpdateBatchManagementInfo", Method = "PUT")]
        public virtual ProductBatchManagementInfo UpdateIsBatch(ProductBatchManagementInfo productExtInfo)
        {
            return ObjectFactory<ProductExtAppService>.Instance.UpdateIsBatch(productExtInfo);
        }
    }
}
