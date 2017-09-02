using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPurchaseInfoVM : ModelBase
    {
        private string _minPackCount;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductPurchaseInfoMinPackCountEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$", ErrorMessageResourceName = "ProductMaintain_ProductPurchaseInfoMinPackCountInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string MinPackCount
        {
            get { return _minPackCount; }
            set { SetValue("MinPackCount", ref _minPackCount, value); }
        }
        public ProductMaintainPurchaseInfoVM()
        {
            this.InventoryTypeList = EnumConverter.GetKeyValuePairs<ProductInventoryType>(EnumConverter.EnumAppendItemType.None);
        }
        private string _pOMemo;

        public string POMemo
        {
            get { return _pOMemo; }
            set { SetValue("POMemo", ref _pOMemo, value); }
        }

        public string HasItemDisplaycolumnPermissionVisible
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemDisplaycolumn) ? "Visible" : "Collapsed";
            }
        }
        public List<KeyValuePair<ProductInventoryType?, string>> InventoryTypeList { get; set; }

        private ProductInventoryType _InventoryType;
        public ProductInventoryType InventoryType
        {
            get
            {
                return _InventoryType;
            }
            set
            {
                SetValue("InventoryType", ref _InventoryType, value);
            }
        }

        private int? _ERPProductID;
        public int? ERPProductID
        {
            get
            {
                return _ERPProductID;
            }
            set
            {
                SetValue("ERPProductID", ref _ERPProductID, value);
            }
        }
    }
}
