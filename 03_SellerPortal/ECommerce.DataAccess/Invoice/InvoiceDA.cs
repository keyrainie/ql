using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Invoice
{
    public class InvoiceDA
    {
        public static int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SOOutStockInvoiceSync");
            cmd.SetParameterValue("@SONumber", soSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo.ToString());
            cmd.SetParameterValue("@InvoiceNo", invoiceNo);
            cmd.SetParameterValue("@CreateDate", DateTime.Now);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteNonQuery();
        }

        public static bool ExistInvoiceMaster(int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ExistsInvoiceMaster");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            var res = cmd.ExecuteScalar();
            int i = 0;
            return res == null || !int.TryParse(res.ToString(), out i) ? false : true;
        }
    }
}
