using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductSalesAreaBatchStockVM:ModelBase
    {

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public string StockID
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库名称
        /// </summary>
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库状态
        /// </summary>
        public ValidStatus StockStatus
        {
            get;
            set;
        }
        private bool isChecked;
        public bool IsChecked {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }
        public int? SysNo { get; set; }
    }
}
