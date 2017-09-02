using System;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainProductImageSingleVM : ModelBase
    {
        private ProductResourceIsShow _isShow;

        public ProductResourceIsShow IsShow
        {
            get { return _isShow; }
            set { SetValue("IsShow", ref _isShow, value); }
        }

        public String ResourceUrl { get; set; }

        public String ResourceName { get; set; }

        public int ResourceSysNo { get; set; }

        public int Priority { get; set; }

    }
}
