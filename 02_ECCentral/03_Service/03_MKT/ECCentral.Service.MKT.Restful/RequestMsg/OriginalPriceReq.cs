using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    public class OriginalPriceReq
    {
        public int? ProductSysNo { get; set; }
        public string IsByGroup { get; set; }
        public string CompanyCode { get; set; }
    }
}
