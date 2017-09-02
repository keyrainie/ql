using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoVIPPriceVM : ModelBase
    {
        private IsUseAlipayVipPrice? _isUseAlipayVipPrice;

        public IsUseAlipayVipPrice? IsUseAlipayVipPrice
        {
            get { return _isUseAlipayVipPrice; }
            set { SetValue("IsUseAlipayVipPrice", ref _isUseAlipayVipPrice, value); }
        }

        private decimal? _alipayVIPPrice;

        public decimal? AlipayVIPPrice
        {
            get { return _alipayVIPPrice; }
            set { SetValue("AlipayVIPPrice", ref _alipayVIPPrice, value); }
        }

        private string _requestAlipayVIPPrice;

        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceName = "ProductMaintain_ProductPriceAlipayVIPPriceInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public string RequestAlipayVIPPrice
        {
            get { return _requestAlipayVIPPrice; }
            set { SetValue("RequestAlipayVIPPrice", ref _requestAlipayVIPPrice, value); }
        }

        public bool HasItemPriceVipPriceMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPriceVipPriceMaintain); }
        }
    }
}
