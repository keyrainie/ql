using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.BizEntity.Invoice.SAP
{
    public class SAPVendorInfo : IIdentity, ICompany
    {
        public int? SysNo
        {
            get;
            set;
        }
        public string CompanyCode
        {
            get;
            set;
        }
        public int? VendorSysNo { get; set; }
        public string SAPVendorID { get; set; }
        public string SAPVendorName { get; set; }
        public int? PaymentTerm { get; set; }
        public SAPStatus? Status { get; set; }
    }
}
