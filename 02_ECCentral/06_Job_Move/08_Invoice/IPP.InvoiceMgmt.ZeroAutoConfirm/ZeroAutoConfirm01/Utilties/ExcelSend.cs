using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using ZeroAutoConfirm.Model;
using NPOI.SS.UserModel;
using System.IO;

namespace ZeroAutoConfirm.Utilties
{
    public class ExcelSend
    {
        HSSFWorkbook hssWorkBook = new HSSFWorkbook();
        Sheet sheet;
        int i = 0;
        int j = 0;

        public byte[] Getbytes(List<ConfirmEntity> list, DateTime nowTime)
        {
            SetHead(nowTime);
            SetContent(list);

            MemoryStream stream = new MemoryStream();
            hssWorkBook.Write(stream);
            stream.Position = 0;

            return stream.GetBuffer();
        }

        public string WriteToFile(List<ConfirmEntity> list, string fileName, DateTime nowTime)
        {
            hssWorkBook = new HSSFWorkbook();
            SetHead(nowTime);
            SetContent(list);

            string targetPath = AppDomain.CurrentDomain.BaseDirectory + "Report\\";
            string filePath = targetPath + fileName;
            if (!System.IO.Directory.Exists(targetPath))
                System.IO.Directory.CreateDirectory(targetPath);

            FileStream file = new FileStream(filePath, FileMode.Create);
            hssWorkBook.Write(file);
            file.Close();

            return filePath;
        }

        private void SetHead(DateTime nowTime)
        {
            sheet = hssWorkBook.CreateSheet("SuccessInfo");
            sheet.DefaultColumnWidth = 18;

            i = 0;

            Row rowHead = sheet.CreateRow(j++);

            BuildFormatCell<string>(rowHead, "单据类型", CenterCell);
            BuildFormatCell<string>(rowHead, "单据编号", CenterCell);
            BuildFormatCell<string>(rowHead, "支付方式", CenterCell);
            BuildFormatCell<string>(rowHead, "单据金额", CenterCell);
            BuildFormatCell<string>(rowHead, "实收金额", CenterCell);
            BuildFormatCell<string>(rowHead, "礼品卡金额", CenterCell);
            BuildFormatCell<string>(rowHead, "预收金额", CenterCell);
            BuildFormatCell<string>(rowHead, "确认时间", CenterCell);
            BuildFormatCell<string>(rowHead, "确认人ID", CenterCell);
            BuildFormatCell<string>(rowHead, "确认结果", CenterCell);
        }

        private void SetContent(List<ConfirmEntity> list)
        {
            i = 0;
            foreach (var item in list)
            {
                Row row = sheet.CreateRow(j++);
                row.CreateCell(i++).SetCellValue("SO");
                row.CreateCell(i++).SetCellValue(item.SoSysNo);
                row.CreateCell(i++).SetCellValue(item.PayTerms);
                BuildFormatCell<decimal>(row, item.OrderAmt, DecimalCell);
                BuildFormatCell<decimal>(row, item.IncomeAmt, DecimalCell);
                BuildFormatCell<decimal>(row, item.GiftCardPayAmt, DecimalCell);
                BuildFormatCell<decimal>(row, item.PrepayAmt, DecimalCell);
                row.CreateCell(i++).SetCellValue(item.ConfirmedDate.ToString(Settings.LongDateFormat));
                row.CreateCell(i++).SetCellValue(item.CofirmedID);
                row.CreateCell(i++).SetCellValue(item.ConfirmedInfo);

                i = 0;
            }
        }

        private void BuildFormatCell<T>(Row row, T value, CellStyle style)
        {
            var formatCell = row.CreateCell(i++);
            Type sourceType = typeof(T);
            if (sourceType == typeof(decimal))
            {
                formatCell.SetCellValue(Convert.ToDouble(Math.Round(Convert.ToDecimal(value.ToString()), 2)));
            }
            else
            {
                formatCell.SetCellValue(value as string);
            }
            formatCell.CellStyle = style;
        }

        private void BuildHeaderCell<T>(string headerText, T headerValue)
        {
            CellStyle cellHeaderStyle = hssWorkBook.CreateCellStyle();
            cellHeaderStyle.Alignment = HorizontalAlignment.RIGHT;
            CellStyle cellValueStyle = hssWorkBook.CreateCellStyle();
            cellValueStyle.Alignment = HorizontalAlignment.LEFT;

            var row = sheet.CreateRow(j++);

            var header = row.CreateCell(i);
            header.SetCellValue(headerText);
            header.CellStyle = cellHeaderStyle;

            var value = row.CreateCell(i + 1);
            value.SetCellValue(headerValue.ToString());
            value.CellStyle = cellValueStyle;
        }

        private CellStyle DecimalCell
        {
            get
            {
                CellStyle cellStyle = hssWorkBook.CreateCellStyle();
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                return cellStyle;
            }
        }

        private CellStyle CenterCell
        {
            get
            {
                CellStyle cellStyle = hssWorkBook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.CENTER;

                return cellStyle;
            }
        }
    }
}
