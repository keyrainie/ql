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
    public class VendorCommissionQueryVM : ModelBase
    {
        private string sysNo;

        public string SysNo
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

        private DateTime? inDateBegin;

        public DateTime? InDateBegin
        {
            get { return inDateBegin; }
            set { base.SetValue("InDateBegin", ref inDateBegin, value); }
        }

        private DateTime? inDateEnd;

        public DateTime? InDateEnd
        {
            get { return inDateEnd; }
            set { base.SetValue("InDateEnd", ref inDateEnd, value); }
        }

        private DateTime? outListDateBegin;

        public DateTime? OutListDateBegin
        {
            get { return outListDateBegin; }
            set { base.SetValue("OutListDateBegin", ref outListDateBegin, value); }
        }

        private DateTime? outListDateEnd;

        public DateTime? OutListDateEnd
        {
            get { return outListDateEnd; }
            set { base.SetValue("OutListDateEnd", ref outListDateEnd, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
    }
}
