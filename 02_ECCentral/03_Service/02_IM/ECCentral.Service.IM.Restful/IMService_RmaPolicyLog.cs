using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 查询退换货信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RmaPolicyLog/QueryRmaPolicyLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryRmaPolicyLog(RmaPolicyLogQueryFilter query)
        {

            int totalCount;
            var data = ObjectFactory<IRmaPolicyLogQueryDA>.Instance.GetRmaPolicyLog(query, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }
        //
    }
}
