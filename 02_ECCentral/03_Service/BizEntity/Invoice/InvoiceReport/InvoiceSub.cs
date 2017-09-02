using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.InvoiceReport
{
    public class InvoiceSub
    {
        public int InvoiceSeq { get; set; }

     
        public int ProductSysNo { get; set; }

     
        public string ProductID { get; set; }

     
        public int SplitQty { get; set; }

     
        public bool IsExtendWarrantyItem { get; set; }

     
        public string MasterProductSysNo { get; set; }
    }
}
