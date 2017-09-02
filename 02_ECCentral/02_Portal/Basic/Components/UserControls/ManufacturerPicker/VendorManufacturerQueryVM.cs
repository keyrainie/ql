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

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public class VendorManufacturerQueryVM : ModelBase
    {
        public VendorManufacturerQueryVM()
        {

        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string manufacturerID;

        public string ManufacturerID
        {
            get { return manufacturerID; }
            set { base.SetValue("ManufacturerID", ref manufacturerID, value); }
        }

        private string manufacturerName;

        public string ManufacturerName
        {
            get { return manufacturerName; }
            set { base.SetValue("ManufacturerName", ref manufacturerName, value); }
        }

        private string briefName;

        public string BriefName
        {
            get { return briefName; }
            set { base.SetValue("BriefName", ref briefName, value); }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
    }
}
