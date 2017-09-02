using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class AreaInfoQueryFilter : QueryFilter
    {
        public int? DistrictSysNo { get; set; }
        public int? CitySysNo { get; set; }
        public string ProvinceSysNo { get; set; }
        public int? Status { get; set; }
        public bool OnlyProvince { get; set; }
        public bool OnlyCity { get; set; }
        public bool OnlyDistrict { get; set; }
    }
}
