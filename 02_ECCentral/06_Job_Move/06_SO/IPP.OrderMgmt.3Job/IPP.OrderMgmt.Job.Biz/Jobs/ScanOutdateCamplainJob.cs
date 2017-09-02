using System;
using System.Text;
using System.Data;
using System.Configuration;

using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.DataAccess;

using IPP.OrderMgmt.Job.Biz.Resource;
using ECCentral.BizEntity.Common;

namespace IPP.OrderMgmt.Job.Biz
{
    public class ScanOutdateCamplainJob:JobInstance
    {
        /// <summary>
        /// this job can be complied.
        /// </summary>
        /// <param name="context"></param>
        public override void Run(JobContext context)
        {
            base.Run(context);
            // 每天上班前发送上午要超过3天的Case邮件，中午发送下午要超过3天的Case邮件
            var time = DateTime.Now.TimeOfDay;
            var beginTime1 = new TimeSpan(7, 00, 00);
            var endTime1 = new TimeSpan(7, 30, 00);
            var beginTime2 = new TimeSpan(12, 15, 00);
            var endTime2 = new TimeSpan(12, 45, 00);

            //在早上7:00-7:30 和 中午12:15-12:45之间运行扫描程序
            if ((time >= beginTime1 && time < endTime1) || (time >= beginTime2 && time < endTime2))
            {
                SendWaitingHandlePostIncome(context);
                if (time >= beginTime1 && time < endTime1)
                {
                    context.Message += StringResource.SendAddPointMailJobMNMsg + "\r\n";
                    ScanOutdateCase("AM", context);
                }
                else
                {
                    context.Message += StringResource.SendAddPointMailJobANMsg + "\r\n";
                    ScanOutdateCase("PM", context);
                }
            }
            else
            {
                context.Message += StringResource.ScanOutdateCamplainJobStartTimeNote + "\r\n";
            }
        }

        private void SendWaitingHandlePostIncome(JobContext context)
        {
            var command = DataCommandManager.GetDataCommand("WaitingHandlePostIncome");
            command.SetParameterValue("HandleStatus", (int)CommonEnum.PostIncomeOrderStatus.No);
            command.SetParameterValue("ConfirmStatus", (int)CommonEnum.SOPostIncomeStatus.Confirmed);
            command.SetParameterValue("CompanyCode", this.CompanyCode);
            DataSet ds = command.ExecuteDataSet();
            string mailBody = string.Empty;
            StringBuilder sb1 = new StringBuilder();

            if (Util.HasMoreRow(ds)) // 前一天截至当前待处理的邮政电汇收款单记录
            {
                context.Message += string.Format(StringResource.ScanOutdateCamplainJobWorkingCount,
                    ds.Tables[0].Rows.Count)+ "\r\n";
                            
                mailBody = @Resource.StringResource.WaitingHandlePostIncomeMailBody;
				
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sb1.Append("<tr>");
                    sb1.Append("<td>" + dr["SysNo"].ToString() + "</td>");
                    sb1.Append("<td>" + dr["SoSysNo"].ToString() + "&nbsp" + "</td>");
                    sb1.Append("<td>" + Util.TrimDecimalNull(dr["IncomeAmt"]).ToString(AppConst.DecimalFormat) + "&nbsp" + "</td>");
                    sb1.Append("<td>" + dr["PayUser"].ToString() + "&nbsp" + "</td>");
                    sb1.Append("<td>" + Util.TrimDateNull(dr["CreateDate"]).ToString(AppConst.DateFormatLong) + "&nbsp" + "</td>");
                    sb1.Append("<td>" + Util.TrimDateNull(dr["IncomeDate"]).ToString(AppConst.DateFormat) + "&nbsp" + "</td>");
                    sb1.Append("<td>" + dr["IncomeBank"].ToString() + "</td>");
                    sb1.Append("</tr>");
                }
                //mail.MailAddress = AppConfig.SendMailToCSHelpdesk;
                Util.SendEmail(CompanyCode, new MailInfo
                {
                    ToName = ConfigurationManager.AppSettings["SendMailToCSHelpdesk"],
                    Subject = StringResource.PostIncomeMailSub,
                    Body = string.Format(mailBody, sb1.ToString()),
                    IsAsync = true,
                    IsInternal = true
                });
                context.Message += StringResource.PostIncomeMailSuccessfulMsg + "\r\n";
            }
        }

