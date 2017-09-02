using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class PointFilter
    {
        public int CustomerSysNo { get; set; }
        public int ObtainType { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCompanyCode { get; set; }
    }
}
