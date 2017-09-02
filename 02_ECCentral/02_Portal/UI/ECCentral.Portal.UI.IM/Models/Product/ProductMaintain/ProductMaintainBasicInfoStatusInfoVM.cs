using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoStatusInfoVM : ModelBase
    {
        private ProductStatus _productStatus;

        public ProductStatus ProductStatus
        {
            get { return _productStatus; }
            set
            {
                SetValue("ProductStatus", ref _productStatus, value);
            }
        }
    }
}
