using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductSaleInfoInterface;
using ProductSaleInfoBiz.Model;
using ProductSaleInfoBiz.DAL;
using ProductSaleInfoBiz.Compoents;
using System.IO;
using ECCentral.BizEntity.Common;
namespace ProductSaleInfoBiz.Biz
{
    public class ProductSaleInfoBiz : IProductSaleInfo
    {
        #region IProductSaleInfo Members

        public void SendProductSaleInfoReport()
        {
            List<ProductSaleInfo> list = ProductSaleInoDal.GetProductSaleInoInfo();

            SendMail(list);
        }

        public ShowMsg ShowInfo{get;set;}

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="dataTable"></param>
        private void SendMail(List<ProductSaleInfo> entities)
        {
            string dateString = DateTime.Now.Date.Year.ToString() + "-" + DateTime.Now.Date.Month.ToString() + "-" + DateTime.Now.Date.Day.ToString();
            MailInfo mail = new MailInfo();
            ExcelManage excelm = new ExcelManage();

            string filePath = excelm.WriteToFile(entities);

            mail.FromName = Settings.EmailFrom;
            mail.ToName = Settings.EmailAddress;
            mail.Subject = Settings.MailSubject + DateTime.Now.Date.ToShortDateString();
            mail.Body = Settings.MailBody;
            mail.IsHtmlType = false;

            mail.Attachments = new List<string>();
            mail.Attachments.Add(filePath);
            mail.IsAsync = false;
            mail.IsInternal = true;

            try
            {
                MailHelper.SendEmail(mail);
                MailDA.SendEmail(mail.ToName, mail.Subject, mail.Body, 1);
            }
            catch
            {
                MailDA.SendEmail(mail.ToName, mail.Subject, mail.Body, -1);
            }

        }

        #endregion
    }
}
