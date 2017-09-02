using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.IO;

namespace SendNoticeMailForGiftCard
{
    public class Biz : IJobAction
    {
        public string path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
        #region IJobAction Members
        public void ShowMessage(JobContext context,string message) {
            context.Message = message;
            //Console.WriteLine(context.Message);
        }
        public void Run(JobContext context)
        {
            ShowMessage(context,"JobStart"); 

            string MailSubject = GetMailTemplate("MailSubjectTemplate.txt");
            string MailBody = GetMailTemplate("MailBodyTemplate.htm");

            SendMailForExpireNote(context, MailSubject, MailBody,1);
            SendMailForExpireNote(context, MailSubject, MailBody,3);
            SendMailForExpireNote(context, MailSubject, MailBody,7);

            List<EmailList> codeEntityList = DA.GetExpiredCodeList(-1);
            if (codeEntityList == null)
            {
                codeEntityList = new List<EmailList>();
            }
            ShowMessage(context,"Get ExpiredCodeList: " + codeEntityList.Count);

            List<string> codeList = codeEntityList.Select(item => item.Code).ToList();
            string statusCode= DA.ExpiredVoid(codeList, "CustomerDomain ExpiredVoid Job",path);

            if (string.IsNullOrEmpty(statusCode))
            {
                ShowMessage(context,"ExpiredVoid Successed"); 
            }
            else
            {
                ShowMessage(context,"ExpiredVoid StatusCode:" + statusCode);
            }
            ShowMessage(context,"JobSuccessed");
            //Console.ReadLine();
        }

        private static void SendMailForExpireNote(JobContext context, 
            string MailSubject, 
            string MailBody,
            int days
            )
        {
            List<EmailList> list = DA.GetCustomerLastBuyTime(days);
            if (list != null && list.Count > 0)
            {
                context.Message = "Get ExpiredEmailList " + days + " day; " + list.Count;

                foreach (var item in list)
                {
                    string mailSubject = MailSubject.Replace("{CustomerID}", item.customerid);
                    string mailBody = MailBody.Replace("{CustomerID}", item.customerid)
                        .Replace("{Code}", item.Code)
                        .Replace("{PointExpiringDate}", item.EndDate.Value.ToString("yyyy年MM月dd日"))
                        .Replace("{SentDate}", DateTime.Now.ToString("yyyy年MM月dd日"));
                    DA.InsertEmail(item.email, mailSubject, mailBody);
                }
            }
        }

        #endregion

        private string GetMailTemplate(string txtPath)
        {
            string Template = "";
            FileStream fs = new FileStream(Path.Combine(path, txtPath), FileMode.Open);
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
    }
}