using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductSalesAreaBatchProvinceVM : ModelBase
    {
        public int SysNo { get; set; }
        public string ProvinceName { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }
    }
}
