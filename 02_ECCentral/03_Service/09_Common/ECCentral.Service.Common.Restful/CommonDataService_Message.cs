using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/Message/SendMail", Method = "POST")]
        public void SendMail(ECCentral.Service.Utility.MailMessage mailMessage)
        {
            ObjectFactory<IEmailSend>.Instance.SendMail(mailMessage, true, false);
        }

        [WebInvoke(UriTemplate = "/Message/SendSMS", Method = "POST")]
        public void SendSMS(SMSInfo smsInfo)
        {
            ObjectFactory<CommonDataAppService>.Instance.SendSMS(smsInfo.CellPhoneNum, smsInfo.Content, (SMSPriority)smsInfo.Priority);
        }
    }
}