        /// <summary>
        /// 扫描达到或即将达到3个工作日的未处理投诉
        /// </summary>
        private void ScanOutdateCase(string timeStr,JobContext context)
        {
            context.Message += StringResource.ScanOutdateCaseWorkingStart + "\r\n";
            // 每天上班前发送上午要超过3天的Case邮件，中午发送下午要超过3天的Case邮件
            // 在早上7:00-7:30 和 中午12:15-12:45之间运行扫描程序
            DateTime outDateTime;


            // 如果在早上发送，截止时间在下午1点之前
            if (timeStr == "AM")
            {
                outDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) + new TimeSpan(13, 0, 0);
            }
            else // 如果在中午发送，截止时间在下午17点半
            {
                outDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) + new TimeSpan(17, 30, 0);
            }

            // 3个工作日以前的时间
            DateTime caseTime = AddWorkMinute(outDateTime, -1 * 60 * 9 * 3);
            var command = DataCommandManager.GetDataCommand("ScanOutdateCase");
            command.SetParameterValue("CreateDate", caseTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.ReplaceParameterValue("#Status", 
                ((int)CommonEnum.ComplainStatus.Abandoned).ToString() + "," 
                + ((int)CommonEnum.ComplainStatus.Handled).ToString());
            command.SetParameterValue("CompanyCode", CompanyCode);
            var ds = command.ExecuteDataSet();
            if (Util.HasMoreRow(ds)) // 有即将到期的Case数据
            {
                context.Message += string.Format(StringResource.ScanOutdateCaseWorkingCount, 
                    ds.Tables[0].Rows.Count) + "\r\n";
                var mailBody = @Resource.StringResource.ScanOutdateCaseMailBody;
                SendComplainMail(mailBody, ds.Tables[0], context);
            }
            else
            {
                context.Message += StringResource.ScanOutdateCaseNoResult + "\r\n";
            }
        }

        private void SendComplainMail(string tempMailBody, DataTable dt, JobContext context)
        {
            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["CompanyCustomer"].ToString() == "1")
                {
                    sb1.Append("<tr>");
                    sb1.Append("<td>" + dr["ComplainSysNo"].ToString() + "</td>");
                    sb1.Append("<td>" + dr["SOSysNo"].ToString() + "&nbsp" + "</td>");
                    sb1.Append("<td>" + dr["Subject"].ToString() + "&nbsp" + "</td>");
                    sb1.Append("<td>" + DateTime.Parse(dr["CreateDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                    sb1.Append("</tr>");
                }
                else
                {
                    sb2.Append("<tr>");
                    sb2.Append("<td>" + dr["ComplainSysNo"].ToString() + "</td>");
                    sb2.Append("<td>" + dr["SOSysNo"].ToString() + "&nbsp" + "</td>");
                    sb2.Append("<td>" + dr["Subject"].ToString() + "&nbsp" + "</td>");
                    sb2.Append("<td>" + DateTime.Parse(dr["CreateDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                    sb2.Append("</tr>");
                }
            }

            var eMailBody = string.Empty;
            var mailTo = string.Empty;
            if (sb1.Length > 0)
            {
                Util.SendEmail(this.CompanyCode,
                    new MailInfo
                {
                    ToName = ConfigurationManager.AppSettings["AstraZenecaComplainOutdatedAlertMailList"],
                    Subject = StringResource.ComplainMailSubject,
                    Body = string.Format(tempMailBody, sb1.ToString()),
                    IsAsync = true,
                    IsInternal = true
                });
                context.Message += string.Format(StringResource.ComplainMaiSendMsg + "(AstraZeneca)", mailTo) + "\r\n";
            }

            if (sb2.Length > 0)
            {
                Util.SendEmail(this.CompanyCode,
                    new MailInfo
                {
                    ToName = ConfigurationManager.AppSettings["ComplainOutdatedAlertMailList"],
                    Subject = StringResource.ComplainMailSubject,
                    Body = string.Format(tempMailBody, sb2.ToString()),
                    IsAsync = true,
                    IsInternal = true
                });
                context.Message += string.Format(StringResource.ComplainMaiSendMsg, mailTo) + "\r\n";
            }       
        }

        // 工作时间的定义
        private static TimeSpan WorkStart = new TimeSpan(8, 30, 0);
        private static TimeSpan WorkEnd = new TimeSpan(17, 30, 0);
        private DateTime AddWorkMinute(DateTime time, double mins)
        {
            if (mins == 0) return time;

            DateTime currentDay = DateTime.Parse(time.ToString("yyyy-MM-dd"));

            // 判断传入的起始时间是否是工作日
            if (time.DayOfWeek == DayOfWeek.Saturday || // 如果是周六
                time.DayOfWeek == DayOfWeek.Sunday) // 如果是周日
            {
                // 如果是加就往前推一天，否则往后倒一天
                time = (mins > 0) ? currentDay.AddDays(1) : currentDay.AddSeconds(-1);
                return AddWorkMinute(time, mins);
            }

            // 计算当前日期的工作时间
            double tempMin = 0;
            if (mins > 0)
            {
                tempMin = CalWorkMinute(time, currentDay.Add(WorkEnd), 0);
            }
            else
            {
                tempMin = CalWorkMinute(currentDay.Add(WorkStart), time, 0);
            }

            if (Math.Abs(mins) > tempMin) // 当天工作时间不能满足
            {
                time = (mins > 0) ? currentDay.AddDays(1) : currentDay.AddSeconds(-1);
                return AddWorkMinute(time, Math.Sign(mins) * (Math.Abs(mins) - tempMin));
            }
            else // 当天时间可以满足
            {
                if (mins > 0)
                {
                    if (time.TimeOfDay > WorkStart) //工作时间之前
                    {
                        return time.AddMinutes(mins);
                    }
                    else
                    {
                        return currentDay.Add(WorkStart).AddMinutes(mins);
                    }
                }
                else
                {
                    if (time.TimeOfDay < WorkEnd) //工作时间以后
                    {
                        return time.AddMinutes(mins);
                    }
                    else
                    {
                        return currentDay.Add(WorkEnd).AddMinutes(mins);
                    }
                }
            }
        }

        private double CalWorkMinute(DateTime startTime, DateTime endTime, double totalMinutes)
        {
            // 开始时间>=终止时间
            if (startTime >= endTime) return totalMinutes;

            // 判断传入的开始时间是否是工作日
            if (startTime.DayOfWeek == DayOfWeek.Saturday || // 如果是周六
                startTime.DayOfWeek == DayOfWeek.Sunday) // 如果是周日
            {
                startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
                return CalWorkMinute(startTime, endTime, totalMinutes);
            }

            TimeSpan dayStartTime = startTime.TimeOfDay;
            TimeSpan dayEndTime = endTime.TimeOfDay;
            if (startTime.ToString("yyyyMMdd") != endTime.ToString("yyyyMMdd")) // 开始时间和结束时间不是同一天
            {
                dayEndTime = WorkEnd;
            }

            if (dayStartTime > WorkEnd) // 开始时间在下班以后
            {
                startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
                return CalWorkMinute(startTime, endTime, totalMinutes);
            }

            var allMinutes = ((TimeSpan)(WorkEnd - WorkStart)).TotalMinutes;
            var beforeStartMinutes = ((TimeSpan)(dayStartTime - WorkStart)).TotalMinutes;
            var aboveEndMinutes = ((TimeSpan)(WorkEnd - dayEndTime)).TotalMinutes;

            var dayMinutes = allMinutes - Math.Max(Math.Min(beforeStartMinutes, allMinutes), 0) -
                                              Math.Max(Math.Min(aboveEndMinutes, allMinutes), 0);

            totalMinutes += dayMinutes;

            startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
            return CalWorkMinute(startTime, endTime, totalMinutes);
        }
    }
}
