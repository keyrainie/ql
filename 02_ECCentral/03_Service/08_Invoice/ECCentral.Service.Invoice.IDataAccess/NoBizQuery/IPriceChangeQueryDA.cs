using ECCentral.QueryFilter.Invoice;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IPriceChangeQueryDA
    {
        DataTable QueryPriceChange(ChangePriceFilter filter, out int TotalCount);

        DataTable QUeryLastVendorSysNo();
    }

}
