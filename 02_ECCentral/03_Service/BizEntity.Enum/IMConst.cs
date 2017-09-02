using System;

namespace ECCentral.BizEntity.IM
{
    public static class IMConst
    {
        /// <summary>
        /// 商品默认初始价格
        /// </summary>
        public const Int32 ProductDefaultPrice = 999999;

        public const Int32 ProductPriceZero = 0;

        public const Int32 ProductWeightZero = 0;

        #region Resources

        public const String IMProductMessageResourcesKey = "IM.Product";

        public const String IMCategoryMessageResourcesKey = "IM.Category";

        public const String IMBrandMessageResourcesKey = "IM.Brand";

        public const String IMCategoryMRCategoryInvalid = "CategoryInvalid";

        public const String IMBrandMRCategoryInvalid = "BrandInvalid";

        public const String IMProductMRProductTitleEmpty = "ProductTitleEmpty";

        public const String IMProductMRProductModelEmpty = "ProductModelEmpty";

        public const String IMProductMRShortDescriptionEmpty = "ShortDescriptionEmpty";

        public const String IMProductMRLongDescriptionEmpty = "LongDescriptionEmpty";

        public const String IMProductMRProductPropertiesEmpty = "ProductPropertiesEmpty";

        public const String IMProductMRProductNoImages = "ProductNoImages";

        public const String IMProductMRProductBasicPriceInvalid = "ProductBasicPriceInvalid";

        public const String IMProductMRProductCurrentPriceInvalid = "ProductCurrentPriceInvalid";

        public const String IMProductMRProductWeightInvalid = "ProductWeightInvalid";

        public const String IMProductMRProductIsGift = "ProductIsGift";

        public const String IMProductMRProductIsAttachment = "ProductIsAttachment";

        public const String IMProductMROnlyActiveProductCanOffSale = "OnlyActiveProductCanOffSale";

        public const String IMProductMRProductInfoGroupPropertyEmpty = "ProductInfoGroupPropertyEmpty";

        public const String IMProductMRProductInfoGroupPropertyDuplicate = "ProductInfoGroupPropertyDuplicate";

        //商品上架税则信息检查——Start
         
        public const String IMProductTariffCode = "ProductNeedTariffCode";//税则号

        public const String IMProductTariffRate = "ProductNeedTariffRate";//税率

        public const String IMProductEntryCode = "ProductNeedEntryCode";//备案号

        public const String IMProductEntryInfo = "ProductNeedEntryInfo";//自贸区商品必填信息

        public const String IMProductSKUNO = "ProductSKUNO";//商品货号

        public const String IMSuppliesSerialNo = "SuppliesSerialNo";//商品序号

        public const String IMProductApplyUnit = "ProductApplyUnit";///申报单位

        public const String IMProductApplyQty = "ProductApplyQty";//申报数量

        public const String IMProductGrossWeight = "ProductGrossWeight";//毛重

        public const String IMProductSuttleWeight = "ProductSuttleWeight";//净重

        //商品上架税则信息检查——End

        #endregion

    }
}