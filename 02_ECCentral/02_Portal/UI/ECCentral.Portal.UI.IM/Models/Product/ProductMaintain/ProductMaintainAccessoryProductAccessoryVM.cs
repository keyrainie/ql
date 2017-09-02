using System;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainAccessoryProductAccessoryVM : ModelBase
    {
        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { base.SetValue("SysNo", ref _sysNo, value); }
        }

        private ProductAccessoryStatus _isShow;

        public ProductAccessoryStatus IsShow
        {
            get { return _isShow; }
            set { SetValue("IsShow", ref _isShow, value); }
        }

        public AccessoryVM Accessory { get; set; }

        private string _qty;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductAccessoryQtyEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "ProductMaintain_ProductAccessoryQtyInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string Qty
        {
            get { return _qty; }
            set { SetValue("Qty", ref _qty, value); }
        }

        private String _description;

        [Validate(ValidateType.MaxLength, 50)]
        public String Description
        {
            get { return _description; }
            set { SetValue("Description", ref _description, value); }
        }

        private string _priority;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductAccessoryPriorityEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "ProductMaintain_ProductAccessoryPriorityInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string Priority
        {
            get { return _priority; }
            set { SetValue("Priority", ref _priority, value); }
        }
    }
}