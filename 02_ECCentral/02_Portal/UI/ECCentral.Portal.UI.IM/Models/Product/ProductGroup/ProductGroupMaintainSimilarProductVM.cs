using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductGroup
{
    public class ProductGroupMaintainSimilarProductVM : ModelBase
    {
        public ProductGroupMaintainSimilarProductVM()
        {
            SelectGroupPropertyValue = new List<SelectGroupProperty>();
            SelectedProduct = new List<SelectProduct>();
        }

        public ProductGroupMaintainVM MainPageVM
        {
            get { return ((Views.ProductGroupMaintain)CPApplication.Current.CurrentPage).VM; }
        }

        public List<SelectGroupProperty> SelectGroupPropertyValue { get; set; }

        public List<ProductInfo> GroupProductList { get; set; }

        private List<SelectProduct> _selectProducts;

        public List<SelectProduct> SelectedProduct
        {
            get { return _selectProducts; }
            set { SetValue("SelectedProduct", ref _selectProducts, value); }
        }
    }

    public class SelectProduct : ModelBase
    {
        public SelectProduct()
        {
            PropertyValueList = new List<PropertyValueVM>();
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue("IsChecked", ref _isChecked, value); }
        }

        public List<PropertyValueVM> PropertyValueList { get; set; }

        private string _productTitle;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductCreate_ProductTitleEmpty", ErrorMessageResourceType = typeof(ResProductCreate))]
        public string ProductTitle
        {
            get { return _productTitle; }
            set { SetValue("ProductTitle", ref _productTitle, value); }
        }

        private string _productModel;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductCreate_ProductModelEmpty", ErrorMessageResourceType = typeof(ResProductCreate))]
        public string ProductModel
        {
            get { return _productModel; }
            set { SetValue("ProductModel", ref _productModel, value); }
        }

        private string _keywords;

        public string Keywords
        {
            get { return _keywords; }
            set { SetValue("Keywords", ref _keywords, value); }
        }

        private string _upcCode;

        public string UPCCode
        {
            get { return _upcCode; }
            set { SetValue("UPCCode", ref _upcCode, value); }
        }

        private string _BMCode;

        public string BMCode
        {
            get { return _BMCode; }
            set { SetValue("BMCode", ref _BMCode, value); }
        }

        private string _virtualPrice;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductCreate_ProductPriceVirtualPriceEmpty", ErrorMessageResourceType = typeof(ResProductCreate))]
        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductCreate_ProductPriceVirtualPriceInvalid", ErrorMessageResourceType = typeof(ResProductCreate))]
        public string VirtualPrice
        {
            get { return _virtualPrice; }
            set { SetValue("VirtualPrice", ref _virtualPrice, value); }
        }

        public override bool Equals(object obj)
        {
            if (obj is SelectProduct)
            {
                var p = obj as SelectProduct;

                if (p.PropertyValueList.OrderBy(vm => vm.PropertySysNo).Select(pr => pr.ValueDescription).Join(" ")
                    == PropertyValueList.OrderBy(vm => vm.PropertySysNo).Select(pr => pr.ValueDescription).Join(" "))
                {
                    return true;
                }
            }
            return false;
        }

        private string _countryCode;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductCreate_CountryCodeEmpty", ErrorMessageResourceType = typeof(ResProductCreate))]
        public string CountryCode
        {
            get { return _countryCode; }
            set { SetValue("CountryCode", ref _countryCode, value); }
        }

        public List<ProductCountry> CountryList { get; set; }
    }

    public class SelectGroupProperty : ModelBase
    {
        public SelectGroupProperty()
        {
            SelectedPropertyValueList = new List<SelectedPropertyValue>();
        }

        public PropertyVM Property { get; set; }

        public List<SelectedPropertyValue> SelectedPropertyValueList { get; set; }
    }

    public class SelectedPropertyValue : ModelBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue("IsChecked", ref _isChecked, value); }
        }

        private PropertyValueVM _propertyValue;

        public PropertyValueVM PropertyValue
        {
            get { return _propertyValue; }
            set { SetValue("PropertyValue", ref _propertyValue, value); }
        }
    }
}
