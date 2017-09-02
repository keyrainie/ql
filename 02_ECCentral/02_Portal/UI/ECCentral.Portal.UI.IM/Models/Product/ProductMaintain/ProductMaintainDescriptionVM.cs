using System;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainDescriptionVM : ModelBase
    {
        private String _productLongDescription;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDescriptionLongEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String ProductLongDescription
        {
            get { return _productLongDescription; }
            set { SetValue("ProductLongDescription", ref _productLongDescription, value); }
        }

        private String _productPhotoDescription;

        public String ProductPhotoDescription
        {
            get { return _productPhotoDescription; }
            set { SetValue("ProductPhotoDescription", ref _productPhotoDescription, value); }
        }

        public bool HasItemBasicInformationMaintainPermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationMaintain) ||
                       AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemDescriptionMaintain);
            }
        }
    }
}