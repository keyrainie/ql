using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IGiftQueryDA
    {
        DataTable Query(CustomerGiftQueryFilter entity, out int totalCount);
    }
}
