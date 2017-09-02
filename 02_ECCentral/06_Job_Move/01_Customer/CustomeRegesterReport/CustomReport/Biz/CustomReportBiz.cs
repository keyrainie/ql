using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReportInterface;
using CustomReport.DAL;
using CustomReport.Model;
using CustomReport.Compoents;
using ECCentral.BizEntity.Common;
namespace CustomReport.Biz
{
    public class CustomReportBiz : ICustomeReport
    {
        #region ICustomeReport Members

        public void SendCustomeReport()
        {

            CustomerInfoDal dal = new CustomerInfoDal();
            List<CustomerInfo> list = dal.GetCustomerInfo();

            ExcelManage excel = new ExcelManage();
            string excelFile = excel.WriteExcel(list);


            MailInfo mailInfo = new MailInfo
            {
                FromName = Settings.EmailFrom,
                ToName = Settings.EmailAddress,
                CCName = Settings.MailCC,
                Subject = Settings.MailSubject,
                Body = Settings.MailBody,
                IsAsync = false,
                IsInternal = true,
                Attachments = new List<string>()
            };
            mailInfo.Attachments.Add(excelFile);
            bool isuccess = MailHelper.SendEmail(mailInfo);
            if (isuccess)
            {
                MailDA.SendEmail(Settings.EmailAddress, Settings.MailSubject, Settings.MailBody, 1);
            }
            else
            {
                MailDA.SendEmail(Settings.EmailAddress, Settings.MailSubject, Settings.MailBody, -1);
            }
        }

        public ShowMsg ShowInfo { get; set; }
        #endregion
    }
}
