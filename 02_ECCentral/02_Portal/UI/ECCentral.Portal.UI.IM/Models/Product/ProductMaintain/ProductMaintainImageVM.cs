using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainImageVM : ModelBase
    {
        private int _imageCount;

        public int ImageCount
        {
            get
            {
                _imageCount = ProductImageList != null
                    ? ProductImageList.Count(image => image.IsShow == ProductResourceIsShow.Yes)
                    : 0;

                return _imageCount;
            }
            set { SetValue("ImageCount", ref _imageCount, value); }
        }

        private ProductIsVirtualPic _isVirtualPic;

        public ProductIsVirtualPic IsVirtualPic
        {
            get { return _isVirtualPic; }
            set { SetValue("IsVirtualPic", ref _isVirtualPic, value); }
        }

        public List<ProductMaintainProductImageSingleVM> ProductImageList { get; set; }

        public bool HasItemPictureMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPictureMaintain); }
        }
    }
}
