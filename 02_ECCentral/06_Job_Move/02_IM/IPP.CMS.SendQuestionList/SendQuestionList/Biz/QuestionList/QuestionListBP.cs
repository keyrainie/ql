using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.ContentMgmt.SendQuestionList.DA;
using IPP.ContentMgmt.SendQuestionList.BusinessEntities;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.ServiceContracts;
using System.ComponentModel;
using System.Reflection;

namespace IPP.ContentMgmt.SendQuestionList.Biz
{
    public class QuestionListBP
    {

        #region rules and const
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;
        #endregion

        #region Business

        public static JobContext jobContext = null;
        private static DateTime TimeFlage;

        public static void CheckQuestionListItem()
        {
            if (TimeFlage.Date < DateTime.Now.Date)
            {
                TimeFlage = DateTime.Now;
            }

            string endMsg = string.Empty;
            WriteLog("\r\n" + DateTime.Now + "------------------- Begin-------------------------");
            WriteLog("\r\n" + DateTime.Now + "用户产品询问回复情况表job开始运行......");

            try
            {
                SendQuestionList();

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


        /// <summary>
        /// 发送问题回复列表Mail
        /// </summary>
        public static void SendQuestionList()
        {
            MailBodyV31 mailBody = new MailBodyV31();
            mailBody.Body = new MailBodyMsg();
            mailBody.Body.MailBody = GetQuestionListMailBody();
            mailBody.Body.MailTo = ConfigurationManager.AppSettings["MailAddressTo"];
            mailBody.Body.Subjuect = "用户询问回复情况表——" + DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd");
            mailBody.Body.CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            QuestionListDA.SendMailQuestionList(mailBody);

        }

        /// <summary>
        /// 取得问题回复列表Mail的邮件内容
        /// </summary>
        /// <returns>
        /// Html脚本
        /// </returns>
        private static string GetQuestionListMailBody()
        {

            List<QuestionList> lQuestionList = QuestionListDA.GetQuestionList();

            WriteLog("\r\n" + DateTime.Now + "查询到" + DateTime.Now.ToString("yyyy-MM-dd") + "已回复记录数" + lQuestionList.Count.ToString() + "条......");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table align=center border=1 cellspacing = 0 cellspadding = 0>");
            //判断列表是否为空，不为空拼接
            if (lQuestionList.Count <= 0)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td bgcolor=#FFFFFF colspan=4>");
                sb.AppendLine("记录为空！");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");

            }
            else
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("    <td align=center>Product ID</td>");
                sb.AppendLine("    <td align =center>Product Name</td>");
                sb.AppendLine("    <td>Nick Name[Time]</td>");
                sb.AppendLine("    <td>Question</td>");
                sb.AppendLine("    <td>Reply</td>");
                sb.AppendLine("    <td>ReplyUser[Time]</td>");
                sb.AppendLine("    <td>Status</td>");
                sb.AppendLine("</tr>");

                foreach (QuestionList item in lQuestionList)
                {         
                    sb.AppendLine("    <tr>");
                    sb.AppendLine("    <td>" + item.ProductID + "</td>");
                    sb.AppendLine("    <td> " + item.ProductName + "</td>");
                    sb.AppendLine("    <td>" + item.NickName + "[" + item.CreateTime + "]</td>");
                    sb.AppendLine("    <td>" + item.Question + "</td>");
                    sb.AppendLine("    <td>" + item.Reply + "</td>");
                    sb.AppendLine("    <td>" + item.ReplyUserName + "[" + item.ReplyTime + "]</td>");
                    sb.AppendLine("    <td>" + GetDetailStatusName(item.Status.ToString()) + "</td>");
                    sb.AppendLine("</tr>");

                }
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                QuestionListDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }

        public static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
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

        #endregion

        #region 获取状态值
        /// <summary> 
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="val">传入的值</param>
        /// <returns>
        /// 未处理(O),已阅读(E),已回复(R),审核通过(A),审核不通过(D)
        /// </returns>
        private static string GetDetailStatusName(string status)
        {
            string statusName = "";
            if (status == null) return statusName;
            switch (status)
            {
                case "O":
                    statusName = "未处理";
                    break;
                case "E":
                    statusName = "已阅读";
                    break;
                case "R":
                    statusName = "已回复";
                    break;
                case "A)":
                    statusName = "审核通过";
                    break;
                case "D":
                    statusName = "审核不通过";
                    break;
                default:
                    break;
            }

            return statusName;
        }
        #endregion

    }
}
