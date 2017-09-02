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
    public class VendorHoldPMInfoVM : ModelBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
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
        private string pMID;

        public string PMID
        {
            get { return pMID; }
            set { base.SetValue("PMID", ref pMID, value); }
        }
        private string pMName;

        public string PMName
        {
            get { return pMName; }
            set { base.SetValue("PMName", ref pMName, value); }
        }


        private string inUser;

        public string InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }
    }
}
