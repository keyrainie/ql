using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class ImportPostIncomeReq
    {
        public string FileIdentity { get; set; }
        public string CompanyCode { get; set; }
    }
}
