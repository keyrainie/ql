using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Common
{
    public class AsyncEmail
    {
        public int? SysNo { get; set; }

        public string MailAddress { get; set; }

        public string CCMailAddress { get; set; }

        public string BCMailAddress { get; set; }

        public string MailSubject { get; set; }

        public string MailBody { get; set; }

        public string MailFrom { get; set; }

        public string MailSenderName { get; set; }

        public int? Status { get; set; }

        //public int? CustomerSysNo { get; set; }

        //public EmailType MailType { get; set; }

        public int? Priority { get; set; }

        public string Department { get; set; }

        public DateTime? SendTime { get; set; }

        public DateTime? CreateTime { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }

        public string StoreCompanyCode { get; set; }

        #region 邮件参数
        public string CustomerID { get; set; }

        public string SetNewTokenUrl { get; set; }

        public string CurrentDateTime { get; set; }

        public string Year { get; set; }

        public string ImgBaseUrl { get; set; }
        #endregion
    }
}
