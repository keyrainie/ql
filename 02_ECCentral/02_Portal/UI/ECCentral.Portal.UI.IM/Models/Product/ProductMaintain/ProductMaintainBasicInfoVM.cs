using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoVM : ModelBase
    {

        public ProductMaintainBasicInfoVM()
        {
            ProductMaintainBasicInfoSpecificationInfo = new ProductMaintainBasicInfoSpecificationInfoVM();
            ProductMaintainBasicInfoDisplayInfo = new ProductMaintainBasicInfoDisplayInfoVM();
            ProductMaintainBasicInfoStatusInfo = new ProductMaintainBasicInfoStatusInfoVM();
            ProductMaintainBasicInfoChannelInfo = new ProductMaintainBasicInfoChannelInfoVM();
            ProductMaintainBasicInfoDescriptionInfo = new ProductMaintainBasicInfoDescriptionInfoVM();
            ProductMaintainBasicInfoOther = new ProductMaintainBasicInfoOtherVM();
        }

        public ProductMaintainBasicInfoDisplayInfoVM ProductMaintainBasicInfoDisplayInfo { get; set; }

        public ProductMaintainBasicInfoStatusInfoVM ProductMaintainBasicInfoStatusInfo { get; set; }

        public ProductMaintainBasicInfoSpecificationInfoVM ProductMaintainBasicInfoSpecificationInfo { get; set; }

        public ProductMaintainBasicInfoChannelInfoVM ProductMaintainBasicInfoChannelInfo { get; set; }

        public ProductMaintainBasicInfoDescriptionInfoVM ProductMaintainBasicInfoDescriptionInfo { get; set; }

        public ProductMaintainBasicInfoOtherVM ProductMaintainBasicInfoOther { get; set; }

        public bool HasItemBasicInformationMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationMaintain); }
        }
    }
}
