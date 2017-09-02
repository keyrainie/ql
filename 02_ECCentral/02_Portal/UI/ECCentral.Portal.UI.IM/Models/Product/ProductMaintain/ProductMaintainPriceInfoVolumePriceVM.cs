using System;
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoVolumePriceVM : ModelBase
    {
        public List<ProductVolumePriceVM> ProductVolumePriceList { get; set; }
    }

    public class ProductVolumePriceVM : ModelBase
    {

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue("IsChecked", ref _isChecked, value); }
        }

        private WholeSaleLevelType _level;

        public WholeSaleLevelType Level
        {
            get { return _level; }
            set { SetValue("Level", ref _level, value); }
        }

        private int? _qty;

        public int? Qty
        {
            get { return _qty; }
            set { SetValue("Qty", ref _qty, value); }
        }

        private String _volumePriceRequestQty;

        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$", ErrorMessageResourceName = "ProductMaintain_ProductPriceVolumePriceQtyInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String VolumePriceRequestQty
        {
            get { return _volumePriceRequestQty; }
            set { SetValue("VolumePriceRequestQty", ref _volumePriceRequestQty, value); }
        }

        private decimal? _price;

        public decimal? Price
        {
            get { return _price; }
            set { SetValue("Price", ref _price, value); }
        }

        private String _volumePriceRequestPrice;

        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceVolumePriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String VolumePriceRequestPrice
        {
            get { return _volumePriceRequestPrice; }
            set { SetValue("VolumePriceRequestPrice", ref _volumePriceRequestPrice, value); }
        }

    }
}