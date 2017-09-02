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

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterMemoVM : ModelBase
    {
        public int? RegisterSysNo { get; set; }

        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string VendorName { get; set; }

        private string content;
        [Validate(ValidateType.Required)]
        public string Content
        {
            get { return content; }
            set { base.SetValue("Content", ref content, value); }
        }

        public string Memo { get; set; }
    }
}
