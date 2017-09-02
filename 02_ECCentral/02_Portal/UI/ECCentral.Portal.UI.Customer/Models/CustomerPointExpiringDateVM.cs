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
    public class CustomerPointExpiringDateVM : ModelBase
    {
        public int SysNo { get; set; }

        public int PointAmount { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? Expiredate { get; set; }

        private DateTime? pointExpiringDate;

        [Validate(ValidateType.Required)]
        public DateTime? PointExpiringDate
        {
            get
            {
                return pointExpiringDate;
            }
            set
            {
                SetValue("PointExpiringDate", ref pointExpiringDate, value);
            }
        }
    }
}
