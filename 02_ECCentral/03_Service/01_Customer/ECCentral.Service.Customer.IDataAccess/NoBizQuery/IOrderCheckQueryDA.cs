using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IOrderCheckMasterQueryDA
    {
        DataTable Query(OrderCheckMasterQueryFilter queryCriteria, out int totalCount);
    }
    public interface IOrderCheckItemQueryDA 
    {
        DataTable Query(OrderCheckItemQueryFilter queryCriteria, out int totalCount);
    }
}
