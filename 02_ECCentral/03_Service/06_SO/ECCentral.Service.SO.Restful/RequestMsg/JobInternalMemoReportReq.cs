using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.SO.Restful.RequestMsg
{
    public class JobInternalMemoReportReq
    {
        public string EmailTo { get; set; }

        public string EmailCC { get; set; }

        public string CompanyCode { get; set; }
    }
}
