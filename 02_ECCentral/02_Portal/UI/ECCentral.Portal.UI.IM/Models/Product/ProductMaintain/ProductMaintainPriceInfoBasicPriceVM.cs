using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoBasicPriceVM : ModelBase
    {
        public ProductMaintainPriceInfoBasicPriceVM()
        {
            ProductPayTypeList = EnumConverter.GetKeyValuePairs<ProductPayType>(EnumConverter.EnumAppendItemType.None);
            ProductPayTypeList.RemoveAll(p => p.Key == ProductPayType.PointOnly);
            this.PayType = ProductPayType.All;
        }

        public List<KeyValuePair<ProductPayType?, string>> ProductPayTypeList { get; set; }

        private String _basicPrice;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductPriceBasicPriceEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceBasicPriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String BasicPrice
        {
            get { return _basicPrice; }
            set { SetValue("BasicPrice", ref _basicPrice, value); }
        }

        private decimal? _currentPrice;

        public decimal? CurrentPrice
        {
            get { return _currentPrice; }
            set { SetValue("CurrentPrice", ref _currentPrice, value); }
        }

        private decimal? _cashRebate;

        public decimal? CashRebate
        {
            get { return _cashRebate; }
            set { SetValue("CashRebate", ref _cashRebate, value); }
        }

        private int? _point;

        public int? Point
        {
            get { return _point; }
            set { SetValue("Point", ref _point, value); }
        }

        private ProductPayType _payType;

        public ProductPayType PayType
        {
            get { return _payType; }
            set { SetValue("PayType", ref _payType, value); }
        }

        private String _maxCountPerDay;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductPriceMaxCountPerDayEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$", ErrorMessageResourceName = "ProductMaintain_ProductPriceMaxCountPerDayInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String MaxCountPerDay
        {
            get { return _maxCountPerDay; }
            set { SetValue("MaxCountPerDay", ref _maxCountPerDay, value); }
        }

        private decimal _unitCost;

        public decimal UnitCost
        {
            get { return _unitCost; }
            set { SetValue("UnitCost", ref _unitCost, value); }
        }

        private decimal _unitCostWithoutTax;

        public decimal UnitCostWithoutTax
        {
            get { return _unitCostWithoutTax; }
            set { SetValue("UnitCostWithoutTax", ref _unitCostWithoutTax, value); }
        }

        private decimal _discountAmount;

        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set { SetValue("DiscountAmount", ref _discountAmount, value); }
        }

        private String _virtualPrice;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductPriceVirtualPriceEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceVirtualPriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String VirtualPrice
        {
            get { return _virtualPrice; }
            set { SetValue("VirtualPrice", ref _virtualPrice, value); }
        }

        private String _minCountPerOrder;
         [Validate(ValidateType.Required, ErrorMessageResourceType=typeof(ResProductMaintain),ErrorMessageResourceName="Error_MinCountPerOrderMessage")]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$", ErrorMessageResourceName = "ProductMaintain_ProductPriceMinCountPerOrderInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String MinCountPerOrder
        {
            get { return _minCountPerOrder; }
            set { SetValue("MinCountPerOrder", ref _minCountPerOrder, value); }
        }

        private String _requestCurrentPrice;

        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceCurrentPriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String RequestCurrentPrice
        {
            get { return _requestCurrentPrice; }
            set { SetValue("RequestCurrentPrice", ref _requestCurrentPrice, value); }
        }

        private String _minCommission;
        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceMinCommissionInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String MinCommission
        {
            get { return _minCommission; }
            set { SetValue("MinCommission", ref _minCommission, value); }
        }

        private String _requestCashRebate;

        [Validate(ValidateType.Regex, @"^\d+$", ErrorMessageResourceName = "ProductMaintain_ProductPriceCashRebateInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String RequestCashRebate
        {
            get { return _requestCashRebate; }
            set { SetValue("RequestCashRebate", ref _requestCashRebate, value); }
        }

        private String _requestPoint;

        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductPricePointInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String RequestPoint
        {
            get { return _requestPoint; }
            set { SetValue("RequestPoint", ref _requestPoint, value); }
        }

        private bool _isSyncShopPrice;

        public bool IsSyncShopPrice
        {
            get { return _isSyncShopPrice; }
            set { SetValue("IsSyncShopPrice", ref _isSyncShopPrice, value); }
        }

        public ProductVM MainPageVM
        {
            get { return ((Views.ProductMaintain)CPApplication.Current.CurrentPage).VM; }
        }

        public decimal? JDPrice { get; set; }

        public decimal? AMPrice { get; set; }

        public bool HasItemPriceRemarkMaintainOnlyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPriceRemarkMaintainOnly); }
        }

        public bool HasItemPriceUpdateVPMaintainPermission
        {
            //get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPriceUpdateVPMaintain); }
            get { return false; }
        }

        public string HasItemDisplaycolumnPermissionVisible
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemDisplaycolumn) ? "Visible" : "Collapsed";
            }
            //get { return "Visible"; }
        }

        private string _hasMinCommissionVisible = "Collapsed";
        public string HasMinCommissionVisible 
        {
            get { return _hasMinCommissionVisible; }
            set { _hasMinCommissionVisible = value; }
        }
    }
}