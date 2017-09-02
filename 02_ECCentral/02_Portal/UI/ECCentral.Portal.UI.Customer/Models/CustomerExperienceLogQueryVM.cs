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

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerExperienceLogQueryVM:ModelBase
    {
        public CustomerExperienceLogQueryVM()
        {
            
        }
        private DateTime? createTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get
            {
                return createTimeFrom;
            }
            set
            {
                base.SetValue("CreateTimeFrom", ref createTimeFrom, value);
            }
        }

        private DateTime? createTimeTo;
        public DateTime? CreateTimeTo
        {
            get
            {
                return createTimeTo;
            }
            set
            {
                base.SetValue("CreateTimeTo", ref createTimeTo, value);
            }
        }

        private string  customerSysNo;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }
    }
}
