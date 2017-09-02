using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using CustomReport.Model;
using System.IO;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
namespace CustomReport.Biz
{
    public class ExcelManage
    {
        HSSFWorkbook hssWorkBook = new HSSFWorkbook();
        
        public string WriteExcel(List<CustomerInfo> list )
        {
            string reportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder);
            }
            string reportFile = Path.Combine(reportFolder, string.Format("CustomerReportInfo-{0:yyyyMMddHHmmss}.xls", DateTime.Now));
            List<List<CustomerInfo>> lists = new List<List<CustomerInfo>>();

            for (int k = 0; k < list.Count; k++)
            {
                if (k % 50000 == 0)
                {
                    lists.Add(new List<CustomerInfo>());
                }

                lists[lists.Count -1].Add(list[k]);
            }

            for (int i = 0; i < lists.Count;i++ )
            {
                Sheet sheet = hssWorkBook.CreateSheet("CustomerInfo"+i.ToString());
                SetHead(sheet);
                SetCoutent(lists[i], sheet);
            }

            using (FileStream stream = new FileStream(reportFile, FileMode.Create))
            {
                hssWorkBook.Write(stream);
            }

            hssWorkBook.Dispose();

            return reportFile;
        }
        private void SetHead(Sheet sheet)
        {
           
            //sheet = hssWorkBook.CreateSheet("CustomerInfo");
           
            Row rowHead = sheet.CreateRow(0);
            Cell cell=rowHead.CreateCell(0);
            cell.SetCellValue("新客户注册统计表");
            Cell cellhead1 = rowHead.CreateCell(1);
            Cell cellhead2 = rowHead.CreateCell(2);
            Cell cellhead3 = rowHead.CreateCell(3);
            sheet.AddMergedRegion(new  CellRangeAddress(0,0,0,3));

            NPOI.SS.UserModel.CellStyle style = hssWorkBook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            Font font = hssWorkBook.CreateFont();
            font.FontName = "黑体";
            font.Boldweight = 700;


            font.FontHeightInPoints = 14;
            style.SetFont(font);

            //style.BorderBottom = CellBorderType.THIN;
            //style.BorderLeft = CellBorderType.THIN;
            //style.BorderRight = CellBorderType.THIN;
            //style.BorderTop = CellBorderType.THIN;

            cell.CellStyle = style;
            cellhead1.CellStyle = style;
            cellhead2.CellStyle = style;
            cellhead3.CellStyle = style;
            rowHead.Height = 2*256;


            NPOI.SS.UserModel.CellStyle stylefirst = hssWorkBook.CreateCellStyle();
            stylefirst.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            Font fontfirst = hssWorkBook.CreateFont();
            fontfirst.FontName = "宋体";
            fontfirst.FontHeightInPoints = 10;
            fontfirst.Boldweight = 700;
            stylefirst.SetFont(fontfirst);

            //stylefirst.BorderBottom = CellBorderType.THIN;
            //stylefirst.BorderLeft = CellBorderType.THIN;
            //stylefirst.BorderRight = CellBorderType.THIN;
            //stylefirst.BorderTop = CellBorderType.THIN;
          //  rowHead.Height = 1 * 256;

            Row rowfirst = sheet.CreateRow(1);
            rowfirst.CreateCell(0).SetCellValue("");
            Cell celfirst = rowfirst.CreateCell(1);
            DateTime dateTiem = DateTime.Now.Date.AddDays(-7);
            string dateBegin = dateTiem.Year.ToString() + "-" + dateTiem.Month.ToString() + "-" + dateTiem.Day.ToString();
            DateTime dateEnd = DateTime.Now.AddDays(-1);
            string date = dateEnd.Year.ToString() + "-" + dateEnd.Month.ToString() + "-" + dateEnd.Day.ToString();
            celfirst.SetCellValue(dateBegin + " 00:00——" + date + " 24:00");
            celfirst.CellStyle = stylefirst;
            rowfirst.CreateCell(2).SetCellValue("");
            Cell cellTitle3 =rowfirst.CreateCell(3);
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 2));

            NPOI.SS.UserModel.CellStyle styleTitle = hssWorkBook.CreateCellStyle();
            styleTitle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            Font fontTitle = hssWorkBook.CreateFont();
            fontTitle.Boldweight = 700;
            styleTitle.SetFont(fontTitle);

            styleTitle.BorderBottom = CellBorderType.THIN;
            styleTitle.BorderLeft = CellBorderType.THIN;
            styleTitle.BorderRight = CellBorderType.THIN;
            styleTitle.BorderTop = CellBorderType.THIN;
           // cellTitle3.CellStyle = styleTitle;
            Row row = sheet.CreateRow(2);
            Cell cell0 = row.CreateCell(0);
            cell0.SetCellValue("序号");
            cell0.CellStyle = styleTitle;
            Cell cell1 = row.CreateCell(1);
            cell1.SetCellValue("顾客ID");
            cell1.CellStyle = styleTitle;
            Cell cell2 = row.CreateCell(2);
            cell2.SetCellValue("电子邮件");
            cell2.CellStyle = styleTitle;
            Cell cell3 = row.CreateCell(3);
            cell3.SetCellValue("状态");
            cell3.CellStyle = styleTitle;
            sheet.SetColumnWidth(0, 5*256);
            sheet.SetColumnWidth(1, 20*256);
            sheet.SetColumnWidth(2, 50*256);
            sheet.SetColumnWidth(3, 5*256);

        }
        private void SetCoutent(List<CustomerInfo> list, Sheet sheet)
        {
            NPOI.SS.UserModel.CellStyle styleCoutent = hssWorkBook.CreateCellStyle();
            styleCoutent.BorderBottom = CellBorderType.THIN;
            styleCoutent.BorderLeft = CellBorderType.THIN;
            styleCoutent.BorderRight = CellBorderType.THIN;
            styleCoutent.BorderTop = CellBorderType.THIN;

            for (int i = 0; i < list.Count; i++)
            {
                Row newrow = sheet.CreateRow(i + 3);
                Cell cell = newrow.CreateCell(0);
                cell.SetCellValue((i + 1).ToString());
                cell.CellStyle = styleCoutent;

                Cell cell1 = newrow.CreateCell(1);
                cell1.SetCellValue(list[i].CustomerID);
                cell1.CellStyle = styleCoutent;
                Cell cell2 = newrow.CreateCell(2);
                cell2.SetCellValue(list[i].Email);
                cell2.CellStyle = styleCoutent;
                Cell cell3 = newrow.CreateCell(3);
                cell3.SetCellValue(list[i].StausString);
                cell3.CellStyle = styleCoutent;
            }
        }
    }
}
