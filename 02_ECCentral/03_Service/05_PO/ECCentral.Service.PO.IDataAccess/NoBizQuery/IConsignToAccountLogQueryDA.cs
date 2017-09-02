using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IConsignToAccountLogQueryDA
    {
        DataTable QueryConsignToAccountLog(ConsignToAccountLogQueryFilter queryFilter, out int totalCount);

        DataTable QueryConsignToAccountLogTotalAmt(ConsignToAccountLogQueryFilter queryFilter);
    }
}
