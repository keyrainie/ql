using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IShipTypeSMSQueryDA
    {
        DataTable Query(ShipTypeSMSQueryFilter filter, out int totalCount);
    }
}
