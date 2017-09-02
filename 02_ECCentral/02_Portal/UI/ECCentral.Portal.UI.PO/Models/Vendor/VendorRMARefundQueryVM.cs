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
    public class VendorRMARefundQueryVM : ModelBase
    {

        private DateTime? createDateFrom;

        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set { this.SetValue("CreateDateFrom", ref createDateFrom, value); }
        }

        private DateTime? createDateTo;

        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set { this.SetValue("CreateDateTo", ref createDateTo, value); }
        }

        private string vendorRefundSysNo;

        public string VendorRefundSysNo
        {
            get { return vendorRefundSysNo; }
            set { this.SetValue("VendorRefundSysNo", ref vendorRefundSysNo, value); }
        }

        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { this.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        private string rMARegisterSysNo;

        public string RMARegisterSysNo
        {
            get { return rMARegisterSysNo; }
            set { this.SetValue("RMARegisterSysNo", ref rMARegisterSysNo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { this.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private VendorRefundStatus? status;

        public VendorRefundStatus? Status
        {
            get { return status; }
            set { this.SetValue("Status", ref status, value); }
        }

        private VendorRefundPayType? payType;

        public VendorRefundPayType? PayType
        {
            get { return payType; }
            set { this.SetValue("PayType", ref payType, value); }
        }

        private int? pMSysNo;

        public int? PMSysNo
        {
            get { return pMSysNo; }
            set { this.SetValue("PMSysNo", ref pMSysNo, value); }
        }
    }
}
