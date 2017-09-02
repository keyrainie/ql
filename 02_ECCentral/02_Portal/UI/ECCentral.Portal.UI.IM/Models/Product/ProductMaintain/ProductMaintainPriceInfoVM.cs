using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoVM : ModelBase
    {
        public ProductMaintainPriceInfoVM()
        {
            ProductMaintainPriceInfoBasicPrice = new ProductMaintainPriceInfoBasicPriceVM();
            ProductMaintainPriceInfoVolumePrice = new ProductMaintainPriceInfoVolumePriceVM();
            ProductMaintainPriceInfoRankPrice = new ProductMaintainPriceInfoRankPriceVM();
            ProductMaintainPriceInfoAutoPrice = new ProductMaintainPriceInfoAutoPriceVM();
            ProductMaintainPriceInfoVIPPrice = new ProductMaintainPriceInfoVIPPriceVM();
            ProductMaintainPriceInfoAuditMemo = new ProductMaintainPriceInfoAuditMemoVM();
        }

        public ProductMaintainPriceInfoAutoPriceVM ProductMaintainPriceInfoAutoPrice { get; set; }

        public ProductMaintainPriceInfoBasicPriceVM ProductMaintainPriceInfoBasicPrice { get; set; }

        public ProductMaintainPriceInfoVolumePriceVM ProductMaintainPriceInfoVolumePrice { get; set; }

        public ProductMaintainPriceInfoRankPriceVM ProductMaintainPriceInfoRankPrice { get; set; }

        public ProductMaintainPriceInfoVIPPriceVM ProductMaintainPriceInfoVIPPrice { get; set; }

        public ProductMaintainPriceInfoAuditMemoVM ProductMaintainPriceInfoAuditMemo { get; set; }

        private ProductPriceRequestStatus? _productPriceRequestStatus;

        public ProductPriceRequestStatus? ProductPriceRequestStatus
        {
            get { return _productPriceRequestStatus; }
            set { SetValue("ProductPriceRequestStatus", ref _productPriceRequestStatus, value); }
        }

        private Merchant _productMerchant;
        public Merchant ProductMerchant
        {
            get { return _productMerchant; }
            set { SetValue("ProductMerchant", ref _productMerchant, value); }
        }

        public bool HasItemPriceMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemPriceMaintain) && (!IsSellerProduct); }
            //get { return true; }
        }

        public bool IsSellerProduct
        {
            get { return ProductMerchant != null 
                        && ProductMerchant.MerchantID.HasValue 
                        && ProductMerchant.MerchantID > 1; }
        }
        
    }
}
