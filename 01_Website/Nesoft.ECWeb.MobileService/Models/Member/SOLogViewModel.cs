using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class SOLogViewModel
    {
        public int SysNo { get; set; }
        public DateTime OptTime { get; set; }
        public int OptUserSysNo { get; set; }
        public string OptIP { get; set; }
        public int OptType { get; set; }
        public int SOSysNo { get; set; }
        public string Note { get; set; }
        public string OptTimeString
        { get; set; }
    }
}