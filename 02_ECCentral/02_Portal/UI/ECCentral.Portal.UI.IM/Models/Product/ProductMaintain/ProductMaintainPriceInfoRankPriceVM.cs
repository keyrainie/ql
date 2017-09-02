using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoRankPriceVM : ModelBase
    {
        public List<ProductRankPriceVM> ProductRankPriceList { get; set; }
    }

    public class ProductRankPriceVM : ModelBase
    {
        private CustomerRank _customerRank;

        public CustomerRank CustomerRank
        {
            get { return _customerRank; }
            set { SetValue("CustomerRank", ref _customerRank, value); }
        }

        private decimal? _rankPrice;

        public decimal? RankPrice
        {
            get { return _rankPrice; }
            set { SetValue("RankPrice", ref _rankPrice, value); }
        }

        private ProductRankPriceStatus? _status;

        public ProductRankPriceStatus? Status
        {
            get { return _status; }
            set { SetValue("Status", ref _status, value); }
        }

        private String _requestRankPrice;

        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceRankPriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String RequestRankPrice
        {
            get { return _requestRankPrice; }
            set { SetValue("RequestRankPrice", ref _requestRankPrice, value); }
        }
    }
}
