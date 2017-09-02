using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService;
using ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Order/OrderQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryOrder(OrderQueryFilter query)
        {
         
            int totalCount;
            var dt = ObjectFactory<IOrdrQueryDA>.Instance.Query(query, out totalCount);
            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }
    }
}
