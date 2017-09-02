using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Common
{
    public class AreaViewModel
    {
        public int? SysNo { get; set; }

        public int? ProvinceSysNo { get; set; }

        public string ProvinceName { get; set; }

        public int? CitySysNo { get; set; }

        public string CityName { get; set; }

        public string DistrictName { get; set; }
    }
}