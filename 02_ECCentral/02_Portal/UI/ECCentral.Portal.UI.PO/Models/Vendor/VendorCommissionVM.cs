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

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorCommissionVM : ModelBase
    {
        private bool isCheckedItem;

        public bool IsCheckedItem
        {
            get { return isCheckedItem; }
            set { base.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
        private int? vendorSysNo;

        public int? VendorSysNo
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

        private decimal? totalAmt;

        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { base.SetValue("TotalAmt", ref totalAmt, value); }
        }
        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }
        private DateTime? endDate;

        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }
        private string status;

        public string Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        private string settleStatus;

        public string SettleStatus
        {
            get { return settleStatus; }
            set { base.SetValue("SettleStatus", ref settleStatus, value); }
        }
    }
}
