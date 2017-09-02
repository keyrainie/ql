using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupMaintainVM : ModelBase
    {
        public ProductGroupMaintainVM()
        {
            BasicInfoVM = new ProductGroupMaintainBasicInfoVM();
            ProductListVM = new ProductGroupMaintainProductListVM();
            PropertyVM = new ProductGroupMaintainPropertySettingVM();
        }

        public int ProductGroupSysNo { get; set; }

        public ProductGroupMaintainBasicInfoVM BasicInfoVM { get; set; }

        public ProductGroupMaintainProductListVM ProductListVM { get; set; }

        public ProductGroupMaintainPropertySettingVM PropertyVM { get; set; }

        public bool CreateFlag { get; set; }
    }
}
