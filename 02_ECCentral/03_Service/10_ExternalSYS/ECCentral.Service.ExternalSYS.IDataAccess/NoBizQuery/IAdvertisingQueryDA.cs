using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.ExternalSYS;
using System.Data;

namespace ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery
{
    public interface IAdvertisingQueryDA
    {
        DataTable Query(AdvertisingQueryFilter filter, out int totalCount);
    }
}
