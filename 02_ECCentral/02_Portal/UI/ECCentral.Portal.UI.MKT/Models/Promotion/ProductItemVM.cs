using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductItemVM : ModelBase
    {
        private bool _IsChecked;

        /// <summary>
        /// 是否选中。
        /// </summary>
        public bool IsChecked
        {
            get { return this._IsChecked; }
            set { this.SetValue("IsChecked", ref _IsChecked, value); }
        }

        /// <summary>
        /// 商品编号。
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称。
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int? AvailableQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        public int? ConsignQty { get; set; }



        /// <summary>
        /// 成本
        /// </summary>
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        private string _PurchasingAmount;

        /// <summary>
        /// 购买数量
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,3}$", ErrorMessage = "请输入1至9999的整数！")]
        public string PurchasingAmount
        {
            get { return this._HandselQty; }
            set { this.SetValue("PurchasingAmount", ref _HandselQty, value); }
        }

        private string _Priority;

        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,5}$", ErrorMessage = "请输入1至999999的整数！")]
        public string Priority
        {
            get { return this._Priority; }
            set { this.SetValue("Priority", ref _Priority, value); }
        }

        private string _HandselQty;


        /// <summary>
        /// 赠送数量
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,3}$", ErrorMessage = "请输入1至9999的整数！")]
        public string HandselQty
        {
            get { return this._HandselQty; }
            set {
                this.SetValue("HandselQty", ref _HandselQty, value); }
        }

        public ProductItemInfo ToEntity()
        {
            return this.ConvertVM<ProductItemVM, ProductItemInfo>();
        }

    }
}
