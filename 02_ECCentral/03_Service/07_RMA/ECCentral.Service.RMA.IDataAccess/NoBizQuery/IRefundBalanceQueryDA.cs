using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.RMA.IDataAccess.NoBizQuery
{
    public interface IRefundBalanceQueryDA
    {
        DataTable Query(QueryFilter.RMA.RefundBalanceQueryFilter queryFilter, out int totalCount);
    }
}
