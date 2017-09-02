using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Common
{
    public class SMSInfo
    {
        public int? SysNo { get; set; }
        public string CellNumber { get; set; }
        public string SMSContent { get; set; }
        public int? Priority { get; set; }
        public int? RetryCount { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? HandleTime { get; set; }
        public SMSStatus? Status { get; set; }
        public int? CreateUserSysNo { get; set; }
        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public string StoreCompanyCode { get; set; }
        public SMSType? Type { get; set; }

        public string IPAddress { get; set; }
        //public int? SOSysNo { get; set; }
        //public int? CustomerSysNo { get; set; }
    }
}
