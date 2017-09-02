using Newegg.Oversea.Framework.EmailService.ServiceInterfaces;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace SendMailARAmtOfVIPCustomer.Biz.ServiceAdapter
{
    public class CommonServiceAdapter
    {
        #region Message Contains Mail
        /// <summary>
        /// 发送带附件的邮件
        /// </summary>
        /// <returns></returns>
        public static void SendMailWithAccessories(MailContract mailContract)
        {
            ISendEmail service = ServiceBroker.FindService<ISendEmail>();
            try
            {
                bool isSuccess = service.SendMail(mailContract, false);
                mailContract.Attachments.Clear();           //清空附件列表

                if (!isSuccess)
                {
                    throw (new BusinessException("Invoice job(SendMailARAmtOfVIPCustomer) Send mail failed!"));
                }
            }
            finally 
            {
                ServiceBroker.DisposeService<ISendEmail>(service);
            }
        
        }
 
        #endregion
    }
}
