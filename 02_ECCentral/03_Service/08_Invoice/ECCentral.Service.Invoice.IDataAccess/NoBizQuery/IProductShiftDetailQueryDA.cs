using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IProductShiftDetailQueryDA
    {
        List<ProductShiftDetail> Query(ProductShiftDetailReportQueryFilter filter, out int totalCount
                                        , ref ProductShiftDetailAmtInfo outAmt, ref ProductShiftDetailAmtInfo inAmt
                                        , ref ProductShiftDetail needManualItem);
    }
}
