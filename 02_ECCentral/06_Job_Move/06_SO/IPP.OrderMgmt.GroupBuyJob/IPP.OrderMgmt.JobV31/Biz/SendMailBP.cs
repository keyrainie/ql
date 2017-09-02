using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using System.Threading;
using IPP.OrderMgmt.JobV31.ServiceAdapter;
using Newegg.Oversea.Framework.ExceptionHandler;
using IPP.OrderMgmt.JobV31.Dac;
using System.Configuration;
using IPP.OrderMgmt.JobV31.Resources;

namespace IPP.OrderMgmt.JobV31.Biz
{
    public class SendMailBP
    {

        private static string CompanyCode
        {
            get { return ConfigurationManager.AppSettings["CompanyCode"]; }
        }

        public static void SendFailedMail4SO(SOEntity soEntity, ProductGroupBuyingEntity group)
        { 
            CustomerInfo customerInfo=CommonDA.GetCustomerBySysNo(soEntity.SOMaster.CustomerSysNo,CompanyCode);

            if (customerInfo == null || customerInfo.Email == null)
            {
                return;
            }

            group.LowerLimitSellCount = CommonDA.GetLowerLimitSellCount(group.SystemNumber);
            string mailFrom = "IPPSystem";
            string mailTo = Util.TrimNull(customerInfo.Email);

            string mailSubject = "您在新蛋的订单SO#"+ soEntity.SOMaster.SystemNumber +"团购失败，订单取消";
            string mailBody = BuildFailedMail(customerInfo, soEntity,group);


            
            CommonServiceAdapter.SendEmail2MailDb(mailFrom, mailTo, null, null, mailSubject, mailBody, CompanyCode);

        }

        private static string BuildFailedMail(CustomerInfo customerInfo, SOEntity soEntity,ProductGroupBuyingEntity group)
        {
            string emailTemplate = MailTemplates.GroupBuyFailed;

            string linkForTitle = string.Format("<a href='http://tuan.newegg.com.cn/deal/{0}.htm'>{1}</a>", group.SystemNumber, group.GroupBuyingTitle);
            emailTemplate = emailTemplate.Replace("${CustomerID}", customerInfo.CustomerID)
                .Replace("${SOSysNo}", soEntity.SOMaster.SystemNumber.ToString())
                .Replace("${OrderDateTime}", soEntity.SOMaster.OrderDate.ToString("yyyy年MM月dd日 HH:mm"))
                .Replace("${GroupTitle}", linkForTitle)
                .Replace("${GroupStart}", group.BeginDate.ToString("yyyy年MM月dd日 HH:mm"))
                .Replace("${GroupEnd}", group.EndDate.ToString("yyyy年MM月dd日 HH:mm"))
                .Replace("${GroupLimited}",group.LowerLimitSellCount.ToString())
                .Replace("${GroupCurrentCount}",group.CurrentSellCount.ToString())
                .Replace("${SendDateTime}", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("${CopyRight}", DateTime.Now.Year.ToString());

            return emailTemplate;
        }


    }
}
