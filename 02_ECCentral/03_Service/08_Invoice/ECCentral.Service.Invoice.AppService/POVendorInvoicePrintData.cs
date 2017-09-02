using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 供应商问题发票打印
    /// </summary>
    public class POVendorInvoicePrintData : IPrintDataBuild
    {
        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            if (requestPostData != null)
            {
                variables.Add("TotalAmount", HttpUtility.UrlDecode(requestPostData["TotalAmount"]));
            }
            int count;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("VendorName");
            dataTable.Columns.Add("InvoiceTime");
            dataTable.Columns.Add("InputTime");
            dataTable.Columns.Add("TotalAmt");
            dataTable.Columns.Add("InvoiceNumber");

            if (int.TryParse(requestPostData["InvoiceCount"], out count))
            {
                DataRow row;
                for (int i = 0; i < count; i++)
                {
                    row = dataTable.NewRow();
                    row["VendorName"] = HttpUtility.UrlDecode(requestPostData["V" + i]);
                    row["InvoiceTime"] = HttpUtility.UrlDecode(requestPostData["I" + i]);
                    row["InputTime"] = HttpUtility.UrlDecode(requestPostData["T" + i]);
                    row["TotalAmt"] = HttpUtility.UrlDecode(requestPostData["A" + i]);
                    row["InvoiceNumber"] = HttpUtility.UrlDecode(requestPostData["N" + i]);
                    dataTable.Rows.Add(row);
                }
            }
            tableVariables.Add("InvoiceDetailList", dataTable);
        }

        #endregion IPrintDataBuild Members
    }
}
