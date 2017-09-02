using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainDimensionVM : ModelBase
    {
        public ProductMaintainDimensionVM()
        {
            LargeFlagList = EnumConverter.GetKeyValuePairs<Large>(EnumConverter.EnumAppendItemType.None);
        }

        public List<KeyValuePair<Large?, string>> LargeFlagList { get; set; }

        private String _weight;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDimensionWeightEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Interger,ErrorMessageResourceName = "ProductMaintain_ProductDimensionWeightInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String Weight
        {
            get { return _weight; }
            set { SetValue("Weight", ref _weight, value); }
        }

        private Large _largeFlag;

        public Large LargeFlag
        {
            get { return _largeFlag; }
            set { SetValue("LargeFlag", ref _largeFlag, value); }
        }

        private String _length;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDimensionLengthEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]+(.[0-9]{0,2})?|0.[0-9][1-9]|0.[1-9][0-9]", ErrorMessageResourceName = "ProductMaintain_ProductDimensionLengthInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String Length
        {
            get { return _length; }
            set { SetValue("Length", ref _length, value); }
        }

        private String _width;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDimensionWidthEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]+(.[0-9]{0,2})?|0.[0-9][1-9]|0.[1-9][0-9]", ErrorMessageResourceName = "ProductMaintain_ProductDimensionWidthInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String Width
        {
            get { return _width; }
            set { SetValue("Width", ref _width, value); }
        }

        private String _height;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductDimensionHeightEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]+(.[0-9]{0,2})?|0.[0-9][1-9]|0.[1-9][0-9]", ErrorMessageResourceName = "ProductMaintain_ProductDimensionHeightInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String Height
        {
            get { return _height; }
            set { SetValue("Height", ref _height, value); }
        }

        public bool HasItemWeightMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemWeightMaintain); }
        }

        public bool HasItemDimensionMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemDimensionMaintain); }
        }
    }
}