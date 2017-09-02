using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainThirdPartyInventoryVM : ModelBase
    {
        public ProductMaintainThirdPartyInventoryVM()
        {
            ThirdPartnerList = EnumConverter.GetKeyValuePairs<ThirdPartner>(EnumConverter.EnumAppendItemType.None);

            StockRulesList = EnumConverter.GetKeyValuePairs<StockRules>(EnumConverter.EnumAppendItemType.None);
        }

        public List<KeyValuePair<ThirdPartner?, string>> ThirdPartnerList { get; set; }

        public List<KeyValuePair<StockRules?, string>> StockRulesList { get; set; }

        private ThirdPartner _thirdPartner;

        public ThirdPartner ThirdPartner
        {
            get { return _thirdPartner; }
            set { SetValue("ThirdPartner", ref _thirdPartner, value); }
        }

        private string _synProductID;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductThirdPartyInventorySynProductIDEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string SynProductID
        {
            get { return _synProductID; }
            set { SetValue("SynProductID", ref _synProductID, value); }
        }

        private StockRules _stockRules;

        public StockRules StockRules
        {
            get { return _stockRules; }
            set { SetValue("StockRules", ref _stockRules, value); }
        }

        private string _limitCount;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductThirdPartyInventoryLimitCountEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductThirdPartyInventoryLimitCountInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string LimitCount
        {
            get { return _limitCount; }
            set { SetValue("LimitCount", ref _limitCount, value); }
        }

        private ProductMappingStatus _status;

        public ProductMappingStatus Status
        {
            get { return _status; }
            set { SetValue("Status", ref _status, value); }
        }

        private string _directVisible;

        public string DirectVisible
        {
            get { return _directVisible; }
            set { SetValue("DirectVisible", ref _directVisible, value); }
        }
    }
}
