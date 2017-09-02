using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.RMA;

namespace ECCentral.Service.RMA.IDataAccess.NoBizQuery
{
    public interface IReportQueryDA
    {
        DataTable QueryProductCardInventoryByProductSysNo(int productSysNo);

        DataTable QueryProductCardsByProductSysNo(int productSysNo);

        DataTable QueryOutBoundNotReturn(OutBoundNotReturnQueryFilter request, out int totalCount);

        DataTable QueryRMAProductInventory(RMAInventoryQueryFilter request, out int totalCount, out decimal totalMisCost);

        DataTable QueryRMAItemInventory(RMAInventoryQueryFilter request, out int totalCount);
    }
}
