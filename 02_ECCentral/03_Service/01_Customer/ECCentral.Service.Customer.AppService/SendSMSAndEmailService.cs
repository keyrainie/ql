using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.BizProcessor;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(SendSMSAndEmailService))]
    public class SendSMSAndEmailService
    {
        public virtual List<string> SendByCellphone(List<string> cellphoneList, string message)
        {
            List<string> resultList = new List<string>();
            foreach (var item in cellphoneList)
            {
                string errorMsg = string.Empty;
                ObjectFactory<SMSProcessor>.Instance.SendByCellphone(item, message, out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    resultList.Add(errorMsg);
                }
            }
            return resultList;

        }
        public virtual List<string> SendBySOSysNo(List<string> soSysNoList, string message)
        {
            List<string> resultList = new List<string>();
            foreach (var item in soSysNoList)
            {
                string errorMsg = string.Empty;
                int soSysNo = 0;
                if (int.TryParse(item, out soSysNo))
                {
                    ObjectFactory<SMSProcessor>.Instance.SendBySOSysNo(soSysNo, message, out errorMsg);
                }
                else
                {
                    errorMsg = item + "不是有效的数字";
                }
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    resultList.Add(errorMsg);
                }
            }
            return resultList;
        }

        public virtual List<string> SendEmail(List<string> emailList, string title, string content,string companyCode)
        {
            List<string> resultList = new List<string>();
            foreach (var item in emailList)
            {
                if (!ObjectFactory<SMSProcessor>.Instance.SendEmail(item, title, content,companyCode))
                {
                    resultList.Add(item);
                }
            }
            return resultList;
        }
    }
}
