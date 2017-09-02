using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;

namespace IPP.Oversea.CN.CustomerMgmt.SendRMAEveryDayList
{
    public class Biz : IJobAction
    {
        static ILog log = null;
        private JobContext _context = null;
        public void ShowMessage(string message)
        {
            if (_context == null)
            {
                Console.WriteLine(message);
            }
            else
            {
                _context.Message += message + Environment.NewLine;
            }
        }
        public void DoJob()
        {
            ILog log = LogerManger.GetLoger();
            ShowMessage("Job Start");
            string MailSubject = GetMailTemplate("MailSubjectTemplate.txt");
            string MailBody = GetMailTemplate("MailBodyTemplate.txt");
            string HtmlReports = GetItemHtml(MailBody, @"\[reports\](.+?)\[\/reports\]");
            string HtmlRows = GetItemHtml(MailBody, @"\[rows\](.+?)\[\/rows\]");

            List<DBModelPM> PMs = GetDBModelPM();
            if (PMs == null)
            {
                ShowMessage("e-mail list is null,Job End");
                return;
            }
            int mailNum = 0;

            Dictionary<int, string> dicEmail = GetEmailGroup();
            string ccEmail = string.Empty;
            foreach (DBModelPM PM in PMs)
            {
                ccEmail = string.Empty;
                if (PM == null || string.IsNullOrEmpty(PM.email))
                {
                    continue;
                }
                if(dicEmail!=null){
                    bool b= dicEmail.Keys.Contains(PM.PMGroupSysNo);
                    if (b)
                    {
                        ccEmail = dicEmail[PM.PMGroupSysNo];
                    }
                }                
                mailNum += SendMail(PM.pmusersysno, PM.username, PM.email, ccEmail, MailSubject, MailBody, HtmlReports, HtmlRows);
            }
            ShowMessage(mailNum + " e-mail has been sent,Job End");
        }

