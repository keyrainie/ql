using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using org.in2bits.MyXls;
using System.Data;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.Exports
{
    public class FinanceExporter : ExcelFileExporter
    {
        //protected Dictionary<string, XF> m_xlsXFs;

        //public FinanceExporter()
        //{
        //    m_xlsXFs = new Dictionary<string, XF>();
        //}

        protected override void BuildSheet(Worksheet worksheet, int sheetIndex, DataTable data, List<ColumnData> columnSetting, List<TextInfo> textInfo)
        {
            CreateExcelColumns(worksheet, 10, 1, 3, 4, 5, 9, 15, 16);
            CreateExcelColumns(worksheet, 30, 6, 7, 8, 10, 11, 12, 13, 14);
            CreateExcelColumns(worksheet, 40, 2);

            InitHeader(worksheet, sheetIndex);

            InitContentRows(worksheet, sheetIndex, data);
        }

        private void InitHeader(Worksheet worksheet, int sheetIndex)
        {
            int colIndex = 0;
            worksheet.Cells.Add(1, ++colIndex, "供应商#", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "供应商", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "汇总金额", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "单据ID", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "单据类型", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "PM", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "创建时间", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "预计付款日期", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "发票状态", GetDataHeaderXF());
            //worksheet.Cells.Add(1, ++colIndex, "90天以上滞销金额", GetDataHeaderXF());
            worksheet.Cells.Add(1, ++colIndex, "单据金额", GetDataHeaderXF());
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
            ushort currentColIndex = 0;
            var result = from p in list.AsQueryable()
                         group p by new { p.VendorSysNo, p.VendorName } into g
                         select new
                         {
                             g.Key.VendorSysNo,
                             g.Key.VendorName,
                             TotalAmt = g.Sum(x => x.OrderAmt).ToString("###,###,###0.00"),
                             Detail = g.Where(x => x.VendorSysNo == g.Key.VendorSysNo).ToList()
                         };
            foreach (var vendor in result)
            {
                currentColIndex = 0;

                ++currentColIndex;
                worksheet.AddMergeArea(new MergeArea(currentRowIndex, currentRowIndex + vendor.Detail.Count - 1, currentColIndex, currentColIndex));
                worksheet.Cells.Add(currentRowIndex, currentColIndex, vendor.VendorSysNo, GetDataCenterCellXF());

                ++currentColIndex;
                worksheet.AddMergeArea(new MergeArea(currentRowIndex, currentRowIndex + vendor.Detail.Count - 1, currentColIndex, currentColIndex));
                worksheet.Cells.Add(currentRowIndex, currentColIndex, vendor.VendorName, GetDataCenterCellXF());

                ++currentColIndex;
                worksheet.AddMergeArea(new MergeArea(currentRowIndex, currentRowIndex + vendor.Detail.Count - 1, currentColIndex, currentColIndex));
                worksheet.Cells.Add(currentRowIndex, currentColIndex, vendor.TotalAmt, GetDataCenterCellXF());
                bool flag = false;
                vendor.Detail.ForEach(delegate(FinanceExportInfo item)
                {
                    currentColIndex = 3;

                    if (flag)
                    {
                        currentColIndex = 0;
                        worksheet.Cells.Add(currentRowIndex, ++currentColIndex, string.Empty, GetDataLeftCellXF());
                        worksheet.Cells.Add(currentRowIndex, ++currentColIndex, string.Empty, GetDataLeftCellXF());
                        worksheet.Cells.Add(currentRowIndex, ++currentColIndex, string.Empty, GetDataLeftCellXF());
                    }

                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.OrderIDDisplay, GetDataLeftCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.OrderTypeDisplay, GetDataLeftCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.PMName, GetDataCenterCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.CreateTimeDisplay, GetDataCenterCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ETPDisplay, GetDataCenterCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.InvoiceStatusDisplay, GetDataCenterCellXF());
                    //worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.ZXAmtString, GetDataRightCellXF());
                    worksheet.Cells.Add(currentRowIndex, ++currentColIndex, item.OrderAmtDisplay, GetDataCenterCellXF());
                    ++currentRowIndex;
                    flag = true;
                });
            }
        }

        protected virtual XF GetDataHeaderXF()
        {
            string key = "XF_DataHeader_Key";
            if (!this.m_xlsXFs.ContainsKey(key))
            {
                XF xf = this.m_xlsDocument.NewXF();
                xf.HorizontalAlignment = HorizontalAlignments.Centered;
                xf.VerticalAlignment = VerticalAlignments.Centered;
                xf.Font.Height = (ushort)(10 * this.FONT_HEIGHT_SCALE);
                xf.Font.Weight = FontWeight.Bold;
                xf.Pattern = 1;
                xf.PatternColor = Colors.White;

                xf.LeftLineStyle = 1;
                xf.LeftLineColor = Colors.Black;
                xf.TopLineStyle = 1;
                xf.TopLineColor = Colors.Black;
                xf.RightLineStyle = 1;
                xf.RightLineColor = Colors.Black;
                xf.BottomLineStyle = 1;
                xf.BottomLineColor = Colors.Black;

                this.m_xlsXFs.Add(key, xf);
            }

            return this.m_xlsXFs[key];
        }

        protected virtual XF GetDataLeftCellXF()
        {
            string key = "XF_DateLeftCell_Key";
            if (!this.m_xlsXFs.ContainsKey(key))
            {
                XF xf = this.m_xlsDocument.NewXF();
                xf.HorizontalAlignment = HorizontalAlignments.Left;
                xf.Font.Height = (ushort)(10 * this.FONT_HEIGHT_SCALE);

                xf.LeftLineStyle = 1;
                xf.LeftLineColor = Colors.Black;
                xf.TopLineStyle = 1;
                xf.TopLineColor = Colors.Black;
                xf.RightLineStyle = 1;
                xf.RightLineColor = Colors.Black;
                xf.BottomLineStyle = 1;
                xf.BottomLineColor = Colors.Black;

                this.m_xlsXFs.Add(key, xf);
            }

            return this.m_xlsXFs[key];
        }

        protected virtual XF GetDataCenterCellXF()
        {
            string key = "XF_DateCenterCell_Key";
            if (!this.m_xlsXFs.ContainsKey(key))
            {
                XF xf = this.m_xlsDocument.NewXF();
                xf.HorizontalAlignment = HorizontalAlignments.Centered;
                xf.Font.Height = (ushort)(10 * this.FONT_HEIGHT_SCALE);

                xf.LeftLineStyle = 1;
                xf.LeftLineColor = Colors.Black;
                xf.TopLineStyle = 1;
                xf.TopLineColor = Colors.Black;
                xf.RightLineStyle = 1;
                xf.RightLineColor = Colors.Black;
                xf.BottomLineStyle = 1;
                xf.BottomLineColor = Colors.Black;

                this.m_xlsXFs.Add(key, xf);
            }

            return this.m_xlsXFs[key];
        }

        protected XF GetDataRightCellXF()
        {
            string key = "XF_DateRightCell_Key";
            if (!this.m_xlsXFs.ContainsKey(key))
            {
                XF xf = this.m_xlsDocument.NewXF();
                xf.HorizontalAlignment = HorizontalAlignments.Right;
                xf.Font.Height = (ushort)(10 * this.FONT_HEIGHT_SCALE);

                xf.LeftLineStyle = 1;
                xf.LeftLineColor = Colors.Black;
                xf.TopLineStyle = 1;
                xf.TopLineColor = Colors.Black;
                xf.RightLineStyle = 1;
                xf.RightLineColor = Colors.Black;
                xf.BottomLineStyle = 1;
                xf.BottomLineColor = Colors.Black;

                this.m_xlsXFs.Add(key, xf);
            }

            return this.m_xlsXFs[key];
        }

        protected void CreateExcelColumns(Worksheet worksheet, ushort width, params ushort[] colIndexs)
        {
            if (colIndexs.Length > 0)
            {
                ColumnInfo col;

                for (int i = 0; i < colIndexs.Length; i++)
                {
                    col = new ColumnInfo(this.m_xlsDocument, worksheet);
                    col.Width = (ushort)(width * this.COLUMN_WIDTH_SCALE);
                    col.ColumnIndexStart = colIndexs[i];
                    col.ColumnIndexEnd = colIndexs[i];

                    worksheet.AddColumnInfo(col);
                }
            }
        }

    }
}
