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
    public class VendorManufacturerQueryVM : ModelBase
    {
        public VendorManufacturerQueryVM()
        {

        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { sysNo = value; }
        }

        private string manufacturerID;

        public string ManufacturerID
        {
            get { return manufacturerID; }
            set { manufacturerID = value; }
        }

        private string manufacturerName;

        public string ManufacturerName
        {
            get { return manufacturerName; }
            set { manufacturerName = value; }
        }

        private string briefName;

        public string BriefName
        {
            get { return briefName; }
            set { briefName = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { companyCode = value; }
        }
    }
}
