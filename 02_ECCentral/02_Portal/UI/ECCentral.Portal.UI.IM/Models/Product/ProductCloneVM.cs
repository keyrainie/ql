using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductCloneVM : ModelBase
    {
        public ProductCloneVM()
        {
            ProductCloneTypeList = new List<KeyValuePair<BizEntity.IM.ProductCloneType?, string>>() 
            {
                new KeyValuePair<BizEntity.IM.ProductCloneType?, string>(ProductCloneType.Auction,ResProductCopy.Bidders),
                new KeyValuePair<BizEntity.IM.ProductCloneType?, string>(ProductCloneType.Gifts,ResProductCopy.Donation),
                new KeyValuePair<BizEntity.IM.ProductCloneType?, string>(ProductCloneType.Bad,ResProductCopy.Spoilage),
            };
            ProductCloneType = ProductCloneType.OpenBox;
        }

        public List<KeyValuePair<ProductCloneType?, string>> ProductCloneTypeList { get; set; }

        private ProductCloneType _productCloneType;

        public ProductCloneType ProductCloneType
        {
            get { return _productCloneType; }
            set { SetValue("ProductCloneType", ref _productCloneType, value); }
        }

        private string _productIDList;

        public string ProductIDList
        {
            get { return _productIDList; }
            set { SetValue("ProductIDList", ref _productIDList, value); }
        }
    }
}
