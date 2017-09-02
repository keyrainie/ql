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
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.Basic.Components.UserControls.VendorPicker
{
    public class VendorQueryVM : ModelBase
    {
        private string vendorSysNo;

        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }

        private VendorStatus? status;

        public VendorStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
    }
}
