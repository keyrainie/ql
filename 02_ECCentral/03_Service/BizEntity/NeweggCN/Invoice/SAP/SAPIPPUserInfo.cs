using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.SAP
{
    public class SAPIPPUserInfo : IIdentity, ICompany
    {
        public int? SysNo
        {
            get;
            set;
        }
        public int? PayTypeSysNo { get; set; }

        public string CustDescription { get; set; }

        public string CustID { get; set; }

        public string SystemConfirmID { get; set; }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}
