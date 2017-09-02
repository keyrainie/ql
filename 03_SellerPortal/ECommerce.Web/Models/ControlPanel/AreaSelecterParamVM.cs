using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Web.Models.ControlPanel
{
    public class AreaSelecterParamVM : SelecterParamVM
    {
        public int ProvinceSysNo { get; set; }
        public int CitySysNo { get; set; }
        public bool HasCountry { set; get; }
        public bool HasRegion { set; get; }
    }
}