using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.ContentMgmt.SendMailForPicture.DA;
using IPP.ContentMgmt.SendMailForPicture.BusinessEntities;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ContentMgmt.SendMailForPicture.Common;
using System.Configuration;

namespace IPP.ContentMgmt.SendMailForPicture.Biz
{
    public class Product_StatusBP
    {
        public static JobContext jobContext = null;
        public static void SendNotTick()
        {
            List<ProductList> notTickPictureList = new List<ProductList>();
            notTickPictureList = Product_StatusDA.GetNotTickPicture();
            GetTickMail(notTickPictureList);
        }
        public static void SendUpload()
        {
            List<ProductList> notUploadPicture = new List<ProductList>();
            notUploadPicture = Product_StatusDA.GetNotUploadPicture();
            GetUploadMail(notUploadPicture);
        }
        public static void GetTickMail(List<ProductList> TickPictureList)
        {
            StringBuilder sbTick = new StringBuilder(1000);
            sbTick.Append("<table border=1 ><font color=blue></font>");
            sbTick.Append("<tr><td colspan=3 align=center>图片没有打勾Item#</td></tr>");
            sbTick.Append("<tr>");
            sbTick.Append("<td>三级类名称</td>");
            sbTick.Append("<td>产品id</td>");
            sbTick.Append("<td>产品名称</td>");
            sbTick.Append("</tr>");
            foreach (ProductList p in TickPictureList)
            {
                sbTick.Append("<tr>");
                sbTick.Append("<td>" + p.c3name.ToString() + "</td>");
                sbTick.Append("<td>" + p.ProductID.ToString() + "</td>");
                sbTick.Append("<td>" + p.ProductName.ToString() + "</td>");
                sbTick.Append("</tr>");
            }
            sbTick.Append("</table>");
            SendMail(sbTick);
        }
        public static void GetUploadMail(List<ProductList> UploadPicture)
        {
            StringBuilder sbUpload = new StringBuilder(1000);
            sbUpload.Append("<table border=1 ><font color=blue></font>");
            sbUpload.Append("<tr><td colspan=3 align=center>没有上传图片Item#</td></tr>");
            sbUpload.Append("<tr>");
            sbUpload.Append("<td>三级类名称</td>");
            sbUpload.Append("<td>产品id</td>");
            sbUpload.Append("<td>产品名称</td>");
            sbUpload.Append("</tr>");
            foreach (ProductList p in UploadPicture)
            {
                sbUpload.Append("<tr>");
                sbUpload.Append("<td>" + p.c3name.ToString() + "</td>");
                sbUpload.Append("<td>" + p.ProductID.ToString() + "</td>");
                sbUpload.Append("<td>" + p.ProductName.ToString() + "</td>");
                sbUpload.Append("</tr>");
            }
            sbUpload.Append("</table>");
            SendMail(sbUpload);
        }
        public static void SendMail(StringBuilder sb)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            DateTime today = DateTime.Today;

            //MailBodyV31 mb = new MailBodyV31();
            //mb.Body = new MailBodyMsg();
            //mb.Body.MailFrom = ConfigurationManager.AppSettings["MailAddress"];
            //mb.Body.MailTo = ConfigurationManager.AppSettings["MailAddress"];
            //mb.Body.MailBody = sb.ToString();
            //mb.Body.Subjuect = "" + year.ToString() + "年" + month.ToString() + "月" + day.ToString() + "日需要拍照商品Item#";
            //mb.Body.Status = 0;
            //mb.Body.Priority = 0;

            Product_StatusDA.InsertMailDB(sb.ToString());
        }

        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;


        public static void CheckProduct_StatusItem()
        {

            string endMsg = string.Empty;
            WriteLog("\r\n" + DateTime.Now + " ------------------- Begin-------------------------");
            WriteLog(DateTime.Now + " 实库图片拍摄Item 的Newegg Support 通知邮件");
            try
            {

                WriteLog(DateTime.Now + " 正在获取发送图片没有打钩ITEM......");
                SendNotTick();

                WriteLog(DateTime.Now + " 正在获取发送没有上传图片ITEM......");
                SendUpload();

                endMsg = DateTime.Now + " 本次job成功结束!";
                WriteLog(endMsg);
            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.Message;
                SendExceptionInfoEmail(endMsg);
                WriteLog(endMsg);
            }
            WriteLog(DateTime.Now + " ------------------- End-----------------------------\r\n");
        }

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                Product_StatusDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }
    }
}
