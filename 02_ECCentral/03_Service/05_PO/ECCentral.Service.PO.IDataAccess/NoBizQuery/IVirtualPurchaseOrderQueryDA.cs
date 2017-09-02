using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IVirtualPurchaseOrderQueryDA
    {
        DataTable QueryVirtualPurchaseOrderList(VirtualPurchaseOrderQueryFilter queryFilter, out int totalCount);
    }
}
