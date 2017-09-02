using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroAutoConfirm.DAL;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using System.IO;
using ZeroAutoConfirm.Model;
using ZeroAutoConfirm.Compoents;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Common;
using ZeroAutoConfirm.Utilties;
using System.Collections;

namespace ZeroAutoConfirm.Biz
{
    public static class ZeroConfirmBP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);
        #endregion

        public static void DoWork(AutoResetEvent are)
        {
            //获取前一日已出库，未确认的SOIncomeNo
            OnShowInfo("开始获取数据");
            var getData = AutoConfirm.GetConfirmData(Settings.InitialDate);

            if (getData != null && getData.Count > 0)
            {
                List<SOIncomeInfo> lstSOIncome = new List<SOIncomeInfo>();
                getData.ForEach(x =>
                {
                    lstSOIncome.Add(new SOIncomeInfo
                    {
                        SysNo = x.SysNo
                    });
                });
                OnShowInfo("开始呼叫服务，确认SOIncome记录");

                ZeroConfirmSOIncomeJobResp response = new ZeroConfirmSOIncomeJobResp();

                response = CallRestfulService(lstSOIncome);

                SendEmailForData(response);
            }
            else
            {
                OnShowInfo("没有需要确认的SOIncome记录");
            }

            are.Set();
        }

        #region Assistant Method
        public static void SendMail(string subject, string mailBody, int status)
        {
            MailDA.SendEmail(Settings.EmailAddress, subject, mailBody, status);
        }

        private static ZeroConfirmSOIncomeJobResp CallRestfulService(List<SOIncomeInfo> lstSOIncome)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            string relativeUrl = "/SOIncome/BatchConfirmJob";
            ZeroConfirmSOIncomeJobResp response = null;
            List<int> lstSOIncomeSysNo = new List<int>();
            
            foreach (SOIncomeInfo item in lstSOIncome)
            {
                lstSOIncomeSysNo.Add(item.SysNo.Value);
            }

            var ar = client.Update<ZeroConfirmSOIncomeJobResp>(relativeUrl, lstSOIncomeSysNo, out response, out error);

            var messageBuilder = new StringBuilder();
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    messageBuilder.AppendFormat(" {0} <br/>", errorItem.ErrorDescription);
                }

                throw new Exception(messageBuilder.ToString());
            }

            return response;
        }

        public static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }

        private static void SendEmailForData(ZeroConfirmSOIncomeJobResp response)
        {
            if (response == null)
                return;

            DateTime nowTime = DateTime.Now;
            string emailModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.EmailModel);
            string emailBody = ReadFile(emailModelPath);
            emailBody = emailBody.Replace("#DateTime#", nowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            List<ConfirmEntity> emailList = new List<ConfirmEntity>();
            response.Result.ForEach(x =>
            {
                var s = AutoConfirm.GetEmailData(Convert.ToInt32(x.SysNo));
                var confirmInfo = "确认成功";
                if (x.ErrorDescription != null)
                {
                    confirmInfo = x.ErrorDescription;
                }

                string confirmedID = Settings.UserLoginName;

                emailList.Add(new ConfirmEntity { 
                    SoSysNo = s.SoSysNo,
                    PayTerms = s.PayTerms,
                    OrderAmt = s.OrderAmt,
                    IncomeAmt = s.IncomeAmt,
                    PrepayAmt = s.PrepayAmt,
                    PointPayAmt = s.PointPayAmt,
                    GiftCardPayAmt = s.GiftCardPayAmt,
                    ConfirmedDate = s.ConfirmedDate,
                    ConfirmedInfo = confirmInfo,
                    CofirmedID = confirmedID
                });
            });

            MailInfo mail = new MailInfo();

            string fileName = "ZeroAutoConfirm" + nowTime.ToString(Settings.ShortDateFormat) + ".xls";

            string filePath = new ExcelSend().WriteToFile(emailList, fileName, nowTime);

            mail.FromName = Settings.EmailFrom;
            mail.ToName = Settings.EmailAddress;
            mail.Subject = string.Format(Settings.EmailSubject, nowTime.ToString(Settings.ShortDateFormat));
            mail.Body = emailBody;
            mail.IsHtmlType = false;

            mail.Attachments = new List<string>();
            mail.Attachments.Add(filePath);
            mail.IsAsync = false;
            mail.IsInternal = true;

            try
            {
                MailHelper.SendEmail(mail);
                MailDA.SendEmail(mail.ToName, mail.Subject, mail.Body, 1);
                OnShowInfo("邮件发送成功");
            }
            catch
            {
                MailDA.SendEmail(mail.ToName, mail.Subject, mail.Body, -1);
                OnShowInfo("邮件发送失败");
            }

        }

        private static string ReadFile(string path)
        {

            var result = "";

            StreamReader reader = new StreamReader(path, System.Text.Encoding.Default);
            try
            {
                result = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        private static string ConstructEmailBody(string emailBody, string param)
        {
            emailBody = emailBody + "<td>";
            emailBody = emailBody + param;
            emailBody = emailBody + "</td>";

            return emailBody;
        }

        private static string ConstructEmailBodyAdd(string emailBody, string param)
        {
            emailBody = emailBody + "<td align='right'>";
            emailBody = emailBody + param;
            emailBody = emailBody + "</td>";

            return emailBody;
        }
        #endregion
    }
}