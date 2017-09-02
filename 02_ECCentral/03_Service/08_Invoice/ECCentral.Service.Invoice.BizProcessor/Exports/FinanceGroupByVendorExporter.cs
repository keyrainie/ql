using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.in2bits.MyXls;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.Exports
{
    public class FinanceGroupByVendorExporter : FinanceExporter
    {
        protected override void BuildSheet(Worksheet worksheet, int sheetIndex, DataTable data, List<ColumnData> columnSetting, List<TextInfo> textInfo)
        {
            CreateExcelColumns(worksheet, 10, 1, 2, 3, 5, 9, 15, 17);
            CreateExcelColumns(worksheet, 20, 6, 7, 8, 10, 11, 12, 13, 14, 16, 18, 19, 20);

            InitHeader(worksheet, sheetIndex);

            InitContentRows(worksheet, sheetIndex, data);
        }

        private void InitHeader(Worksheet worksheet, int sheetIndex)
        {
            int colIndex = 0;

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "供应商#", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "供应商", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "供应商属性", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "收款人帐号", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "开户银行", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "已到应付", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "帐期", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "一级分类", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "含税库存", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "总应付款", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "库存大于应付", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "90天以上滞销金额", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.Cells.Add(2, ++colIndex, "票扣", GetDataHeaderXF());
            worksheet.Cells.Add(2, ++colIndex, "帐扣", GetDataHeaderXF());
            worksheet.Cells.Add(2, ++colIndex, "PO单扣减", GetDataHeaderXF());
            worksheet.Cells.Add(2, ++colIndex, "代销结算", GetDataHeaderXF());
            worksheet.Cells.Add(2, ++colIndex, "现金", GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 1, colIndex - 4, colIndex));
            worksheet.Cells.Add(1, colIndex - 4, "总计未扣EIMS金额", GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "单据明细", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "EIMS草稿金额", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());

            worksheet.AddMergeArea(new MergeArea(1, 2, ++colIndex, colIndex));
            worksheet.Cells.Add(1, colIndex, "EIMS应计金额", GetDataHeaderXF());
            worksheet.Cells.Add(2, colIndex, string.Empty, GetDataHeaderXF());
        }

        private void InitContentRows(Worksheet worksheet, int sheetIndex, DataTable data)
        {
            List<FinanceExportInfo> list = new List<FinanceExportInfo>((int)(data.Rows.Count * 1.3));
            foreach (DataRow row in data.Rows)
            {
                FinanceExportInfo entity = FinanceExportInfo.Create(row);
                list.Add(entity);
            }

            ushort currentRowIndex = 2;
            ushort currentColIndex;

            list.ForEach(delegate(FinanceExportInfo item)
            {
                currentRowIndex++;
                currentColIndex = 0;

                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.VendorSysNo.ToString(), GetDataCenterCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.VendorName, GetDataLeftCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.IsConsignDisplay, GetDataLeftCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.AccountID, GetDataLeftCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.BankName, GetDataLeftCellXF());
                                
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.PayAmtMatureString, GetDataRightCellXF());

                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.VendorPayTypeDisplay, GetDataLeftCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.C1NameStr, GetDataLeftCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.KCAmtDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.PayAmtLeftString, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, (item.KCAmt - item.PayAmtLeft).ToString("###,###,###0.00"), GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ZXAmtDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ReceiveByInvoiceDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ReceiveByAcctDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ReceiveByPODisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ReceiveByConsignDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.CashDisplay, GetDataRightCellXF());

                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.DetailOrderSysNoStr, GetDataLeftCellXF());

                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.PendingInvoiceAmountDisplay, GetDataRightCellXF());
                worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.EndBalanceAccruedDisplay, GetDataRightCellXF());
            });
        }

    }
}
