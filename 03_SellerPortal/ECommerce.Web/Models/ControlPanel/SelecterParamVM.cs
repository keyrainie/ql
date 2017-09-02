using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Web.Models.ControlPanel
{
    public class SelecterParamVM
    {
        public SelecterParamVM()
        {
            this.ShowAll = true;
        }
        public int SysNo { get; set; }
        public bool ShowAll { get; set; }
        public bool HideCity { get; set; }
        public bool HideDistrict { get; set; }
        public string Tag { get; set; }
    }
}