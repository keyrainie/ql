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

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class InventoryTransferStockingVendorInfoVM : ModelBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { this.SetValue("IsChecked", ref isChecked, value); }
        }

        private string vendorSysNo;

        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { this.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        private string vendorID;

        public string VendorID
        {
            get { return vendorID; }
            set { this.SetValue("VendorID", ref vendorID, value); }
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }

        private string isAlreadyPlaceOrderOfTaday;

        public string IsAlreadyPlaceOrderOfTaday
        {
            get { return isAlreadyPlaceOrderOfTaday; }
            set { this.SetValue("IsAlreadyPlaceOrderOfTaday", ref isAlreadyPlaceOrderOfTaday, value); }
        }
    }
}
