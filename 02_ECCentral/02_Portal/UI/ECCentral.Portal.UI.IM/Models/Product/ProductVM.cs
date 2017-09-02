using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductVM : ModelBase
    {
        public ProductVM()
        {
            ProductMaintainCommonInfo = new ProductMaintainCommonInfoVM();
            ProductMaintainBasicInfo = new ProductMaintainBasicInfoVM();
            ProductMaintainDescription = new ProductMaintainDescriptionVM();
            ProductMaintainAccessory = new ProductMaintainAccessoryVM();
            ProductMaintainImage = new ProductMaintainImageVM();
            ProductMaintainPriceInfo = new ProductMaintainPriceInfoVM();
            ProductMaintainProperty = new ProductMaintainPropertyVM();
            ProductMaintainWarranty = new ProductMaintainWarrantyVM();
            ProductMaintainDimension = new ProductMaintainDimensionVM();
            ProductMaintainSalesArea = new ProductMaintainSalesAreaVM();
            ProductMaintainPurchaseInfo = new ProductMaintainPurchaseInfoVM();
            ProductMaintainBatchManagementInfo = new ProductMaintainBatchManagementInfoVM();
            ProductMaintainStepPrice = new ProductMaintainStepPriceVM();
        }

        public ProductMaintainCommonInfoVM ProductMaintainCommonInfo { get; set; }

        public ProductMaintainBasicInfoVM ProductMaintainBasicInfo { get; set; }

        public ProductMaintainDescriptionVM ProductMaintainDescription { get; set; }

        public ProductMaintainAccessoryVM ProductMaintainAccessory { get; set; }

        public ProductMaintainImageVM ProductMaintainImage { get; set; }

        public ProductMaintainPriceInfoVM ProductMaintainPriceInfo { get; set; }

        public ProductMaintainPropertyVM ProductMaintainProperty { get; set; }

        public ProductMaintainWarrantyVM ProductMaintainWarranty { get; set; }

        public ProductMaintainDimensionVM ProductMaintainDimension { get; set; }

        public ProductMaintainSalesAreaVM ProductMaintainSalesArea { get; set; }

        public ProductMaintainPurchaseInfoVM ProductMaintainPurchaseInfo { get; set; }

        public ProductMaintainBatchManagementInfoVM ProductMaintainBatchManagementInfo { get; set; }

        public ProductMaintainStepPriceVM ProductMaintainStepPrice { get; set; }


        public int ProductSysNo { get; set; }

        private string _note;

        public string Note
        {
            get { return _note; }
            set { SetValue("Note", ref _note, value); }
        }

        public bool HasItemChangeProductStatusToUnShowPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemChangeProductStatusToUnShow); }
        }

        public bool HasItemMaintainAllTypePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemMaintainAllType); }
        }
    }
}