using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using ProductSaleInfoBiz.DAL;
using ProductSaleInfoBiz.Model;
using Newegg.Oversea.Framework.Utilities.Compress;
using System.IO.Compression;
using Microsoft.Win32;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
namespace ProductSaleInfoBiz.Biz
{
    public class ExcelManage
    {
        string dateString = DateTime.Now.Date.Year.ToString() + "-" + DateTime.Now.Date.Month.ToString() + "-" + DateTime.Now.Date.Day.ToString();
        HSSFWorkbook hssWorkBook = new HSSFWorkbook();
        Sheet sheet;
        public byte[] Getbytes(List<ProductSaleInfo> list)
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls")))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls"));
            }
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar")))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar"));
            }

            
            SetHead();
            SetCoutent(list);
          
            FileStream stream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls"), FileMode.CreateNew, FileAccess.ReadWrite);
            hssWorkBook.Write(stream);
            stream.Position = 0;
            stream.Flush();
            stream.Close();
            RARsave(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls"
                , AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar");
            byte[] fileBytes = new byte[1];
            using (FileStream streamRar = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar"), FileMode.Open, FileAccess.Read))
            {

                fileBytes = new byte[(int)streamRar.Length];
                streamRar.Read(fileBytes, 0, (int)streamRar.Length);
                streamRar.Close();
            }

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls")))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale" + dateString + ".xls"));
            }
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar")))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSale.rar"));
            }

            return fileBytes;
        }

        public string WriteToFile(List<ProductSaleInfo> list)
        {
            string targetPath = AppDomain.CurrentDomain.BaseDirectory + "Report\\";
            string filePath = targetPath + "ProductSale.rar";
            if (!System.IO.Directory.Exists(targetPath))
                System.IO.Directory.CreateDirectory(targetPath);

            if (File.Exists(Path.Combine(targetPath, "ProductSale" + dateString + ".xls")))
            {
                File.Delete(Path.Combine(targetPath, "ProductSale" + dateString + ".xls"));
            }
            if (File.Exists(Path.Combine(targetPath, "ProductSale.rar")))
            {
                File.Delete(Path.Combine(targetPath, "ProductSale.rar"));
            }

            SetHead();
            SetCoutent(list);

            FileStream stream = new FileStream(Path.Combine(targetPath, "ProductSale" + dateString + ".xls"), FileMode.CreateNew, FileAccess.ReadWrite);
            hssWorkBook.Write(stream);
            stream.Position = 0;
            stream.Flush();
            stream.Close();

            RARsave(targetPath, "ProductSale" + dateString + ".xls"
                , targetPath, "ProductSale.rar");

            return filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rarPatch">rar文件所在目录</param>
        /// <param name="rarFiles">要压缩的文件列表</param>
        /// <param name="patch">要放Rar目录</param>
        /// <param name="rarName">Rar文件名</param>
        public void RARsave(string rarPatch, string rarFiles, string patch, string rarName)
        {
            string the_rar;
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                the_rar = Path.Combine( AppDomain.CurrentDomain.BaseDirectory+"Compoents\\" ,"Rar.exe");//the_rar.Substring(1, the_rar.Length - 7);
                if (!Directory.Exists(patch))
                    Directory.CreateDirectory(patch);
                //命令参数
                the_Info = string.Format(" a {0} {1}  -r", rarName,rarFiles);// " a " + rarName + " " + patch;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments =  the_Info ;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录

                the_StartInfo.WorkingDirectory = rarPatch;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private  void SetHead()
        {
          
            sheet = hssWorkBook.CreateSheet("CustomerInfo");

            NPOI.SS.UserModel.CellStyle stylefirst = hssWorkBook.CreateCellStyle();
            stylefirst.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            //Font fontfirst = hssWorkBook.CreateFont();
            //fontfirst.FontName = "宋体";
            //fontfirst.FontHeightInPoints = 10;
            //fontfirst.Boldweight = 700;
            //stylefirst.SetFont(fontfirst);
           
            //stylefirst.BorderBottom = CellBorderType.THIN;
            //stylefirst.BorderLeft = CellBorderType.THIN;
            //stylefirst.BorderRight = CellBorderType.THIN;
            //stylefirst.BorderTop = CellBorderType.THIN;

            Row rowHead = sheet.CreateRow(0);

            Cell cal21 = rowHead.CreateCell(0); cal21.SetCellValue("Domain"); cal21.CellStyle = stylefirst;
            Cell cal0 = rowHead.CreateCell(1,CellType.STRING); cal0.SetCellValue("Item#"); cal0.CellStyle = stylefirst;
            Cell cal1 = rowHead.CreateCell(2); cal1.SetCellValue("商品名称"); cal1.CellStyle = stylefirst;
            Cell cal2 = rowHead.CreateCell(3); cal2.SetCellValue("一级类"); cal2.CellStyle = stylefirst;
            Cell cal3 = rowHead.CreateCell(4); cal3.SetCellValue("二级类"); cal3.CellStyle = stylefirst;
            Cell cal4 = rowHead.CreateCell(5); cal4.SetCellValue("三级类"); cal4.CellStyle = stylefirst;
            Cell cal5 = rowHead.CreateCell(6); cal5.SetCellValue("品牌"); cal5.CellStyle = stylefirst;
            Cell cal6 = rowHead.CreateCell(7); cal6.SetCellValue("PM"); cal6.CellStyle = stylefirst;
            Cell cal7 = rowHead.CreateCell(8); cal7.SetCellValue("销售价格"); cal7.CellStyle = stylefirst;
            Cell cal8 = rowHead.CreateCell(9); cal8.SetCellValue("Unit Cost"); cal8.CellStyle = stylefirst;
            Cell cal9 = rowHead.CreateCell(10); cal9.SetCellValue("CreateTime"); cal9.CellStyle = stylefirst;
            Cell cal10 = rowHead.CreateCell(11); cal10.SetCellValue("首次上线时间"); cal10.CellStyle = stylefirst;

            Cell cal25 = rowHead.CreateCell(12); cal25.SetCellValue("最近一次采购时间"); cal25.CellStyle = stylefirst;
            
            Cell cal22 = rowHead.CreateCell(13); cal22.SetCellValue("评论数"); cal22.CellStyle = stylefirst;
            Cell cal11 = rowHead.CreateCell(14); cal11.SetCellValue("虚库数量"); cal11.CellStyle = stylefirst;
            Cell cal12 = rowHead.CreateCell(15); cal12.SetCellValue("Online库存"); cal12.CellStyle = stylefirst;
            Cell cal13 = rowHead.CreateCell(16); cal13.SetCellValue("前7天销售数量"); cal13.CellStyle = stylefirst;
            Cell cal14 = rowHead.CreateCell(17); cal14.SetCellValue("前7天销售金额"); cal14.CellStyle = stylefirst;
            Cell cal15 = rowHead.CreateCell(18); cal15.SetCellValue("前14天销售数量"); cal15.CellStyle = stylefirst;
            Cell cal16 = rowHead.CreateCell(19); cal16.SetCellValue("前14天销售金额"); cal16.CellStyle = stylefirst;
            Cell cal17 = rowHead.CreateCell(20); cal17.SetCellValue("前30天销售数量"); cal17.CellStyle = stylefirst;
            Cell cal18 = rowHead.CreateCell(21); cal18.SetCellValue("前30天销售金额"); cal18.CellStyle = stylefirst;
            Cell cal19 = rowHead.CreateCell(22); cal19.SetCellValue("前60天销售数量"); cal19.CellStyle = stylefirst;
            Cell cal20 = rowHead.CreateCell(23); cal20.SetCellValue("前60天销售金额"); cal20.CellStyle = stylefirst;

            Cell cal23 = rowHead.CreateCell(24); cal23.SetCellValue("销售数量"); cal23.CellStyle = stylefirst;
            Cell cal24 = rowHead.CreateCell(25); cal24.SetCellValue("销售金额（去税）"); cal24.CellStyle = stylefirst;
           
            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 15 * 256);
            sheet.SetColumnWidth(2, 50 * 256);
            sheet.SetColumnWidth(3, 15 * 256);
            sheet.SetColumnWidth(4, 15 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 15 * 256);
            sheet.SetColumnWidth(7, 15 * 256);
            sheet.SetColumnWidth(8, 15 * 256);
            sheet.SetColumnWidth(9, 15 * 256);
            sheet.SetColumnWidth(10, 15 * 256);
            sheet.SetColumnWidth(11, 15 * 256);
            sheet.SetColumnWidth(12, 15 * 256);
            sheet.SetColumnWidth(13, 15 * 256);
            sheet.SetColumnWidth(14, 15 * 256);
            sheet.SetColumnWidth(15, 15 * 256);
            sheet.SetColumnWidth(16, 15 * 256);
            sheet.SetColumnWidth(17, 15 * 256);
            sheet.SetColumnWidth(18, 15 * 256);
            sheet.SetColumnWidth(19, 15 * 256);
            sheet.SetColumnWidth(20, 15 * 256);
            sheet.SetColumnWidth(21, 15 * 256);
            sheet.SetColumnWidth(22, 15 * 256);
            sheet.SetColumnWidth(23, 15 * 256);
            sheet.SetColumnWidth(24, 15 * 256);
            sheet.SetColumnWidth(25, 15 * 256);
            //  
          //  Cell cell=rowHead.CreateCell(0);
          //  cell.SetCellValue("新客户注册统计表");
          //  Cell cellhead1 = rowHead.CreateCell(1);
          //  Cell cellhead2 = rowHead.CreateCell(2);
          //  Cell cellhead3 = rowHead.CreateCell(3);
          //  sheet.AddMergedRegion(new  CellRangeAddress(0,0,0,3));

          //  NPOI.SS.UserModel.CellStyle style = hssWorkBook.CreateCellStyle();
          //  style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
          //  Font font = hssWorkBook.CreateFont();
          //  font.FontName = "黑体";
          //  font.Boldweight = 700;


          //  font.FontHeightInPoints = 14;
          //  style.SetFont(font);

          //  //style.BorderBottom = CellBorderType.THIN;
          //  //style.BorderLeft = CellBorderType.THIN;
          //  //style.BorderRight = CellBorderType.THIN;
          //  //style.BorderTop = CellBorderType.THIN;

          //  cell.CellStyle = style;
          //  cellhead1.CellStyle = style;
          //  cellhead2.CellStyle = style;
          //  cellhead3.CellStyle = style;
          //  rowHead.Height = 2*256;


          
          ////  rowHead.Height = 1 * 256;

          //  Row rowfirst = sheet.CreateRow(1);
          //  rowfirst.CreateCell(0).SetCellValue("");
          //  Cell celfirst = rowfirst.CreateCell(1);
          //  DateTime dateTiem = DateTime.Now.Date.AddDays(-7);
          //  string dateBegin = dateTiem.Year.ToString() + "-" + dateTiem.Month.ToString() + "-" + dateTiem.Day.ToString();
          //  DateTime dateEnd = DateTime.Now.AddDays(-1);
          //  string date = dateEnd.Year.ToString() + "-" + dateEnd.Month.ToString() + "-" + dateEnd.Day.ToString();
          //  celfirst.SetCellValue(dateBegin + " 00:00——" + date + " 24:00");
          //  celfirst.CellStyle = stylefirst;
          //  rowfirst.CreateCell(2).SetCellValue("");
          //  Cell cellTitle3 =rowfirst.CreateCell(3);
          //  sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 2));

          //  NPOI.SS.UserModel.CellStyle styleTitle = hssWorkBook.CreateCellStyle();
          //  styleTitle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
          //  Font fontTitle = hssWorkBook.CreateFont();
          //  fontTitle.Boldweight = 700;
          //  styleTitle.SetFont(fontTitle);

          //  styleTitle.BorderBottom = CellBorderType.THIN;
          //  styleTitle.BorderLeft = CellBorderType.THIN;
          //  styleTitle.BorderRight = CellBorderType.THIN;
          //  styleTitle.BorderTop = CellBorderType.THIN;
          // // cellTitle3.CellStyle = styleTitle;
          //  Row row = sheet.CreateRow(2);
          //  Cell cell0 = row.CreateCell(0);
          //  cell0.SetCellValue("序号");
          //  cell0.CellStyle = styleTitle;
          //  Cell cell1 = row.CreateCell(1);
          //  cell1.SetCellValue("顾客ID");
          //  cell1.CellStyle = styleTitle;
          //  Cell cell2 = row.CreateCell(2);
          //  cell2.SetCellValue("电子邮件");
          //  cell2.CellStyle = styleTitle;
          //  Cell cell3 = row.CreateCell(3);
          //  cell3.SetCellValue("状态");
          //  cell3.CellStyle = styleTitle;
          //  sheet.SetColumnWidth(0, 5*256);
          //  sheet.SetColumnWidth(1, 20*256);
          //  sheet.SetColumnWidth(2, 50*256);
          //  sheet.SetColumnWidth(3, 5*256);

        }
        private void SetCoutent(List<ProductSaleInfo> list)
        {
            CellStyle cellTxtStyle = hssWorkBook.CreateCellStyle();
            cellTxtStyle.DataFormat =(HSSFDataFormat.GetBuiltinFormat("text"));

            NPOI.SS.UserModel.CellStyle styleCoutent = hssWorkBook.CreateCellStyle();
            styleCoutent.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

            NPOI.SS.UserModel.CellStyle styleCoutentdate = hssWorkBook.CreateCellStyle();
            DataFormat format = hssWorkBook.CreateDataFormat();
            styleCoutentdate.DataFormat = format.GetFormat("yyyy年m月d日");


            for (int i = 0; i < list.Count; i++)
            {
                Row newrow = sheet.CreateRow(i + 1);
                Cell cel21 = newrow.CreateCell(0); cel21.SetCellValue(list[i].Domain); 
                Cell cel0 = newrow.CreateCell(1); cel0.SetCellValue(list[i].ProductID); cel0.CellStyle = cellTxtStyle;
                Cell cel1 = newrow.CreateCell(2); cel1.SetCellValue(list[i].ProductName);
                Cell cel2 = newrow.CreateCell(3); cel2.SetCellValue(list[i].Category1Name);
                Cell cel3 = newrow.CreateCell(4); cel3.SetCellValue(list[i].Category2Name);
                Cell cel4 = newrow.CreateCell(5); cel4.SetCellValue(list[i].Category3Name);
                Cell cel5 = newrow.CreateCell(6); cel5.SetCellValue(list[i].Manufacturername);
                Cell cel6 = newrow.CreateCell(7); cel6.SetCellValue(list[i].LastPMName);
                Cell cel7 = newrow.CreateCell(8); cel7.SetCellValue(list[i].CurrentPrice.HasValue ? (double)list[i].CurrentPrice.Value : 0.00); cel7.CellStyle = styleCoutent;
                Cell cel8 = newrow.CreateCell(9); cel8.SetCellValue(list[i].UnitCost.HasValue ? (double)list[i].UnitCost.Value : 0.00); cel8.CellStyle = styleCoutent;
                Cell cel9 = newrow.CreateCell(10);
                if (list[i].CreateTime.HasValue) { cel9.SetCellValue( list[i].CreateTime.Value); } cel9.CellStyle = styleCoutentdate;
                Cell cel10 = newrow.CreateCell(11);
                if (list[i].FirstOnlineTime.HasValue) { cel10.SetCellValue(list[i].FirstOnlineTime.Value); } cel10.CellStyle = styleCoutentdate;

                Cell cel25 = newrow.CreateCell(12);
                if (list[i].LastInTime.HasValue) { cel25.SetCellValue(list[i].LastInTime.Value); } cel25.CellStyle = styleCoutentdate;

                Cell cel22 = newrow.CreateCell(13);
                if (list[i].VirtualQty.HasValue) { cel22.SetCellValue(list[i].DetailCount.Value); }

                Cell cel11 = newrow.CreateCell(14);
                if (list[i].VirtualQty.HasValue) { cel11.SetCellValue(list[i].VirtualQty.Value); }
                Cell cel12 = newrow.CreateCell(15);
                if (list[i].OnlineAccountQty.HasValue) { cel12.SetCellValue(list[i].OnlineAccountQty.Value); }
                Cell cel13 = newrow.CreateCell(16);
                if (list[i].QuantityW1.HasValue) { cel13.SetCellValue(list[i].QuantityW1.Value); }
                Cell cel14 = newrow.CreateCell(17);
                if (list[i].ProductAmtW1.HasValue) { cel14.SetCellValue((double)list[i].ProductAmtW1.Value); } cel14.CellStyle = styleCoutent;
                Cell cel15 = newrow.CreateCell(18);
                if (list[i].Quantity14days.HasValue) { cel15.SetCellValue(list[i].Quantity14days.Value); }
                Cell cel16 = newrow.CreateCell(19);
                if (list[i].ProductAmt14days.HasValue) { cel16.SetCellValue((double)list[i].ProductAmt14days.Value); } cel16.CellStyle = styleCoutent;
                Cell cel17 = newrow.CreateCell(20);
                if (list[i].QuantityM1.HasValue) { cel17.SetCellValue(list[i].QuantityM1.Value); }
                Cell cel18 = newrow.CreateCell(21);
                if (list[i].ProductAmtM1.HasValue) { cel18.SetCellValue((double)list[i].ProductAmtM1.Value); } cel18.CellStyle = styleCoutent;

                Cell cel19 = newrow.CreateCell(22);
                if (list[i].QuantityM1.HasValue) { cel19.SetCellValue(list[i].Quantity60days.Value); }
                Cell cel20 = newrow.CreateCell(23);
                if (list[i].ProductAmtM1.HasValue) { cel20.SetCellValue((double)list[i].ProductAmt60days.Value); } cel20.CellStyle = styleCoutent;

                Cell cel23 = newrow.CreateCell(24);
                if (list[i].QuantityM1.HasValue) { cel23.SetCellValue(list[i].AllQuantity.Value); }
                Cell cel24 = newrow.CreateCell(25);
                if (list[i].ProductAmtM1.HasValue) { cel24.SetCellValue((double)list[i].AllAmt.Value); } cel24.CellStyle = styleCoutent;

              
                
                
              
            
            }
        }

    }
}
