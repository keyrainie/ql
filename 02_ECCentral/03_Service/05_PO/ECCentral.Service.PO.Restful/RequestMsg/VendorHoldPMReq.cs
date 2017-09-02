using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.Restful.RequestMsg
{
    public class VendorHoldPMReq
    {
        public int VendorSysNo { get; set; }
        public int HoldUserSysNo { get; set; }
        public string Reason { get; set; }
        public List<int> HoldSysNoList { get; set; }
        public List<int> UnHoldSysNoList { get; set; }
        public string CompanyCode { get; set; }
    }
}
