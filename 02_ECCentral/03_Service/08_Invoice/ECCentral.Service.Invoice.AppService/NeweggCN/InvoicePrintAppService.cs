using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Collections.Specialized;
using ECCentral.BizEntity;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using System.Data;

namespace ECCentral.Service.Invoice.AppService
{
    public class InvoicePrintAppService : IPrintDataBuild
    {
        private static readonly int MaxItemRowsCount = 14;

        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            int soSysNo = 0;
            int warhouseNumber = 0;
            string param = requestPostData["SOSysNo"];
            string param1 = requestPostData["WareHouseNumber"];
            SOInvoiceInfo soInvoice = null;
            bool hasError = false;

            DataTable table = CreatePrintData();
            DataTable errorTable = new DataTable();
            try
            {
                if (!int.TryParse(param, out soSysNo))
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("Invoice.InvoicePrint", "InvoicePrint_IllegalOrderID"), param));
                }
                if (!int.TryParse(param1, out warhouseNumber))
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("Invoice.InvoicePrint", "InvoicePrint_IllegalStockID"), param1));
                }

                soInvoice = ObjectFactory<InvoiceReportProcessor>.Instance.GetNew(new SOInvoiceInfo
                {
                    SOInfo = new SOInfo
                    {
                        SOSysNo = soSysNo,
                        StockSysNo = warhouseNumber
                    },
                    InvoiceInfoList = new List<InvoiceInfo>()
                });
            }
            catch (BizException ex)
            {
                hasError = true;
                InitErrorTable(ex, errorTable);
            }

            if (!hasError)
            {
                InitData(soInvoice, table);
            }
            tableVariables.Add("Error", errorTable);
            tableVariables.Add("PrintInvoiceDetail", table);
        }

        private void InitData(SOInvoiceInfo soInvoice, DataTable table)
        {
            int rowCount = soInvoice.InvoiceInfoList.Count;
            DataRow row = null;
            InvoiceInfo invoice = null;
            for (int i = 0; i < rowCount; i++)
            {
                invoice = soInvoice.InvoiceInfoList[i];
                row = table.NewRow();
                row["InvoiceDate"] = invoice.InvoiceDate;
                row["InvoiceCurPageNum"] = invoice.InvoiceCurPageNum;
                row["InvoiceSumPageNum"] = invoice.InvoiceSumPageNum;
                row["ReceiveName"] = invoice.ReceiveName;
                row["ReceiveContact"] = invoice.ReceiveContact;
                row["ReceiveAddress1"] = invoice.ReceiveAddress1;
                row["ReceiveAddress2"] = invoice.ReceiveAddress2;
                row["ReceivePhone"] = invoice.ReceivePhone;
                row["InvoiceNote"] = invoice.InvoiceNote;
                row["CustomerSysNo"] = invoice.CustomerSysNo;
                row["PayTypeName"] = invoice.PayTypeName;
                row["InvoiceSeqEx"] = invoice.InvoiceSeqEx;
                row["SOSysNo"] = invoice.SOSysNo;
                row["TotalWeight"] = invoice.TotalWeight == 0 ? string.Empty : invoice.TotalWeight.ToString();
                row["ServicePhone"] = string.IsNullOrWhiteSpace(invoice.ServicePhone) ? "400-820-4400" : invoice.ServicePhone;
                row["Importance"] = invoice.Importance;
                row["RMBConvert"] = invoice.RMBConvert;
                row["ShipTypeName"] = invoice.ShipTypeName;

                //发票明细
                DataTable itemTable = CreateItemsPrintData();
                InitItemData(invoice.Items, itemTable);

                DataTable otherTable = CreateOtherPrintData();
                InitOtherData(invoice.Items, otherTable);

                row["Items"] = itemTable;
                row["Others"] = otherTable;
                table.Rows.Add(row);
            }

        }

        private void InitOtherData(List<InvoiceItem> list, DataTable otherTable)
        {
            int rowCount = MaxItemRowsCount - list.Count;
            DataRow row = null;
            for (int i = 0; i < rowCount; i++)
            {
                row = otherTable.NewRow();
                otherTable.Rows.Add(row);
            }
        }

        private DataTable CreateOtherPrintData()
        {
            return new DataTable();
        }

        private DataTable CreatePrintData()
        {
            DataTable table = new DataTable();

            table.Columns.Add("InvoiceDate");
            table.Columns.Add("InvoiceCurPageNum");
            table.Columns.Add("InvoiceSumPageNum");
            table.Columns.Add("ReceiveName");
            table.Columns.Add("ReceiveContact");
            table.Columns.Add("ReceiveAddress1");
            table.Columns.Add("ReceiveAddress2");
            table.Columns.Add("ReceivePhone");
            table.Columns.Add("InvoiceNote");
            table.Columns.Add("CustomerSysNo");
            table.Columns.Add("PayTypeName");
            table.Columns.Add("InvoiceSeqEx");
            table.Columns.Add("SOSysNo");
            table.Columns.Add("TotalWeight");
            table.Columns.Add("ServicePhone");
            table.Columns.Add("Importance");
            table.Columns.Add("RMBConvert");
            table.Columns.Add("ShipTypeName");

            //发票明细
            table.Columns.Add("Items", typeof(DataTable));
            table.Columns.Add("Others", typeof(DataTable));

            return table;
        }

        private DataTable CreateItemsPrintData()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ItemNumber");
            table.Columns.Add("Description");
            table.Columns.Add("UnitPrice");
            table.Columns.Add("Quantity");
            table.Columns.Add("SumExtendPriceEx");
            table.Columns.Add("RepairWarrantyDays");
            return table;
        }

        private void InitItemData(List<InvoiceItem> items, DataTable table)
        {
            int rowCount = items.Count;
            DataRow row = null;
            InvoiceItem item = null;
            for (int i = 0; i < rowCount; i++)
            {
                item = items[i];
                row = table.NewRow();
                row["ItemNumber"] = item.ItemNumber;
                row["Description"] = item.Description;
                row["UnitPrice"] = item.Quantity == 0 && item.UnitPrice == 0M ? string.Empty : item.UnitPrice.ToString("#########0.00");
                row["Quantity"] = item.Quantity == 0 ? string.Empty : item.Quantity.ToString();
                row["SumExtendPriceEx"] = item.SumExtendPriceEx;
                row["RepairWarrantyDays"] = item.RepairWarrantyDays;
                table.Rows.Add(row);
            }
        }

        private void InitErrorTable(BizException ex, DataTable errorTable)
        {
            if (!errorTable.Columns.Contains("ErrorMessage"))
            {
                errorTable.Columns.Add("ErrorMessage");
            }
            DataRow row = errorTable.NewRow();
            row["ErrorMessage"] = ex.Message;
            errorTable.Rows.Add(row);
        }
        #endregion
    }
}
