using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Job.SO.SIMUnicomSO.Common;
using ECCentral.Job.Utility.Email.DataContracts;
using ECCentral.Job.Utility.Email;
using ECCentral.Job.Utility;


namespace ECCentral.Job.SO.SIMUnicomSO.Logic
{
	public class EmailBP
	{
        public static void SendMail(SOSIMCardEntity entity,SendMailCallback callback,OnSendMailExceptionHandle onException) 
        {
           if(entity==null)
           {
               return;
           }
           OnSendingMailArgs args = new OnSendingMailArgs { SIMCardEntity = entity };
           try 
           {
               MailBodyV31 mailBodyV31 = new Utility.Email.DataContracts.MailBodyV31();
               mailBodyV31.Body = CreateMailBody(entity);
               MailHelper.SendMail2IPP3Internal(mailBodyV31);       
               if(callback!=null)
               {
                   callback(args);
               }
           }
           catch(Exception  ex)
           {
               if(onException!=null)
               {
                   onException(ex, args);
               }
           }
        }

        private static MailBodyMsg CreateMailBody(SOSIMCardEntity entity)
        {
            if(entity!=null)
            {
                throw new ArgumentNullException("entity");
            }
            MailBodyMsg bodyMsg = new MailBodyMsg();
            bodyMsg.MailFrom = Config.MailFrom;
            bodyMsg.MailTo = Config.MailAddress;
            bodyMsg.Subjuect = string.Format(Config.MailSubject, entity.SOSysNo);
            bodyMsg.CCMailAddress = Config.MailCCAddress;
            bodyMsg.MailBody = string.Format("订单编号{0}已出库，请激活SIM卡，并在IPP系统中&nbsp;<a href='{1}' target='_blank'>更新</a>&nbsp;SIM卡状态。SIM卡序列号{2}",
            entity.SOSysNo, string.Format(Config.SOMaintainURL, entity.SOSysNo), entity.SIMSN);
            return bodyMsg;
        }
	}
}
