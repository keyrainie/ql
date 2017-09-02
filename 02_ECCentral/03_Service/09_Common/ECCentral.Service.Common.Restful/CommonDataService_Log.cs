using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Common;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/Log/QuerySysLogWithOutCancelOutStore", Method = "POST")]
        public QueryResult QuerySysLogWithOutCancelOutStore(LogQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ILogQueryDA>.Instance.QuerySysLogWithOutCancelOutStore(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Log/QuerySysLog", Method = "POST")]
        public QueryResult QuerySysLog(LogQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ILogQueryDA>.Instance.QuerySysLog(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Log/QuerySOLog", Method = "POST")]
        public QueryResult QuerySOLog(LogQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ILogQueryDA>.Instance.QuerySOLog(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

    }
}
