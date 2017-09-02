using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.InvoiceReport
{
    public class SOInvoiceInfo
    {
        public SOInfo SOInfo { get; set; }

        public List<InvoiceInfo> InvoiceInfoList { get; set; }
    }
}
