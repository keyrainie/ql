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
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class InventoryItemCardQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue("StockSysNo", ref stockSysNo, value); }
        }

        private int? productSysNo;
        [Validate(ValidateType.Required)]
        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
        }

        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        /// <summary>
        /// 自己权限内能访问到的PM
        /// </summary>
        public string AuthorizedPMsSysNumber { get; set; }

        public string UserName { get; set; }
    }

    public class InventoryItemCardQueryView : ModelBase
    {
        public InventoryItemCardQueryVM QueryInfo
        {
            get;
            set;
        }   

        private List<dynamic> itemCardResult;
        public List<dynamic> ItemCardResult
        {
            get { return itemCardResult; }
            set
            {
                SetValue("ItemCardResult", ref itemCardResult, value);
            }
        }

        private List<dynamic> inventoryResult;
        public List<dynamic> InventoryResult
        {
            get { return inventoryResult; }
            set
            {
                SetValue("InventoryResult", ref inventoryResult, value);
            }
        }

        private int itemCardTotalCount;
        public int ItemCardTotalCount
        {
            get { return itemCardTotalCount; }
            set
            {
                SetValue<int>("ItemCardTotalCount", ref itemCardTotalCount, value);
            }
        }

        private int inventoryTotalCount;
        public int InventoryTotalCount
        {
            get { return inventoryTotalCount; }
            set
            {
                SetValue<int>("InventoryTotalCount", ref inventoryTotalCount, value);
            }
        }

        public InventoryItemCardQueryView()
        {
            QueryInfo = new InventoryItemCardQueryVM();            
        }
    }
}
