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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CostChangeItemInfoVM : ModelBase
    {
        private int? itemSysNo;
        /// <summary>
        /// Item编号
        /// </summary>
        public int? ItemSysNo
        {
            get { return itemSysNo; }
            set { this.SetValue("ItemSysNo", ref itemSysNo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { this.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { this.SetValue("ProductID", ref productID, value); }
        }

        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { this.SetValue("ProductName", ref productName, value); }
        }

        private int? poSysNo;

        public int? POSysNo
        {
            get { return poSysNo; }
            set { this.SetValue("POSysNo", ref poSysNo, value); }
        }

        private decimal? oldPrice;

        public decimal? OldPrice
        {
            get { return oldPrice; }
            set { this.SetValue("OldPrice", ref oldPrice, value); }
        }

        private string newPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string NewPrice
        {
            get { return newPrice; }
            set { this.SetValue("NewPrice", ref newPrice, value); }
        }

        private int? avaliableQty;

        public int? AvaliableQty
        {
            get { return avaliableQty; }
            set { this.SetValue("AvaliableQty", ref avaliableQty, value); }
        }

        private string changeCount;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ChangeCount
        {
            get { return changeCount; }
            set { this.SetValue("ChangeCount", ref changeCount, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }

        private bool isCheckedItem;

        public bool IsCheckedItem
        {
            get { return isCheckedItem; }
            set { this.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }

        private ItemActionStatus itemActionStatus;
        /// <summary>
        /// 明细状态
        /// </summary>
        public ItemActionStatus ItemActionStatus
        {
            get
            {
                if (itemActionStatus == null)
                {
                    return ItemActionStatus.Update;
                }
                else
                {
                    return itemActionStatus;
                }
            }
            set
            {
                itemActionStatus = value; 
            }
        }
    }
}
