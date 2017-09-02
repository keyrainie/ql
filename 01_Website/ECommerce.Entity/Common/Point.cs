using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class Point
    {
        public int CustomerSysNo { get; set; }
        public int Points { get; set; }
        public int AvailablePoint { get; set; }
        public int ObtainType { get; set; }
        public int IsFromSysAccount { get; set; }
        public int? SysAccount { get; set; }
        public DateTime InDate { get; set; }
        public string InUser { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Memo { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCompanyCode { get; set; }
    }
}
