using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorStagedSaleRuleEntityVM : ModelBase
    {
        /// <summary>
        /// 阶梯销售设置
        /// </summary>
        public VendorStagedSaleRuleEntityVM()
        {
            stagedSaleRuleItems = new List<VendorStagedSaleRuleInfoVM>();
        }
        private List<VendorStagedSaleRuleInfoVM> stagedSaleRuleItems;

        public List<VendorStagedSaleRuleInfoVM> StagedSaleRuleItems
        {
            get { return stagedSaleRuleItems; }
            set { base.SetValue("StagedSaleRuleItems", ref stagedSaleRuleItems, value); }
        }

        private decimal? minCommissionAmt;

        public decimal? MinCommissionAmt
        {
            get { return minCommissionAmt; }
            set { base.SetValue("MinCommissionAmt", ref minCommissionAmt, value); }
        }
    }

}
