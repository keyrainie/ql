using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class Vendor_ExInfo
    {
        public int SysNo { get; set; }

        public int VendorSysNo { get; set; }

        public VendorStockType StockType { get; set; }

        public VendorShippingType ShippingType { get; set; }

        public VendorInvoiceType InvoiceType { get; set; }
    }
}
