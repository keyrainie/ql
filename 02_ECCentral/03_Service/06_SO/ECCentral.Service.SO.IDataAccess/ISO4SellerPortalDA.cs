using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISO4SellerPortalDA
    {
        int UpdateSOSellerStatus(SalesOrderStatusEntity soStatusEntity, string companyCode);

        int UpdateSOMasterInvoiceNo(SOInvoicePrintedSalesOrder sOInvoicePrintedSalesOrder, string companyCode);
    }
}
