using ECommerce.DataAccess;
using ECommerce.Entity.Common;
using ECommerce.Entity.Customer;
using ECommerce.Entity.SO;
using ECommerce.Enums;
using ECommerce.Service.Customer;
using ECommerce.Service.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.Common
{
    public class SMSService
    {
        public static bool SendEmail(AsyncEmail email)
        {
            return CommonService.SendEmail(email);
        }

        public static bool SendSMS(string phoneNumber, string content)
        {
            return SendSMS(phoneNumber, content, SMSPriority.Normal);
        }

        public static bool SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            return CommonDA.SendSMS(phoneNumber, content, priority);
        }

        public static bool SendSMSForSO(int soSysNo, SMSType smsType)
        {
            SOInfo soInfo = SOService.GetSOInfo(soSysNo);
            CustomerBasicInfo customerInfo = CustomerService.GetCustomerInfo(soInfo.CustomerSysNo);
            string customerLanguageCode = customerInfo.LanguageCode;

            var sms = CommonDA.GetShipTypeSMSInfo(soInfo.ShipType.ShipTypeSysNo, smsType, null);
            var smsContent = sms.SMSContent;

            if (string.IsNullOrEmpty(smsContent))
            {
                return false;
            }
            string mobilePhone = string.Empty;
            if (!string.IsNullOrEmpty(soInfo.ReceiveCellPhone))
            {
                mobilePhone = soInfo.ReceiveCellPhone;
            }
            else
            {
                mobilePhone = soInfo.ReceivePhone;
            }

            if (smsContent.IndexOf("SO#") != -1)
            {
                smsContent = smsContent.Replace("SO#", soInfo.SOSysNo.ToString());
            }
            else
            {
                return false;
            }

            return SendSMS(mobilePhone, smsContent, SMSPriority.Normal);
        }
    }
}
