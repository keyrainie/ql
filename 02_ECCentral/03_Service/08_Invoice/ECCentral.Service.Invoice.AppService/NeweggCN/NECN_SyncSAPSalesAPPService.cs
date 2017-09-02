using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(NECN_SyncSAPSalesAPPService))]
    public class NECN_SyncSAPSalesAPPService
    {
        public void SyncSAPSales(System.Data.DataTable dt, BizEntity.Invoice.SOIncomeOrderType sOIncomeOrderType)
        {
            ObjectFactory<NECN_SyncSAPSalesProcessor>.Instance.SyncSAPSales(dt, sOIncomeOrderType);
        }
    }
}
