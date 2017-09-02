using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    public class SOTrackingInfo
    {
        public int? SONumber { get; set; }
        public string WarehouseNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CreateUserID { get; set; }

        public int? DropshipID { get; set; }
        public int? SubCode { get; set; }
        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public string StoreCompanyCode { get; set; }
    }
}
