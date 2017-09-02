using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRMARequestQueryDA
    {
        DataTable QueryRMARequest(RMARequestQueryFilter filter, out int totalCount);

        List<UserInfo> GetAllReceiveUsers();

        List<UserInfo> GetAllConfirmUsers();
    }
}
