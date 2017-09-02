using System;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupMaintainBasicInfoVM : ModelBase
    {

        public ProductGroupMaintainBasicInfoVM()
        {
            ProductGroupCategory = new CategoryVM();
            ProductGroupBrand = new BrandVM();
        }

        private String _productGroupName;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductGroupMaintain_ProductGroupNameEmpty", ErrorMessageResourceType = typeof(ResProductGroupMaintain))]
        public String ProductGroupName
        {
            get { return _productGroupName; }
            set { SetValue("ProductGroupName", ref _productGroupName, value); }
        }

        private CategoryVM _productGroupCategory;

        public CategoryVM ProductGroupCategory
        {
            get { return _productGroupCategory; }
            set { SetValue("ProductGroupCategory", ref _productGroupCategory, value); }
        }

        private BrandVM _productGroupBrand;

        public BrandVM ProductGroupBrand
        {
            get { return _productGroupBrand; }
            set { SetValue("ProductGroupBrand", ref _productGroupBrand, value); }
        }

        private String _productGroupModel;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductGroupMaintain_ProductGroupModelEmpty", ErrorMessageResourceType = typeof(ResProductGroupMaintain))]
        public String ProductGroupModel
        {
            get { return _productGroupModel; }
            set { SetValue("ProductGroupModel", ref _productGroupModel, value); }
        }

        public ProductGroupMaintainVM MainPageVM
        {
            get { return ((Views.ProductGroupMaintain)CPApplication.Current.CurrentPage).VM; }
        }
    }
}
