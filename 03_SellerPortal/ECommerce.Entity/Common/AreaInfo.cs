using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class AreaInfo
    {
        public int? SysNo { get; set; }

        public int? ProvinceSysNo { get; set; }

        public string ProvinceName { get; set; }

        public int? CitySysNo { get; set; }

        public string CityName { get; set; }

        public string DistrictName { get; set; }

        public int? Status { get; set; }

        public int? RegionSysNo { get; set; }

        public string StockName { get; set; }
    }
}
