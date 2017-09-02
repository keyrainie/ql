using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorStagedSaleRuleInfoVM : ModelBase
    {

        private int? order;

        public int? Order
        {
            get { return order; }
            set { base.SetValue("Order", ref order, value); }
        }
        private decimal? startAmt;

        public decimal? StartAmt
        {
            get { return startAmt; }
            set { base.SetValue("StartAmt", ref startAmt, value); }
        }
        private decimal? endAmt;

        public decimal? EndAmt
        {
            get { return endAmt; }
            set { base.SetValue("EndAmt", ref endAmt, value); }
        }
        private decimal? percentage;

        public decimal? Percentage
        {
            get { return percentage; }
            set { base.SetValue("Percentage", ref percentage, value); }
        }
    }
}
