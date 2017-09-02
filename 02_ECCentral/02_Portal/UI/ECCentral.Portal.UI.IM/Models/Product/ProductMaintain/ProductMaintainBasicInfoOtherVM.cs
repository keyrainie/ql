using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoOtherVM : ModelBase
    {
        private bool _noExtendWarranty;

        public bool NoExtendWarranty
        {
            get { return _noExtendWarranty; }
            set { SetValue("NoExtendWarranty", ref _noExtendWarranty, value); }
        }

        private ProductInfoFinishStatus _productInfoFinishStatus;

        public ProductInfoFinishStatus ProductInfoFinishStatus
        {
            get { return _productInfoFinishStatus; }
            set { SetValue("ProductInfoFinishStatus", ref _productInfoFinishStatus, value); }
        }

        public bool HasItemBasicInformationInfoFinishMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationInfoFinishMaintain); }
        }
    }
}
