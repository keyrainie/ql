using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class EmailInfo
    {
        public EmailInfo()
        {
            Init();
        }

        public int SysNo;
        public string MailAddress;
        public string MailSubject;
        public string MailBody;
        public int Status;
        public string CCMailAddress;
        public string BCMailAddress;

        public string MailFrom;
        public string MailSenderName;

        public void Init()
        {
            SysNo = AppConst.IntNull;
            MailAddress = AppConst.StringNull;
            MailSubject = AppConst.StringNull;
            MailBody = AppConst.StringNull;
            Status = AppConst.IntNull;
            CCMailAddress = AppConst.StringNull;
            BCMailAddress = AppConst.StringNull;

            MailFrom = AppConst.StringNull;
            MailSenderName = AppConst.StringNull;
        }
    }
}
