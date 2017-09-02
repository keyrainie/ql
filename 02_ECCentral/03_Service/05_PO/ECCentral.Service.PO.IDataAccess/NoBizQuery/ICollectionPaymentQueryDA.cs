using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface ICollectionPaymentQueryDA
    {
        DataTable QueryCollectionPayment(CollectionPaymentFilter queryFilter, out int totalCount);

        //DataTable GetConsignSettlmentProductList(ConsignSettlementProductsQueryFilter filter, out int totalCount);
    }
}
