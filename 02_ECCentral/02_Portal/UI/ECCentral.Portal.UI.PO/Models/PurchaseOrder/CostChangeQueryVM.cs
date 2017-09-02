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

namespace ECCentral.Portal.UI.PO.Models
{
    public class CostChangeQueryVM : ModelBase
    {
        public CostChangeQueryVM()
        {
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }
        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        private int? pMSysNo;

        public int? PMSysNo
        {
            get { return pMSysNo; }
            set { base.SetValue("PMSysNo", ref pMSysNo, value); }
        }

        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

        private CostChangeStatus? status;
        public CostChangeStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
    }
}
