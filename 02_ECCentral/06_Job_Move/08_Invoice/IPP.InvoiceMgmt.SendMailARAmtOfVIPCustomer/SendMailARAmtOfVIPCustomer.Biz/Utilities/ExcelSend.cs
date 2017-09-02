using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SendMailARAmtOfVIPCustomer.Biz.Entities;

namespace SendMailARAmtOfVIPCustomer.Biz.Utilities
{
    public class ExcelSend
    {
        HSSFWorkbook hssWorkBook = new HSSFWorkbook();
        Sheet sheet;
        int i = 0;
        int j = 0;

        public byte[] Getbytes(List<ARAmtOfVIPCustomerEntity> list)
        {
            SetHead();
            SetContent(list);

            MemoryStream stream = new MemoryStream();
            hssWorkBook.Write(stream);
            stream.Position = 0;

            return stream.GetBuffer();
        }

        public string WriteToFile(List<ARAmtOfVIPCustomerEntity> list, string fileName)
        {
            hssWorkBook = new HSSFWorkbook();
            SetHead();
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

        private void SetHead()
        {
            sheet = hssWorkBook.CreateSheet();
            sheet.DefaultColumnWidth = 18;

            i = 0;
            //BuildHeaderCell("发送日期:", nowTime.ToString(Settings.LongDateFormat));
            //BuildHeaderCell("总记录数:", count.ToString());

            j = 0;
            Row rowHead = sheet.CreateRow(j++);

            BuildFormatCell<string>(rowHead, "VIP客户系统编号", CenterCell);
            BuildFormatCell<string>(rowHead, "VIP客户ID", CenterCell);
            BuildFormatCell<string>(rowHead, "VIP客户名称", CenterCell);
            BuildFormatCell<string>(rowHead, "VIP客户应收未收款", CenterCell);
        }

        private void SetContent(List<ARAmtOfVIPCustomerEntity> list)
        {
            i = 0;
            foreach (var item in list)
            {
                Row row = sheet.CreateRow(j++);
                row.CreateCell(i++).SetCellValue(item.SystemNumber.ToString());
                row.CreateCell(i++).SetCellValue(item.CustomerID);
                row.CreateCell(i++).SetCellValue(item.CustomerName);
                row.CreateCell(i++).SetCellValue(Convert.ToDouble(item.ArAMT));

                i = 0;
            }
        }

        private void BuildFormatCell<T>(Row row, T value, CellStyle style)
        {
            var formatCell = row.CreateCell(i++);
            if (value is decimal)
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
