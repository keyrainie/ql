using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Invoice.AppService;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {

        [WebInvoke(UriTemplate = "/SyncSAPSales/SOIncome", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void SyncSAPSales(SOIncomeQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ISOIncomeQueryDA>.Instance.Query(request, out totalCount);
            DataTable dt = this.QuerySOIncome(request)[0].Data;
            if (dt != null && dt.Rows.Count > 0)
            {
               ObjectFactory<NECN_SyncSAPSalesAPPService>.Instance.SyncSAPSales(dt, request.OrderType.Value);
            }
        }
    }
}
