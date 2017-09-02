using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using System.Data;
using ECCentral.BizEntity.NeweggCN.Invoice.SAP;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface INECN_SyncSAPSalesDA
    {
        void DeleteSAPSales(int orderTypeSysNo);

        void SyncSAPSales(SAPSalesInfo entity);

    }
}
