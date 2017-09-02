using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess.NoBizQuery
{
    public interface ILogQueryDA
    {
        DataTable QuerySysLogWithOutCancelOutStore(LogQueryFilter filter, out int totalCount);

        DataTable QuerySysLog(LogQueryFilter filter, out int totalCount);

        DataTable QuerySOLog(LogQueryFilter filter, out int totalCount);
    }
}
