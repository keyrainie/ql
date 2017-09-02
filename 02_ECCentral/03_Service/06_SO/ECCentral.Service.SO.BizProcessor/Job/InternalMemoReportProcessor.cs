using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.SO.BizProcessor.Job
{
    [VersionExport(typeof(InternalMemoReportProcessor))]
    public class InternalMemoReportProcessor
    {
        #region Member

        private string m_emailTO;
        private string m_emailCC;
        private string m_companyCode;

        #endregion


        public void Run(string emailTo,string emailCC,string companyCode)
        {
            m_emailTO = emailTo;
            m_emailCC = emailCC;
            m_companyCode = companyCode;

            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();

            endTime = DateTime.Now;
            //获取多少天前数据
            int timeSpan = 0;
            if (!int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_InternalMemoReport_TimeSpan"), out timeSpan))
            {
                timeSpan = 60;
            }

            startTime = endTime.AddDays(-timeSpan);

            var imemoList = ObjectFactory<ISODA>.Instance.GetInternalMemoReportList(startTime, DateTime.Now, m_companyCode);
            if (imemoList != null && imemoList.Count > 0)
            {
                List<InternalMemoReport> reports = ConvertReports(imemoList);
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
        void SendMail(List<InternalMemoReport> reports)
        {
            KeyValueVariables replaceContent = new KeyValueVariables();
            replaceContent.AddKeyValue("#Time#", DateTime.Now.ToString());

            DataTable tableList = new DataTable();
            tableList.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("UserName"),
                new DataColumn("Count"),
                new DataColumn("ResolvedCount"),
                new DataColumn("UnResolvedCount"),
                new DataColumn("ResolvedRate")
            });
            foreach (var item in reports)
            {
                tableList.Rows.Add(item.UserName, item.Count, item.ResolvedCount, item.UnResolvedCount, item.ResolvedRate);
            }
            KeyTableVariables tableContent = new KeyTableVariables();
            tableContent.Add("tbData", tableList);

            ExternalDomainBroker.SendEmail(m_emailTO, m_emailCC, "", "SO_InternalMemoReport", replaceContent, tableContent, false, true);
        }

        /// <summary>
        /// 将查询转为记录
        /// </summary>
        /// <param name="queryData">查询结果</param>
        /// <returns>记录列表</returns>
        List<InternalMemoReport> ConvertReports(List<SOInternalMemoInfo> queryData)
        {
            List<InternalMemoReport> reports = new List<InternalMemoReport>();
            //去掉重复数据
            var distinctQuery = queryData.Distinct();
            var users = distinctQuery.GroupBy(item => item.CreateUserSysNo);

            foreach (var user in users)
            {
                InternalMemoReport report = new InternalMemoReport();
                report.UserSysNo = user.Key;
                report.UserName = ExternalDomainBroker.GetUserInfoBySysNo(user.First().CreateUserSysNo).UserDisplayName;

                List<int> ProcessedSOList = new List<int>();

                foreach (var imemo in user.OrderBy(item => item.LogTime.Value).ToList())
                {
                    if (ProcessedSOList.Exists(item => item == imemo.SOSysNo))
                    {
                        continue;
                    }

                    if (distinctQuery.Count(item => (item.LogTime.Value > imemo.LogTime.Value
                                                && item.LogTime.Value.AddDays(-3) < imemo.LogTime.Value)
                                                && item.SOSysNo == imemo.SOSysNo) == 0)
                    {
                        report.ResolvedCount++;
                    }
                    else
                    {
                        report.UnResolvedCount++;
                    }
                    ProcessedSOList.Add(imemo.SOSysNo);
                }

                reports.Add(report);
            }

            return reports;
        }

        /// <summary>
        /// 用于暂时保存数据的类，没有实际业务价值
        /// </summary>
        class InternalMemoReport
        {
            public int UserSysNo { get; set; }

            public string UserName { get; set; }

            public int ResolvedCount { get; set; }

            public int UnResolvedCount { get; set; }

            public int Count
            {
                get
                {
                    return this.ResolvedCount + this.UnResolvedCount;
                }
            }

            public string ResolvedRate
            {
                get
                {
                    double rate = ResolvedCount * 1.0 / this.Count;
                    return rate.ToString("P");
                }
            }
        }
    }
}
