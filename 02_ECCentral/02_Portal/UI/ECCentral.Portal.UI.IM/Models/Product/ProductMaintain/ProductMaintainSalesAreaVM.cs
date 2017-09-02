using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainSalesAreaVM : ModelBase
    {
        public ProductMaintainSalesAreaVM()
        {
            ProductMaintainSalesAreaSelect = new ProductMaintainSalesAreaSelectVM();
            ProductMaintainSalesAreaSaveList = new List<ProductMaintainSalesAreaSelectVM>();
        }

        public ProductMaintainSalesAreaSelectVM ProductMaintainSalesAreaSelect { get; set; }

        public List<ProductMaintainSalesAreaSelectVM> ProductMaintainSalesAreaSaveList { get; set; }

        public bool HasItemRegionSalesMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemRegionSalesMaintain); }
        }
    }
}
