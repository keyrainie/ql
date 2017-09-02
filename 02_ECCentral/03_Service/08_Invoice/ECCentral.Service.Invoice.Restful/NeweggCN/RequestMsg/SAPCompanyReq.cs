using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.SAP;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class SAPCompanyReq
    {
        public int AlertFlag { get; set; }

        public SAPCompanyInfo SAPCompany { get; set; }
    }
}
