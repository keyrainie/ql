using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice.ReconReport;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using org.in2bits.MyXls;
using System.IO;
using System.Net;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(ReconReportProcessor))]
    public class ReconReportProcessor
    {
        private IReconReportDA m_ReconReportDA = ObjectFactory<IReconReportDA>.Instance;

        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransactionNumbers"></param>
        /// <returns></returns>
        public virtual int UpdateSAPStatus(List<int> TransactionNumbers)
        {
            int num = 0;
            try
            {
                num = m_ReconReportDA.UpdateSAPStatus(TransactionNumbers);
            }
            catch (Exception ex)
            {
                throw new BizException(ex.Message);
            }
            return num;
        }

        #region 手动生成报表
        public virtual void CreateReconReportByJob(DateTime? from, DateTime? to)
        {
            CreateExcel(from, to);        
        }

        public virtual void CreateReconReportByWeb(DateTime? from, DateTime? to)
        {
            List<DateTime> dates = new List<DateTime>();
            dates.Add(from.Value);
            for (int i = 0; i < (to.Value - from.Value).Days; i++)
            {
                dates.Add(dates[i].AddDays(1));
            }
            string msg = string.Empty;

            dates.ForEach((item) =>
            {
                CreateExcel(new DateTime(item.Year, item.Month, 1), item);
            });
        }

        protected virtual void CreateExcel(DateTime? from, DateTime? to)
        {
            //总账科目汇总
            List<SAPInterfaceExchangeInfo> lstAll = m_ReconReportDA.CreateExcel(from, to);
            //总账科目差异
            List<SAPInterfaceExchangeInfo> lstUnbalance = GetUnbalance(lstAll);
            DateTime now = DateTime.Now;
            string dateStr = to.Value.Date.ToShortDateString().Replace("/", "-");
            string beginDateStr = from.Value.ToShortDateString().Replace("-", "/");
            string endDateStr = to.Value.Date.ToShortDateString().Replace("-", "/");
            string fileName = ReconReportConfig.BasicDirectory + "SAPInterface_" + dateStr + ".xls";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ReconReportExportData reporter = new ReconReportExportData();
            reporter.begin = beginDateStr;
            reporter.end = endDateStr;
            Sheet sheet1 = hssfworkbook.CreateSheet("SAPInterface_" + dateStr + "(S)");
            reporter.ImportDataToSheet(sheet1, lstAll);

            //统驭科目(AR/AP)汇总
            List<SAPInterfaceExchangeInfo> lstOther = m_ReconReportDA.CreateOtherExcel(from, to);
            //统驭账科目差异
            List<SAPInterfaceExchangeInfo> lstOtherUnbalance = GetUnbalance(lstOther);
            ReconOtherReportExportData OtherReporter = new ReconOtherReportExportData();
            OtherReporter.begin = beginDateStr;
            OtherReporter.end = endDateStr;
            Sheet sheet2 = hssfworkbook.CreateSheet("AR&AP Report_" + dateStr);
            OtherReporter.ImportDataToSheet(sheet2, lstOther);

            FileStream file = new FileStream(fileName, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
            hssfworkbook.Dispose();

            //上传
            string ftpurl = UpLoad(fileName);
            //发邮件
            SendEmail(ftpurl, dateStr, lstUnbalance, lstOtherUnbalance);
        }


        /// <summary>
        /// 初始化WebClient
        /// </summary>
        /// <returns></returns>
        private static List<SAPInterfaceExchangeInfo> GetUnbalance(List<SAPInterfaceExchangeInfo> lstAll)
        {
            List<SAPInterfaceExchangeInfo> lstResult = new List<SAPInterfaceExchangeInfo>();
            foreach (SAPInterfaceExchangeInfo sap in lstAll)
            {
                if (sap.DateBalance != 0)
                {
                    lstResult.Add(sap);
                }
            }

            return lstResult;
        }

        /// <summary>
        /// 上传EXCEl到服务器
        /// </summary>
        private static string UpLoad(string excelPath)
        {
            string name = Path.GetFileName(excelPath);
            string url = string.Format(@"{0}{1}", FTP.Address, name);
            WebClient client = CreateClient();

            byte[] response = client.UploadFile(url, WebRequestMethods.Ftp.UploadFile, excelPath);
            //上传成功后删除
            File.Delete(excelPath);
            return url;
        }

        /// <summary>
        /// 初始化WebClient
        /// </summary>
        /// <returns></returns>
        private static WebClient CreateClient()
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            client.Credentials = new NetworkCredential(FTP.UserName, FTP.Password);
            client.Encoding = Encoding.UTF8;

            return client;
        }

        /// <summary>
        /// 发送Email
        /// </summary>
        private static void SendEmail(string ftpurl, string dateStr, List<SAPInterfaceExchangeInfo> lstUnblanced, List<SAPInterfaceExchangeInfo> lstOtherUnblanced)
        {
            Email emailConfig = new Email(dateStr);
            MailMessage email = new MailMessage();
            email.FromName = emailConfig.MailFrom;
            email.ToName = emailConfig.MailTo;

            StringBuilder sbBody = new StringBuilder();
            sbBody.Append("Dear All:<br/><br/>");
            sbBody.Append("    本邮件由Recon Report系统自动发布（" + dateStr + "），请勿回复.<br/><br/>");
            sbBody.Append("    请查看报表：" + ftpurl + "<br/><br/>");

            bool isBalanced = true;
            if (lstUnblanced.Count > 0)
            {
                isBalanced = false;
                email.Subject = emailConfig.MailSubject.Replace("(info)", "(info/Unbalanced)");
                sbBody.Append("其中总账科目对账不平数据如下: <br/>");
                sbBody.Append("<table><tr>");
                sbBody.Append("<td align=center>  Report Name  </td>");
                sbBody.Append("<td align=center>  CompanyCode  </td>");
                sbBody.Append("<td align=center>  GL Code  </td>");
                sbBody.Append("<td align=center>  Legacy Data  </td>");
                sbBody.Append("<td align=center>  Retrieved from SAP  </td>");
                sbBody.Append("<td align=center>  Balance  </td>");
                sbBody.Append("</tr>");
                foreach (SAPInterfaceExchangeInfo unblanced in lstUnblanced)
                {
                    sbBody.Append("<tr>");
                    sbBody.Append("<td align=left>Invoice " + unblanced.DocumentType + " Daily</td>");
                    sbBody.Append("<td align=center>" + unblanced.CompanyCode + "</td>");
                    sbBody.Append("<td align=center>" + unblanced.GLAccount + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.Legacy_GLAmount + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.SAP_GLAmount + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.DateBalance + "</td>");
                    sbBody.Append("</tr>");
                }
                sbBody.Append("</table><br/><br/>");
            }

            if (lstOtherUnblanced.Count > 0)
            {
                isBalanced = false;
                email.Subject = emailConfig.MailSubject.Replace("(info)", "(info/Unbalanced)");
                sbBody.Append("其中统驭科目对账不平数据如下: <br/>");
                sbBody.Append("<table><tr>");
                sbBody.Append("<td align=center>  Report Name  </td>");
                sbBody.Append("<td align=center>  AcctType  </td>");
                sbBody.Append("<td align=center>  CompanyCode  </td>");
                sbBody.Append("<td align=center>  Legacy Data  </td>");
                sbBody.Append("<td align=center>  Retrieved from SAP  </td>");
                sbBody.Append("<td align=center>  Balance  </td>");
                sbBody.Append("</tr>");
                foreach (SAPInterfaceExchangeInfo unblanced in lstOtherUnblanced)
                {
                    sbBody.Append("<tr>");
                    sbBody.Append("<td align=center>Invoice " + unblanced.DocumentType + " Daily</td>");
                    sbBody.Append("<td align=center>" + unblanced.AcctTypeDisplay + "</td>");
                    sbBody.Append("<td align=center>" + unblanced.CompanyCode + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.Legacy_GLAmount + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.SAP_GLAmount + "</td>");
                    sbBody.Append("<td align=right>" + unblanced.DateBalance + "</td>");
                    sbBody.Append("</tr>");
                }
                sbBody.Append("</table><br/><br/>");
            }

            if (isBalanced)
            {
                email.Subject = emailConfig.MailSubject.Replace("(info)", "(info/Balanced)");
            }

            email.Body = sbBody.ToString();

            ObjectFactory<IEmailSend>.Instance.SendMail(email, true, true);

            //EmailHelper.SendEmailByTemplate(string.Empty, string.Empty);
        }

        #endregion
    }

    /// <summary>
    /// SAPInterface
    /// </summary>
    public class ReconReportExportData : ExcelFileExporter
    {
        public string begin = string.Empty;
        public string end = string.Empty;

        protected void CreateExcelColumns(Sheet worksheet)
        {
            CreateExcelColumns(worksheet, 4000, 0, 1, 2, 3, 5, 6, 8);
            CreateExcelColumns(worksheet, 7000, 4, 7);
        }

        protected void CreateDataHeader(Sheet worksheet)
        {
            StyleManager.InitStaticStyle();
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 2));
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 3, 5));
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 6, 8));

            int colindex = 0;
            NPOI.SS.UserModel.Row header = worksheet.CreateRow(0);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue("Newegg.com.cn");
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue("Posting Date: " + end);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue("MTD( " + begin + " - " + end + " )");
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue(string.Empty);

            NPOI.SS.UserModel.Row header2 = worksheet.CreateRow(1);
            colindex = 0;
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Legacy Data");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Retrieved from SAP Table");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Balance");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Legacy Data");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Retrieved from SAP Table");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Balance");

            NPOI.SS.UserModel.Row header3 = worksheet.CreateRow(2);
            colindex = 0;
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("DocumentType");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("GL Code");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("CompanyCode");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(A)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(C=A-B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(A)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(C=A-B)");
        }

        protected void CreateDataCells(Sheet worksheet, List<SAPInterfaceExchangeInfo> dataList)
        {
            ushort currentRowIndex = 3;
            ushort currentColIndex = 0;

            dataList.ForEach(delegate(SAPInterfaceExchangeInfo item)
            {
                NPOI.SS.UserModel.Row data = worksheet.CreateRow(currentRowIndex++);
                currentColIndex = 0;
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.DocumentType);
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.GLAccount);
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.CompanyCode);
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.Legacy_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.SAP_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.DateBalance));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDLegacy_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDSAP_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDBalance));
            });
        }

        protected void CreateExcelColumns(Sheet worksheet, int width, params ushort[] colIndexs)
        {
            for (int i = 0; i < colIndexs.Length; i++)
            {
                worksheet.SetColumnWidth(colIndexs[i], width);
            }
        }

        //导入到Sheet
        public void ImportDataToSheet(Sheet worksheet, List<SAPInterfaceExchangeInfo> dataList)
        {
            CreateExcelColumns(worksheet);
            CreateDataHeader(worksheet);
            CreateDataCells(worksheet, dataList);
        }
    }

    /// <summary>
    /// AR&AP
    /// </summary>
    public class ReconOtherReportExportData : ExcelFileExporter
    {
        public string begin = string.Empty;
        public string end = string.Empty;

        protected void CreateExcelColumns(Sheet worksheet)
        {
            CreateExcelColumns(worksheet, 4000, 0, 1, 2, 3, 5, 6, 8);
            CreateExcelColumns(worksheet, 7000, 4, 7);
        }

        protected void CreateDataHeader(Sheet worksheet)
        {
            StyleManager.InitStaticStyle();
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 8));
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 2));
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 3, 5));
            worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 6, 8));

            int colindex = 0;
            NPOI.SS.UserModel.Row header = worksheet.CreateRow(0);
            StyleManager.SetCenterBoldStringCell(header.CreateCell(colindex++)).SetCellValue("Daily AR&AP Interface");
            header.Height = 450;

            NPOI.SS.UserModel.Row header1 = worksheet.CreateRow(1);
            colindex = 0;
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue("Newegg.com.cn");
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue("Posting Date: " + end);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue("MTD( " + begin + " - " + end + " )");
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header1.CreateCell(colindex++)).SetCellValue(string.Empty);

            NPOI.SS.UserModel.Row header2 = worksheet.CreateRow(2);
            colindex = 0;
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue(string.Empty);
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Legacy Data");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Retrieved from SAP Table");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Balance");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Legacy Data");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Retrieved from SAP Table");
            StyleManager.SetCenterBoldStringCell(header2.CreateCell(colindex++)).SetCellValue("Balance");

            NPOI.SS.UserModel.Row header3 = worksheet.CreateRow(3);
            colindex = 0;
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("AcctType");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("DocumentType");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("CompanyCode");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(A)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(C=A-B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(A)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(B)");
            StyleManager.SetCenterBoldStringCell(header3.CreateCell(colindex++)).SetCellValue("(C=A-B)");
        }

        protected void CreateDataCells(Sheet worksheet, List<SAPInterfaceExchangeInfo> dataList)
        {
            ushort currentRowIndex = 4;
            ushort currentColIndex = 0;

            dataList.ForEach(delegate(SAPInterfaceExchangeInfo item)
            {
                NPOI.SS.UserModel.Row data = worksheet.CreateRow(currentRowIndex++);
                currentColIndex = 0;
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.AcctTypeDisplay);
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.DocumentType);
                StyleManager.SetCenterBoldStringCell(data.CreateCell(currentColIndex++)).SetCellValue(item.CompanyCode);
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.Legacy_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.SAP_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.DateBalance));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDLegacy_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDSAP_GLAmount));
                StyleManager.SetRightDataCell(data.CreateCell(currentColIndex++)).SetCellValue(Convert.ToDouble(item.MTDData.MTDBalance));
            });
        }

        protected void CreateExcelColumns(Sheet worksheet, int width, params ushort[] colIndexs)
        {
            for (int i = 0; i < colIndexs.Length; i++)
            {
                worksheet.SetColumnWidth(colIndexs[i], width);
            }
        }

        public void ImportDataToSheet(Sheet worksheet, List<SAPInterfaceExchangeInfo> dataList)
        {
            CreateExcelColumns(worksheet);
            CreateDataHeader(worksheet);
            CreateDataCells(worksheet, dataList);
        }

    }

    public static class StyleManager
    {
        static CellStyle rightDataCellStyle = null, centerBoldCellStyle = null;

        public static void InitStaticStyle()
        {
            rightDataCellStyle = null;
            centerBoldCellStyle = null;
        }

        /// <summary>
        /// 右对齐数字格式
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static CellStyle GetRightDataCellStyle(NPOI.SS.UserModel.Cell cell)
        {
            if (rightDataCellStyle == null)
            {
                CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
                style.BorderBottom = CellBorderType.THIN;
                style.BorderLeft = CellBorderType.THIN;
                style.BorderRight = CellBorderType.THIN;
                style.BorderTop = CellBorderType.THIN;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
                NPOI.SS.UserModel.Font font = cell.Sheet.Workbook.CreateFont();
                font.FontHeightInPoints = 9;
                style.SetFont(font);
                style.DataFormat = cell.Sheet.Workbook.CreateDataFormat().GetFormat("_ * #,##0.00_ ;_ * -#,##0.00_ ;_ * \" - \"??_ ;_ @_ ");
                rightDataCellStyle = style;
            }

            return rightDataCellStyle;
        }

        /// <summary>
        /// 居中对齐加粗格式
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static CellStyle GetCenterBoldCellStyle(NPOI.SS.UserModel.Cell cell)
        {
            if (centerBoldCellStyle == null)
            {
                CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
                style.BorderBottom = CellBorderType.THIN;
                style.BorderLeft = CellBorderType.THIN;
                style.BorderRight = CellBorderType.THIN;
                style.BorderTop = CellBorderType.THIN;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
                NPOI.SS.UserModel.Font font = cell.Sheet.Workbook.CreateFont();
                font.FontHeightInPoints = 10;
                font.Boldweight = (short)FontBoldWeight.BOLD;
                style.SetFont(font);
                centerBoldCellStyle = style;
            }

            return centerBoldCellStyle;
        }

        /// <summary>
        /// 设置居中对齐加粗字符串格式
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static NPOI.SS.UserModel.Cell SetCenterBoldStringCell(NPOI.SS.UserModel.Cell cell)
        {
            cell.SetCellType(CellType.STRING);
            cell.CellStyle = GetCenterBoldCellStyle(cell);
            return cell;
        }

        /// <summary>
        /// 设置右对齐数字格式
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static NPOI.SS.UserModel.Cell SetRightDataCell(NPOI.SS.UserModel.Cell cell)
        {
            cell.SetCellType(CellType.NUMERIC);
            cell.CellStyle = GetRightDataCellStyle(cell);
            return cell;
        }
    }
}
