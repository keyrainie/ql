using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.RMA.IDataAccess.NoBizQuery
{
    public interface IRMATrackingQueryDA
    {
        List<UserInfo> GetRMATrackingCreateUsers();

        List<UserInfo> GetRMATrackingUpdateUsers();

        List<UserInfo> GetRMATrackingHandleUsers();

        DataTable QueryRMATracking(RMATrackingQueryFilter filter, out int totalCount);

    }
}
