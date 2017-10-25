using ECCentral.QueryFilter.Customer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface ICommissionTypeQueryDA
    {
        DataTable QueryCommissionType(CommissionTypeQueryFilter filter, out int totalCount);

        DataTable SocietyCommissionQuery(CommissionTypeQueryFilter filter, out int totalCount);
    }
}
