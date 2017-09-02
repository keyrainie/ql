using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface ICustomerVisitQueryDA
    {
        DataTable Query(CustomerVisitQueryFilter req, out int totalCount);
    }
}
