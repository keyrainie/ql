using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ThirdPart.JobV31.BusinessEntities.IngramMicro;
using IPP.ThirdPart.JobV31.Dac.IngramMicro;
using System.Configuration;
using IPP.OrderMgmt.CsReportsJob.Utilities;
using ECCentral.BizEntity.Common;

namespace IPP.OrderMgmt.CsReportsJob.Biz.InternalMemo
{
    public class InternalMemoBiz : IJobAction
    {
        public void Run(JobContext context)
        {
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();

            object InternalMemo_EndTime = ConfigurationManager.AppSettings["InternalMemo_EndTime"];
            object InternalMemo_TimeSpan = ConfigurationManager.AppSettings["InternalMemo_TimeSpan"];

            endTime = InternalMemo_EndTime == null ? DateTime.Now : Convert.ToDateTime(InternalMemo_EndTime);
            startTime = InternalMemo_TimeSpan == null ? endTime.AddDays(-60) : endTime.AddDays(-1 * Convert.ToInt32(InternalMemo_TimeSpan));

            List<InternalMemoEntity> ImemoList = InternalMemoDA.GetInternalMemoList(startTime, endTime);
            if (ImemoList != null && ImemoList.Count > 0)
            {
                List<InternalMemoReport> reports = GetReports(ImemoList);
                SendMail(reports);
            }
        }

        /// <summary>
        /// 操作人、
        /// 处理的订单总数量、
        /// 已解决的订单数量、
        /// 未解决的订单数量、
        /// 解决率（已解决的数量/订单总数量*100%）
        /// </summary>
        /// <param name="reports"></param>
        public void SendMail(List<InternalMemoReport> reports)
        {
            MailInfo mail = new MailInfo();

            mail.Body = "<table border=1 cellpadding='0' cellspacing='0' ><tr><th>User</th><th>Count</th><th>Resolved</th><th>UnResolved</th><th>Rate</th></tr>";

            foreach (var report in reports)
            {
                mail.Body += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>"
                    ,report.UserName, report.Count, report.ResolvedCount, report.UnResolvedCount, report.ResolvedRate);
            }

            mail.Body += "</table>";

            mail.Subject = string.Format("订单跟进日志的解决率报表-{0}",DateTime.Now.ToString());
            mail.ToName = ConfigurationManager.AppSettings["IntenerMemoEmail"].ToString();
            mail.FromName = ConfigurationManager.AppSettings["IntenerMemoFrom"].ToString();
            mail.CCName = ConfigurationManager.AppSettings["IntenerMemoCC"].ToString();
            mail.IsAsync = true;
            mail.IsInternal = true;

            MailHelper.SendEmail(mail);

        }

        public List<InternalMemoReport> GetReports(List<InternalMemoEntity> ImemoList)
        {
            List<InternalMemoReport> reports = new List<InternalMemoReport>();
            var users = ImemoList.GroupBy(item => item.UserSysNo).ToList();

            foreach (var user in users)
            {
                InternalMemoReport report = new InternalMemoReport();
                report.UserSysNo = user.Key;
                report.UserName = user.ToList()[0].UserName;

                List<int> ProcessedSOList = new List<int>();
                
                foreach (var imemo in user.OrderBy(item => item.InDate).ToList())
                {
                    if(ProcessedSOList.Exists(item => item == imemo.SOSysNo))
                    {
                        continue;
                    }
                    
                    if (!ImemoList.Exists(item => (item.InDate > imemo.InDate
                                                && item.InDate.AddDays(-3) < imemo.InDate)
                                                && item.SOSysNo == imemo.SOSysNo))
                    {
                        report.ResolvedCount++;
                        ProcessedSOList.Add(imemo.SOSysNo);
                    }
                    else
                    {
                        report.UnResolvedCount++;
                        ProcessedSOList.Add(imemo.SOSysNo);
                    }
                }

                reports.Add(report);
            }

            return reports;
        }
    }
}
