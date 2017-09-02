using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;


namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class Vendor_ExVM : ModelBase
    {
        public int SysNo { get; set; }

        public int VendorSysNo { get; set; }

        public VendorStockType StockType { get; set; }

        public VendorShippingType ShippingType { get; set; }

        public VendorInvoiceType InvoiceType { get; set; }
    }
}
