using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainCommonInfoGroupProductVM : ModelBase
    {
        private String _productID;

        public String ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        private List<String> _propertyValueList;

        public List<String> PropertyValueList
        {
            get { return _propertyValueList; }
            set { SetValue("PropertyValueList", ref _propertyValueList, value); }
        }

        private ProductStatus _productStatus;

        public ProductStatus ProductStatus
        {
            get { return _productStatus; }
            set { SetValue("ProductStatus", ref _productStatus, value); }
        }

        private bool _isChecked;
        
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue("IsChecked", ref _isChecked, value); }
        }

        public String PrductIDLinkColor { get; set; }

        public int ProductSysNo { get; set; }

        private String _productGroupProperties;

        public String ProductGroupProperties
        {
            get { return _productGroupProperties; }
            set { SetValue("ProductGroupProperties", ref _productGroupProperties, value); }
        }

    }
}
