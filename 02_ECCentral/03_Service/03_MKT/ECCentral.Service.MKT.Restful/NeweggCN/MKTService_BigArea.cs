using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/BigArea/Query", Method = "POST")]
        public virtual QueryResult GetAllBigAreas(BigAreaQueryFilter filter)
        {
            int totalCount = 0;
            var ds = ObjectFactory<IBigAreaQueryDA>.Instance.GetAllBigAreas(filter);
            var dtResult = ds.Tables[0];

            QueryResult queryResult = new QueryResult();
            queryResult.Data = dtResult;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }
    }
}
