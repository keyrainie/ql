using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(SMSProcessor))]
    public class SMSProcessor
    {

        public virtual void SendByCellphone(string cellphone, string message, out string errorMsg)
        {
            try
            {
                ExternalDomainBroker.SendSMS(cellphone, message, BizEntity.Common.SMSPriority.Normal);
                errorMsg = string.Empty;
            }
            catch
            {
                errorMsg = cellphone + "出现异常";
            }
        }

        public virtual void SendBySOSysNo(int soSysNo, string message, out string errorMsg)
        {
            try
            {
                string cellphone = ExternalDomainBroker.GetSOReceiverPhone(soSysNo);
                if (!string.IsNullOrEmpty(cellphone))
                {
                    ExternalDomainBroker.SendSMS(cellphone, message, BizEntity.Common.SMSPriority.Normal);
                }
                else
                {
                    errorMsg = soSysNo + "不是有效的订单编号";
                    return;
                }
                errorMsg = string.Empty;
            }
            catch
            {
                errorMsg = soSysNo + "出现异常";
            }
        }

        public virtual bool SendEmail(string toEmail, string title, string content, string companyCode)
        {
            try
            {
                var param = new KeyValueVariables();
                param.Add("Content", content);
                param.Add("Title", title);
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(toEmail, "Manual_Send", param, false);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
