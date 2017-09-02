using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity
{
    public class Region
    {
        public int SysNo { get; set; }

        public string RegionCode { get; set; }

        public string RegionParentCode { get; set; }

        public string RegionName { get; set; }

        public int IsLeaf { get; set; }

        public string Picture { get; set; }
    }

    public class Area
    {
        public int SysNo { get; set; }

        public int? ProvinceSysNo { get; set; }

        public string ProvinceName { get; set; }

        public int? CitySysNo { get; set; }

        public string CityName { get; set; }

        public string DistrictName { get; set; }

        public int? Status { get; set; }

        public int? RegionSysNo { get; set; }
    }
}