        private Dictionary<int, string> GetEmailGroup() {
            string value = ConfigurationManager.AppSettings["EmailGroup"];
            int groupId;
            Dictionary<int, string> dicEmail = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(value))
            {
                string[] emails = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in emails)
                {
                    string[] em = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (em == null || em.Length != 2)
                    {
                        break;
                    }
                    else
                    {
                        bool b = int.TryParse(em[0], out groupId);
                        if (b)
                        {
                            dicEmail.Add(groupId, em[1]);
                        }
                    }
                }
            }
            return dicEmail;
        }

        private static int SendMail(int pmsysno, string pmUserName, string email, string ccEmail, string mailSubject, string mailBody, string htmlReports, string htmlRows)
        {
            //if (email!="daisy.l.bian@newegg.com.cn")
            //{
            //    return 0;
            //}
            StringBuilder sbReports = new StringBuilder();

            int num = 0, totalNum = 0;
            decimal cost = 0, totalCost = 0;
            string repostHtml = "";

            #region reports
            DBParam dbParam = new DBParam();
            #region 1
            dbParam.reportName = "今日接受件";
            dbParam.CommandName = "RMARequestStatus";
            dbParam.today = DateTime.Today;
            dbParam.yesterday = DateTime.Today.AddDays(-1);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)RMARequestStatus.Handling;
            dbParam.status = 1;
            dbParam.statusName = "处理中";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{RecRegisterNum}", num.ToString());
            mailBody = mailBody.Replace("{RecRegisterCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #region 2
            dbParam = new DBParam();
            dbParam.reportName = "当日返还物品清单";
            dbParam.CommandName = "RMAOutBoundStatus";
            dbParam.today = DateTime.Today;
            dbParam.yesterday = DateTime.Today.AddDays(-1);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)RMAOutBoundStatus.Responsed;
            dbParam.status = 3;
            dbParam.statusName = "全部返还";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{RevertedNum}", num.ToString());
            mailBody = mailBody.Replace("{RevertedCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #region 3
            dbParam = new DBParam();
            dbParam.reportName = "当日退款物品清单";
            dbParam.CommandName = "RMARefundStatus";
            dbParam.today = DateTime.Today;
            dbParam.yesterday = DateTime.Today.AddDays(-1);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)ERefundStatus.Refunded;
            dbParam.status = 2;
            dbParam.statusName = "已退款";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{RefundedNum}", num.ToString());
            mailBody = mailBody.Replace("{RefundedCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #region 4
            dbParam = new DBParam();
            dbParam.reportName = "当日发新物品清单";
            dbParam.CommandName = "RMARevertStatus";
            dbParam.today = DateTime.Today;
            dbParam.yesterday = DateTime.Today.AddDays(-1);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)RMARevertStatus.Reverted;
            //dbParam.status1 = (int)RMANewProductStatus.Origin;
            dbParam.status = 1;
            dbParam.status1 = 0;
            dbParam.statusName = "非换货";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{RevertedNewNum}", num.ToString());
            mailBody = mailBody.Replace("{RevertedNewCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #region 5
            dbParam = new DBParam();
            dbParam.reportName = "当日退货入库清单";
            dbParam.CommandName = "RMAReturnStatus";
            dbParam.today = DateTime.Today;
            dbParam.yesterday = DateTime.Today.AddDays(-1);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)RMAReturnStatus.Returned;
            dbParam.status = 1;
            dbParam.statusName = "已退正式库";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{ReturnedNum}", num.ToString());
            mailBody = mailBody.Replace("{ReturnedCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #region 6
            dbParam = new DBParam();
            dbParam.reportName = "超时未结束物品清单";
            dbParam.CommandName = "RMARequestStatus21";
            dbParam.daybefore21 = DateTime.Today.AddDays(-21);
            dbParam.PMUserSysNo = pmsysno;
            dbParam.PMUserName = pmUserName;
            //dbParam.status = (int)RMARequestStatus.Handling;
            dbParam.status = 1;
            dbParam.statusName = "处理中";

            repostHtml = GetReportHtml(dbParam, htmlReports, htmlRows, out num, out cost);
            sbReports.Append(repostHtml);
            mailBody = mailBody.Replace("{OverTimeNum}", num.ToString());
            mailBody = mailBody.Replace("{OverTimeCost}", cost.ToString("#,###,###,##0.00"));
            totalNum += num;
            totalCost += cost;
            #endregion

            #endregion

            StringBuilder sbBody = new StringBuilder(mailBody);
            sbBody.Replace("[reports]" + htmlReports + "[/reports]", sbReports.ToString());
            sbBody.Replace("{year}", DateTime.Now.Year.ToString());
            sbBody.Replace("{month}", DateTime.Now.Month.ToString());
            sbBody.Replace("{day}", DateTime.Now.Day.ToString());
            sbBody.Replace("{allRegisterNum}", totalNum.ToString());
            sbBody.Replace("{allRegisterCost}", totalCost.ToString("#,###,###,##0.00"));
            mailBody = sbBody.ToString();

            return InsertAsyncEmailInternal(email, mailSubject, mailBody, 0, ccEmail, "", 1, "");
        }

        #region GetReportHtml
        private static string GetReportHtml(DBParam dbParam, string htmlReports, string htmlRows, out int num, out decimal cost)
        {
            StringBuilder sbReport = new StringBuilder();
            List<DBModel> reports = GetDBModel(dbParam);
            htmlReports = htmlReports.Replace("{ListName}", dbParam.reportName);
            htmlReports = htmlReports.Replace("{RegisterNum}", reports.Count.ToString());
            decimal OverTimeCost = 0;
            foreach (DBModel dbm in reports)
            {
                StringBuilder dbItem = new StringBuilder(htmlRows);
                dbItem.Replace("{SysNo}", dbm.SysNo.ToString());

                dbItem.Replace("{productid}", dbm.productid);
                dbItem.Replace("{productname}", dbm.productname);
                dbItem.Replace("{CustomerDesc}", dbm.CustomerDesc);

                dbItem.Replace("{statusName}", dbParam.statusName);
                //DBModelPMInfo pmInfo = null;
                //if (dbm.PMUserSysNo!=null)
                //{
                //    pmInfo = GetDBModelPMInfo(dbm.PMUserSysNo.Value);                    
                //}
                //if (pmInfo != null)
                //{
                //    dbItem.Replace("{pmName}", pmInfo.UserName);
                //}
                //else
                //{
                //    dbItem.Replace("{pmName}", "");
                //}
                dbItem.Replace("{pmName}", dbParam.PMUserName);
                dbItem.Replace("{VendorName}", dbm.VendorName);
                dbItem.Replace("{LastVendorSysNo}", dbm.LastVendorSysNo.ToString());
                dbItem.Replace("{Cost}", (dbm.Cost != null) ? dbm.Cost.Value.ToString("#########0.00") : "&nbsp;&nbsp;");
                //
                OverTimeCost += (dbm.Cost != null) ? dbm.Cost.Value : 0;
                sbReport.Append(dbItem);

                dbItem.Length = 0;
            }
            htmlReports = htmlReports.Replace("{RegisterCost}", OverTimeCost.ToString("#,###,###,##0.00"));
            num = reports.Count;
            cost = OverTimeCost;
            return htmlReports.Replace("[rows]" + htmlRows + "[/rows]", sbReport.ToString());
        }
        #endregion

        #region getModel
        private static int InsertAsyncEmailInternal(string MailAddress, string MailSubject, string MailBody, int Status, string CCMailAddress, string BCMailAddress, int Priority, string Department)
        {
            int num = 0;

            DataCommand dc = DataCommandManager.GetDataCommand("InsertAsyncEmailInternal");
            dc.SetParameterValue("@MailAddress", MailAddress);
            dc.SetParameterValue("@MailSubject", MailSubject);
            dc.SetParameterValue("@MailBody", MailBody);
            dc.SetParameterValue("@Status", Status);
            dc.SetParameterValue("@CCMailAddress", CCMailAddress);
            dc.SetParameterValue("@BCMailAddress", BCMailAddress);
            dc.SetParameterValue("@Priority", Priority);
            dc.SetParameterValue("@Department", Department);

            num = dc.ExecuteNonQuery();


            return num;
        }

        private static DBModelPMInfo GetDBModelPMInfo(int userID)
        {
            DBModelPMInfo pmInfo = null;

            DataCommand dc = DataCommandManager.GetDataCommand("GetPMName");

            dc.SetParameterValue("@userid", userID);

            pmInfo = dc.ExecuteEntity<DBModelPMInfo>();


            return pmInfo;
        }

        private static List<DBModelPM> GetDBModelPM()
        {
            List<DBModelPM> list = null;

            DataCommand dc = DataCommandManager.GetDataCommand("GetPMList");

            list = dc.ExecuteEntityList<DBModelPM>();


            return list;
        }

        private static List<DBModel> GetDBModel(DBParam dbParam)
        {
            List<DBModel> result = null;

            DataCommand dc = DataCommandManager.GetDataCommand(dbParam.CommandName);
            dc.SetParameterValue("@PMUserSysNo", dbParam.PMUserSysNo);
            if (dbParam.today != null)
            {
                dc.SetParameterValue("@today", dbParam.today);
            }
            if (dbParam.yesterday != null)
            {
                dc.SetParameterValue("@yesterday", dbParam.yesterday);
            }
            if (dbParam.daybefore21 != null)
            {
                dc.SetParameterValue("@daybefore21", dbParam.daybefore21);
            }
            dc.SetParameterValue("@status", dbParam.status);
            if (dbParam.status1 != null)
            {
                dc.SetParameterValue("@status1", dbParam.status1);
            }

            result = dc.ExecuteEntityList<DBModel>();


            return result;
        }
        #endregion

        #region gethtml
        private static string GetItemHtml(string strHtml, string pattern)
        {
            Match m = Regex.Match(strHtml, pattern, RegexOptions.Singleline);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetMailTemplate(string txtPath)
        {
            string Template = "";
            string currentDir = System.Threading.Thread.GetDomain().BaseDirectory;
            txtPath = txtPath.Replace("/", "\\").Trim('\\');
            txtPath = Path.Combine(currentDir, string.Format("{0}", txtPath));
            FileStream fs = new FileStream(txtPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            try
            {
                Template = sr.ReadToEnd();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
                fs.Close();
            }
            return Template;
        }
        #endregion

        #region IJobAction Members

        public void Run(JobContext context)
        {
            string s = ConfigurationManager.AppSettings["LogFileName"];
            _context = context;
            ShowMessage(s);

            DoJob();


            ShowMessage("this task proceseed complete");
        }

        #endregion
    }
}
