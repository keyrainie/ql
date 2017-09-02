using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Collections.Specialized;
using System.Web;
using System.Data;

namespace ECCentral.Service.Invoice.AppService
{
    public class OldChangeNewPrintData :IPrintDataBuild
    {
        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            //if (requestPostData != null)
            //{
            //    variables.Add("TotalCount", HttpUtility.UrlDecode("TotalCount"));
            //}
            int count;
            DataTable dt = new DataTable();
            dt.Columns.Add("SysNo");
            dt.Columns.Add("TradeInId");
            dt.Columns.Add("SoSysNo");
            dt.Columns.Add("CustomerSysNo");
            dt.Columns.Add("CustomerName");
            dt.Columns.Add("ReceiveAddress");
            dt.Columns.Add("receivePhone");
            dt.Columns.Add("Licence");
            dt.Columns.Add("Rebate");
            dt.Columns.Add("ReviseRebate");
            dt.Columns.Add("BankName");
            dt.Columns.Add("BranchBankName");
            dt.Columns.Add("BankAccount");
            dt.Columns.Add("Status");
            dt.Columns.Add("InUser");
            dt.Columns.Add("InDateStr");
            dt.Columns.Add("ConfirmUser");
            dt.Columns.Add("ConfirmDateStr");

            if (int.TryParse(requestPostData["TotalCount"], out count))
            {
                DataRow row;
                for (int i = 0; i < count; i++)
                {
                    row = dt.NewRow();
                    row["SysNo"] = HttpUtility.UrlDecode(requestPostData["SysNo" + i]);
                    row["TradeInId"] = HttpUtility.UrlDecode(requestPostData["TradeInId" + i]);
                    row["SoSysNo"] = HttpUtility.UrlDecode(requestPostData["SoSysNo" + i]);
                    row["CustomerSysNo"] = HttpUtility.UrlDecode(requestPostData["CustomerSysNo" + i]);
                    row["CustomerName"] = HttpUtility.UrlDecode(requestPostData["CustomerName" + i]);
                    row["ReceiveAddress"] = HttpUtility.UrlDecode(requestPostData["ReceiveAddress" + i]);
                    row["receivePhone"] = HttpUtility.UrlDecode(requestPostData["receivePhone" + i]);
                    row["Licence"] = HttpUtility.UrlDecode(requestPostData["Licence" + i]);
                    row["Rebate"] = HttpUtility.UrlDecode(requestPostData["Rebate" + i]);
                    row["ReviseRebate"] = HttpUtility.UrlDecode(requestPostData["ReviseRebate" + i]);
                    row["BankName"] = HttpUtility.UrlDecode(requestPostData["BankName" + i]);
                    row["BranchBankName"] = HttpUtility.UrlDecode(requestPostData["BranchBankName" + i]);
                    row["BankAccount"] = HttpUtility.UrlDecode(requestPostData["BankAccount" + i]);
                    row["Status"] = HttpUtility.UrlDecode(requestPostData["Status" + i]);
                    row["InUser"] = HttpUtility.UrlDecode(requestPostData["InUser" + i]);
                    row["InDateStr"] = HttpUtility.UrlDecode(requestPostData["InDateStr" + i]);
                    row["ConfirmUser"] = HttpUtility.UrlDecode(requestPostData["ConfirmUser" + i]);
                    row["ConfirmDateStr"] = HttpUtility.UrlDecode(requestPostData["ConfirmDateStr" + i]);
                    dt.Rows.Add(row);
                }
            }

            tableVariables.Add("OldChangeNewList", dt);
        }
    }
}
