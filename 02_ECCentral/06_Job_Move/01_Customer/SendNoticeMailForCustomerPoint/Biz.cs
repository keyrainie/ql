using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace SendNoticeMailForCustomerPoint
{
    public class Biz
    {
        private static int[] days = new int[] { 1, 7, 30 };//发送邮件到期时间的提前天数
        public void SendExpireEmailEmail(JobContext context)
        {
            string body = Template.Body;
            string subject = Template.Subject;

            foreach (int day in days)
            {
                int emails = SendEmailByDays(body, subject, day);
                JobContextMessage(string.Format("{0}天到期提醒邮件发送{1}封！", 
                    day, 
                    emails), 
                    context);
            }
            JobContextMessage("Stop",
                    context);
        }

        private static int SendEmailByDays(string body, string subject, int day)
        {
            List<ExpireEmail> expireEmailListByDay = DA.GetExpireIEmailList(day);
            if (expireEmailListByDay!=null)
            {
                foreach (var item in expireEmailListByDay)
                {
                    string tempBody = body;
                    string tempSubject = subject;
                    tempBody = tempBody.Replace("{CustomerID}", item.CustomerID)
                        .Replace("{InDate}", item.InDate.ToString("yyyy年MM月dd日"))
                        .Replace("{ExpireDate}", item.ExpireDate.ToString("yyyy年MM月dd日"))
                        .Replace("{AvailablePoint}", item.AvailablePoint.ToString());
                    DA.InsertEmail(item.Email, tempSubject, tempBody);
                }
                return expireEmailListByDay.Count;
            }
            return 0;
        }

        private void JobContextMessage(string msg, JobContext context)
        {
            if (context != null)
            {
                context.Message += msg + Environment.NewLine;
            }
            else
            {
                Console.WriteLine(msg);
                if (msg == "Stop")
                {
                    Console.ReadLine();
                }
            }
        }
    }
}