using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IPPOversea.InvoiceMgmt.PerMonthReport.DAL;
using IPPOversea.InvoiceMgmt.PerMonthReport.Compoents;
using System.Data;
using System.Transactions;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPPOversea.InvoiceMgmt.PerMonthReport.Biz
{
    public static class ReportBP
    {        
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);

        public static void DoWork(JobContext context)
        {
            var now = DateTime.Now;

            #region 修复数据Code
            if (context != null)
            {
                //从上下文中获取当前日期（用于修复数据使用，平时都为空）
                if (context.Properties.ContainsKey("CurrentDay"))
                {
                    now = Convert.ToDateTime(context.Properties["CurrentDay"]);
                }
            }
            #endregion
         
            if (now.Day != 1 && now.DayOfWeek != DayOfWeek.Monday)
            {
                return;
            }

            DateTime dtPeridTime;

            if (now.Day == 1)
            {
                dtPeridTime = now.AddMonths(-1);
            }
            else
            {
                dtPeridTime = now.AddDays(1 - now.Day);
            }

            var dateFrom = dtPeridTime.Date.ToString(Settings.ShortDateFormat);
            var dateTo = now.Date.AddSeconds(-1).ToString(Settings.LongDateFormat);

            var beginMonthDate = now.AddDays(1 - now.Day).Date;
            var beginMonth = beginMonthDate.ToString(Settings.ShortDateFormat);
            var endLastMonth = beginMonthDate.AddSeconds(-1).ToString(Settings.LongDateFormat);

            try
            {
                var ds = ReportDA.ARAPReport(dateFrom, dateTo, beginMonth, endLastMonth);
                if (!HasMoreRow(ds))
                {
                    return;   
                }

                StringBuilder sb = new StringBuilder(1000);
                sb.Append("<table border=1 ><font color=blue></font>");
                sb.Append("<tr><td colspan=5 align=center>收款方式区分收入" + dtPeridTime.ToString("yyyy-MM-dd") + "～" + dateTo + "</td></tr>");
                sb.Append("<tr>");
                sb.Append("<td>付款方式编号</td>");
                sb.Append("<td>付款方式名称</td>");
                sb.Append("<td>销售收入(不含税)</td>");
                sb.Append("<td>月初应收款余额(含税)</td>");
                sb.Append("<td>截至上周日晚应收款余额(含税)</td>");
                sb.Append("<td>截至前一天晚上的应收款余额(含税)</td>");
                sb.Append("</tr>");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + row["PayTypeSysNo"].ToString() + "</td>");
                    sb.Append("<td>" + row["PayTypeName"].ToString() + "</td>");
                    sb.Append("<td>" + (TrimDecimalNull(row["TotalAmt"]) / 1.17m).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                    sb.Append("<td>" + TrimDecimalNull(row["OriginARAmt"]).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                    sb.Append("<td>" + TrimDecimalNull(row["ARAmt"]).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                    sb.Append("<td>" + TrimDecimalNull(row["OrderAmt"]).ToString(Settings.DecimalFormatWithGroup) + "</td>");

                    sb.Append("</tr>");
                }
                sb.Append("<tr>");
                sb.Append("<td colspan=2 align=center>合计：</td>");

                sb.Append("<td>" + (TrimDecimalNull(ds.Tables[0].Compute("sum(TotalAmt)", null)) / 1.17m).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                sb.Append("<td>" + (TrimDecimalNull(ds.Tables[0].Compute("sum(OriginARAmt)", null))).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                sb.Append("<td>" + (TrimDecimalNull(ds.Tables[0].Compute("sum(ARAmt)", null))).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                sb.Append("<td>" + (TrimDecimalNull(ds.Tables[0].Compute("sum(OrderAmt)", null))).ToString(Settings.DecimalFormatWithGroup) + "</td>");
                
                sb.Append("</tr>");
                sb.Append("</table>");

                sb.Append("<br /> <b>" + dtPeridTime.ToString("yyyy-MM-dd") + "～" + dateTo + "理光客户积分数据:</b>");

                DataSet dsPoint = ReportDA.LGPoint(dateFrom, dateTo);
                if (HasMoreRow(dsPoint))
                {
                    if (TrimDecimalNull(dsPoint.Tables[0].Rows[0][0]) != Settings.DecimalNull)
                        sb.Append("<br />使用积分:").Append(TrimDecimalNull(dsPoint.Tables[0].Rows[0][0]));
                    else
                        sb.Append("<br />使用积分:0");
                    if (TrimIntNull(dsPoint.Tables[0].Rows[0][1]) != Settings.IntNull)
                        sb.Append("<br />作废积分:").Append(TrimIntNull(dsPoint.Tables[0].Rows[0][1]));
                    else
                        sb.Append("<br />作废积分:0");
                }
                else
                {
                    sb.Append("<br />使用积分:0");
                    sb.Append("<br />作废积分:0");
                }

                SendMail(Settings.EmailAddress, "按每种收款方式区分收入汇总报表_理光积分情况", sb.ToString(), 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Help Method
        public static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }

        public static void SendMail(string mailto, string subject, string mailBody, int status)
        {
            MailDA.SendEmail(mailto, subject, mailBody, status);
        }

        public static bool HasMoreRow(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static decimal TrimDecimalNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return Settings.DecimalNull;
            }
            else
            {
                return decimal.Parse(obj.ToString());
            }
        }

        public static int TrimIntNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return Settings.IntNull;
            }
            else
            {
                return Int32.Parse(obj.ToString());
            }
        }
        #endregion 
    }
}
