using System.Collections.Generic;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPropertyVM : ModelBase
    {
        public List<ProductMaintainPropertyPropertyValueVM> ProductPropertyValueList { get; set; }

        public bool HasItemPropertyMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPropertyMaintain); }
        }
    }
}
