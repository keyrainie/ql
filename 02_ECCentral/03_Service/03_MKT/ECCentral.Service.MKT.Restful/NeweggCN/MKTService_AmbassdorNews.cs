using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private AmbassadorNewsAppService _ambassadorNewsAppService = ObjectFactory<AmbassadorNewsAppService>.Instance;

        [WebInvoke(UriTemplate = "/AmbassadorNews/Query", Method = "POST")]
        public virtual QueryResult QueryAmbassadorNews(AmbassadorNewsQueryFilter filter)
        {
            int totalCount = 0;
            var ds = ObjectFactory<IAmbassadorNewsQueryDA>.Instance.Query(filter, out totalCount);
            var dtAmbassadorNews = ds.Tables[0];

            QueryResult queryResult = new QueryResult();
            queryResult.Data = dtAmbassadorNews;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        [WebInvoke(UriTemplate = "/AmbassadorNews/BatchUpdateAmbassadorNewsStatus", Method = "PUT")]
        public virtual void BatchUpdateAmbassadorNewsStatus(BizEntity.MKT.AmbassadorNewsBatchInfo batchInfo)
        {
            _ambassadorNewsAppService.BatchUpdateAmbassadorNewsStatus(batchInfo);
        }
    }
}
