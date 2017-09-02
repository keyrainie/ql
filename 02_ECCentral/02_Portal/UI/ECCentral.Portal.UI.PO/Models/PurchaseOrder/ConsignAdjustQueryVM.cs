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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models.PurchaseOrder
{
    public class ConsignAdjustQueryVM:ModelBase
    {
        public ConsignAdjustQueryVM(){

        }

        private int sysNo;
        public int SysNo
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

        private int? pMSysNo;
        public int? PMSysNo 
        {

            get { return pMSysNo; }
            set { base.SetValue("PMSysNo", ref pMSysNo, value); }
        }

        public ConsignAdjustStatus? status;
        public ConsignAdjustStatus? Status 
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        public DateTime? settleRange;
        public DateTime? SettleRange {
            get { return settleRange; }
            set { base.SetValue("SettleRange", ref settleRange, value); }
        }
    }
}
