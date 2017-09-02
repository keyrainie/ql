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
        /// 根据query得到商品上架信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductShow/GetProductShowByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductShowByQuery(ProductShowQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductShowDA>.Instance.GetProductShowByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
    }
}
