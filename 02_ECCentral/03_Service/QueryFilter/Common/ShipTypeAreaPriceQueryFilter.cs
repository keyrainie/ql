using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
   public class ShipTypeAreaPriceQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }
        public int? SysNo { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? ProvinceSysNo { get; set; }
        public int? CitySysNo { get; set; }
        public int? DistrictSysNo { get; set; }
        public int? AreaSysNo { get; set; }
        public string AreaName { get; set; }
        public int? VendorSysNo { get; set; }
        public CompanyCustomer? CompanyCustomer { get; set; }
    }
}
