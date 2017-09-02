using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainCommonInfoVM : ModelBase
    {
        private String _productID;

        [Validate(ValidateType.Required)]
        public String ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        public int ProductGroupSysNo { get; set; }
        public String ProductGroupName { get; set; }

        private String _productCategoryName;
        public String ProductCategoryName
        {
            get { return _productCategoryName; }
            set { SetValue("ProductCategoryName", ref _productCategoryName, value); }
        }

        private String _productManufacturerName;
        public String ProductManufacturerName
        {
            get { return _productManufacturerName; }
            set { SetValue("ProductManufacturerName", ref _productManufacturerName, value); }
        }

        private String _productBrandName;
        public String ProductBrandName
        {
            get { return _productBrandName; }
            set { SetValue("ProductBrandName", ref _productBrandName, value); }
        }
        public DateTime? ProductFirstOnSaleDate { get; set; }
        public DateTime ProductLastEditDate { get; set; }

        public List<ProductMaintainCommonInfoGroupProductVM> GroupProductList { get; set; }

        public bool CanBatchOnSale
        {
            get { return GroupProductList.Count > 1; }
        }
    }
}
