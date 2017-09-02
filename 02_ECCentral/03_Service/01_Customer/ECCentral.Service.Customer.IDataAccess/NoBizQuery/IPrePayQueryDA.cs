using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IPrePayQueryDA
    {
        DataTable QueryPrePayLogIncome(PrePayQueryFilter filter, out int totalCount);
        DataTable QueryPrePayLogPayment(PrePayQueryFilter filter, out int totalCount);
    }
}
