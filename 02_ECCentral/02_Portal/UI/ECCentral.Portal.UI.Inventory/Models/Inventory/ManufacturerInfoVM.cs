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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ManufacturerInfoVM : ModelBase
    {
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

        private string manufacturerNameDisplay;
        [Validate(ValidateType.Required)]
        public string ManufacturerNameDisplay
        {
            get { return manufacturerNameDisplay; }
            set { base.SetValue("ManufacturerNameDisplay", ref manufacturerNameDisplay, value); }
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

    }
}
