using System;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoDescriptionInfoVM : ModelBase
    {
        private String _productDescription;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDescriptionShortEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String ProductDescription
        {
            get { return _productDescription; }
            set { SetValue("ProductDescription", ref _productDescription, value); }
        }

        private String _performance;

        public String Performance
        {
            get { return _performance; }
            set { SetValue("Performance", ref _performance, value); }
        }

        private String _packageList;

        public String PackageList
        {
            get { return _packageList; }
            set { SetValue("PackageList", ref _packageList, value); }
        }

        private String _productLink;

        [Validate(ValidateType.Regex, @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", ErrorMessageResourceName = "ProductMaintain_ProductLinkInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String ProductLink
        {
            get { return _productLink; }
            set { SetValue("ProductLink", ref _productLink, value); }
        }

        private String _attention;

        [Validate(ValidateType.MaxLength,500)]
        public String Attention
        {
            get { return _attention; }
            set { SetValue("Attention", ref _attention, value); }
        }

        public ProductVM MainPageVM
        {
            get { return ((Views.ProductMaintain)CPApplication.Current.CurrentPage).VM; }
        }
    }
}
