using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainAccessoryVM : ModelBase
    {
        private ProductIsAccessoryShow _isAccessoryShow;

        public ProductIsAccessoryShow IsAccessoryShow
        {
            get { return _isAccessoryShow; }
            set { SetValue("IsAccessoryShow", ref _isAccessoryShow, value); }
        }

        public List<ProductMaintainAccessoryProductAccessoryVM> ProductAccessoryList { get; set; }

        public bool HasItemBasicInformationMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationMaintain); }
        }
    }
}