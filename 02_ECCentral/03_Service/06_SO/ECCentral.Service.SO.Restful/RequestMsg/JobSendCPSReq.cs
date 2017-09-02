using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.SO.Restful.RequestMsg
{
    public class JobCPSSendReq
    {
        public string TargetUrl { get; set; }

        public string SPCode { get; set; }

        public decimal Fanli { get; set; }

        public string CompanyCode { get; set; }
    }
}
