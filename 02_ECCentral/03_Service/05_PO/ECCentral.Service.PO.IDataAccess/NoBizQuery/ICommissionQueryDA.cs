using System.Collections.Generic;
using System.Data;

using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface ICommissionQueryDA
    {
        DataTable QueryCommission(CommissionQueryFilter queryFilter, out int totalCount, out decimal totalAmt);

        DataTable QueryCommissionRules(CommissionQueryFilter queryFilter, out int totalCount);

        decimal QueryCommissionTotalAmt(QueryFilter.PO.CommissionQueryFilter queryFilter);
    }
}
