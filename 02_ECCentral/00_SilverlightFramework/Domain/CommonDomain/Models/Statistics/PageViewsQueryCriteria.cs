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
using System.ComponentModel.DataAnnotations;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models.Statistics
{
    public class BaseQueryCriteria : ModelBase
    {
        private DateTime? m_dateFrom;
        private DateTime? m_dateTo;

        [Validate(ValidateType.Required)]
        public DateTime? DateFrom
        {
            get
            {
                return this.m_dateFrom;
            }
            set
            {
                this.SetValue("DateFrom", ref this.m_dateFrom, value);
            }
        }

        [Validate(ValidateType.Required)]
        public DateTime? DateTo
        {
            get
            {
                return this.m_dateTo;
            }
            set
            {
                this.SetValue("DateTo", ref this.m_dateTo, value);
            }
        }

        public string Url { get; set; }
    }
}
