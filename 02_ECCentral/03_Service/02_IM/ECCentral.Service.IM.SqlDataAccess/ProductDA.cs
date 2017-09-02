using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Text;


namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductDA))]
    public class ProductDA : IProductDA
    {
        private readonly IProductPriceDA _priceDA = ObjectFactory<IProductPriceDA>.Instance;

        private readonly IProductResourceDA _productResourceDA = ObjectFactory<IProductResourceDA>.Instance;

        private readonly IProductPriceRequestDA _priceRequestDA = ObjectFactory<IProductPriceRequestDA>.Instance;

        private readonly IProductAttachmentDA _productAttachmentDA = ObjectFactory<IProductAttachmentDA>.Instance;
        private readonly IProductExtDA _productExtDA = ObjectFactory<IProductExtDA>.Instance;

        #region 添加商品信息

        public void InsertProductInfo(ProductInfo productInfo)
        {
            productInfo.SysNo = GetProductSequence();
            InsertProduct(productInfo);
            InsertProductEx(productInfo);
            InsertProductStatus(productInfo);
            InsertProductPrice(productInfo);
            InsertProductDimension(productInfo);
            InsertProductVirtualPriceLog(productInfo);

            if (productInfo.ProductBasicInfo.ProductProperties != null)
            {
                productInfo.ProductBasicInfo.ProductProperties.ForEach(
                    property =>
                    {
                        if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                        {
                            if (property.OperationUser == null)
                            {
                                property.OperationUser = productInfo.OperateUser;
                            }
                            property.SysNo = InsertProductProperty(productInfo.SysNo, productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value, property);
                        }

                    });
            }
        }

        private int GetProductSequence()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductSequence");
            cmd.ExecuteNonQuery();
            var result = (int)cmd.GetParameterValue("@SysNo");
            return result;
        }

        private void InsertProduct(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProduct");
            cmd.SetParameterValue("@SysNo", productInfo.SysNo);
            cmd.SetParameterValue("@ProductID", productInfo.ProductID);
            cmd.SetParameterValue("@ProductMode", productInfo.ProductBasicInfo.ProductModel.Content);
            cmd.SetParameterValue("@ProductType", productInfo.ProductBasicInfo.ProductType);
            cmd.SetParameterValue("@ProductName", productInfo.ProductName);
            cmd.SetParameterValue("@ProductDesc", productInfo.ProductBasicInfo.ShortDescription.Content);
            cmd.SetParameterValue("@ProductDescLong", productInfo.ProductBasicInfo.LongDescription.Content);
            cmd.SetParameterValue("@Performance", productInfo.ProductBasicInfo.Performance);
            cmd.SetParameterValue("@Warranty", productInfo.ProductWarrantyInfo.Warranty.Content);
            cmd.SetParameterValue("@PackageList", productInfo.ProductBasicInfo.PackageList.Content);
            cmd.SetParameterValue("@Weight", productInfo.ProductBasicInfo.ProductDimensionInfo.Weight);
            cmd.SetParameterValue("@C3SysNo", productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo);
            cmd.SetParameterValue("@ManufacturerSysNo", productInfo.ProductBasicInfo.ProductBrandInfo.Manufacturer.SysNo);
            cmd.SetParameterValue("@ProductLink", productInfo.ProductBasicInfo.ProductLink);
            cmd.SetParameterValue("@PMUserSysNo", productInfo.ProductBasicInfo.ProductManager.UserInfo.SysNo);
            cmd.SetParameterValue("@PPMUserSysNo", null);
            cmd.SetParameterValue("@CreateUserSysNo", productInfo.OperateUser.SysNo == null ? ServiceContext.Current.UserSysNo : productInfo.OperateUser.SysNo);
            cmd.SetParameterValue("@CreateTime", DateTime.Now);
            cmd.SetParameterValue("@Attention", productInfo.ProductBasicInfo.Attention.Content);
            cmd.SetParameterValue("@Note", productInfo.ProductBasicInfo.Note);
            cmd.SetParameterValue("@BarCode", productInfo.ProductBasicInfo.UPCCode);
            cmd.SetParameterValue("@Status", productInfo.ProductStatus);
            cmd.SetParameterValue("@IsLarge", productInfo.ProductBasicInfo.ProductDimensionInfo.LargeFlag);
            cmd.SetParameterValue("@OrderByDateTime", null);
            cmd.SetParameterValue("@BriefName", string.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductBriefName) ? productInfo.ProductName : productInfo.ProductBasicInfo.ProductBriefName);
            cmd.SetParameterValue("@RemarkCount", null);
            cmd.SetParameterValue("@RemarkScore", null);
            cmd.SetParameterValue("@IsConsign", productInfo.ProductConsignFlag);
            cmd.SetParameterValue("@OwnerList", null);
            cmd.SetParameterValue("@PromotionTitle", productInfo.PromotionTitle.Content);
            cmd.SetParameterValue("@ProductTitle", productInfo.ProductBasicInfo.ProductTitle.Content);
            cmd.SetParameterValue("@ProductLine", string.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductBriefTitle.Content) ? productInfo.ProductName : productInfo.ProductBasicInfo.ProductBriefTitle.Content);
            cmd.SetParameterValue("@BriefNameAddition", productInfo.ProductBasicInfo.ProductBriefAddition.Content);
            cmd.SetParameterValue("@IsHaveValidGift", 0);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@CurrencySysNo", 1);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@ProductPhotoDesc", productInfo.ProductBasicInfo.PhotoDescription.Content);
            cmd.SetParameterValue("@VFItem", null);
            cmd.SetParameterValue("@BrandSysNo", productInfo.ProductBasicInfo.ProductBrandInfo.SysNo);
            cmd.SetParameterValue("@ProductCommonInfoSysno", productInfo.ProductCommonInfoSysNo);
            cmd.SetParameterValue("@MerchantSysNo", productInfo.Merchant.SysNo);
            cmd.SetParameterValue("@DefaultImage", null);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductEx(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductEx");
            cmd.SetParameterValue("@SysNo", productInfo.SysNo);
            cmd.SetParameterValue("@IsPermitRefund", 1);
            cmd.SetParameterValue("@IsOfferValueAddedInvoice", productInfo.ProductWarrantyInfo.OfferVATInvoice);
            cmd.SetParameterValue("@DMSValue", 0);
            cmd.SetParameterValue("@SaleDays", 0);
            cmd.SetParameterValue("@InventoryDays", 0);
            cmd.SetParameterValue("@CompanyProduct", 0);
            cmd.SetParameterValue("@PreviousProductSysNo", null);
            cmd.SetParameterValue("@BuyOnlyForRank", 0);
            cmd.SetParameterValue("@IsInstalmentProduct", 0);
            cmd.SetParameterValue("@InstalmentPhase", 0);
            cmd.SetParameterValue("@HoldMark", null);
            cmd.SetParameterValue("@HoldDate", null);
            cmd.SetParameterValue("@HoldUser", null);
            cmd.SetParameterValue("@HoldReason", null);
            cmd.SetParameterValue("@NoInventoryDays", 0);
            cmd.SetParameterValue("@AccessoriesIsShow", productInfo.ProductBasicInfo.IsAccessoryShow);
            cmd.SetParameterValue("@AccessoriesMemo", null);
            cmd.SetParameterValue("@Keywords", productInfo.ProductBasicInfo.Keywords.Content);
            cmd.SetParameterValue("@IsTakePictures", productInfo.ProductBasicInfo.IsTakePicture);
            cmd.SetParameterValue("@ProductNotifyTimes", 0);
            cmd.SetParameterValue("@ProductColor", null);
            cmd.SetParameterValue("@HostWarrantyDay", productInfo.ProductWarrantyInfo.HostWarrantyDay);
            cmd.SetParameterValue("@PartWarrantyDay", productInfo.ProductWarrantyInfo.PartWarrantyDay);
            cmd.SetParameterValue("@ServicePhone", productInfo.ProductWarrantyInfo.ServicePhone);
            cmd.SetParameterValue("@ServiceInfo", productInfo.ProductWarrantyInfo.ServiceInfo);
            cmd.SetParameterValue("@WeighupByWMS", null);
            cmd.SetParameterValue("@IsChangeStylewithTemplate", 1);
            cmd.SetParameterValue("@AutoAdjustPrice", 0);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@AutoPricingStartDate", null);
            cmd.SetParameterValue("@AutoPricingEndDate", null);
            cmd.SetParameterValue("@UnshownAllowSearch", null);
            cmd.SetParameterValue("@ImageVersion", null);
            cmd.SetParameterValue("@EditDate", null);
            cmd.SetParameterValue("@EditUser", null);
            cmd.SetParameterValue("@MinPackNumber", productInfo.ProductPOInfo.MinPackNumber);
            cmd.SetParameterValue("@PO_Memo", null);
            cmd.SetParameterValue("@Keywords0", null);
            cmd.SetParameterValue("@IsBatch", "N");
            cmd.SetParameterValue("@ProductInfoFinishStatus", ProductInfoFinishStatus.No);
            cmd.SetParameterValue("@ProductTradeType", productInfo.ProductBasicInfo.TradeType);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductStatus(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductStatus");
            cmd.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            cmd.SetParameterValue("@InfoStatus", 1);
            cmd.SetParameterValue("@InfoUserSysNo", null);
            cmd.SetParameterValue("@InfoTime", null);
            cmd.SetParameterValue("@PicStatus", 1);
            cmd.SetParameterValue("@PicUserSysNo", null);
            cmd.SetParameterValue("@PicTime", null);
            cmd.SetParameterValue("@WarrantyStatus", 1);
            cmd.SetParameterValue("@WarrantyUserSysNo", null);
            cmd.SetParameterValue("@WarrantyTime", null);
            cmd.SetParameterValue("@PriceStatus", 1);
            cmd.SetParameterValue("@PriceUserSysNo", null);
            cmd.SetParameterValue("@PriceTime", null);
            cmd.SetParameterValue("@WeightStatus", 1);
            cmd.SetParameterValue("@WeightUserSysNo", null);
            cmd.SetParameterValue("@WeightTime", null);
            cmd.SetParameterValue("@AllowStatus", 1);
            cmd.SetParameterValue("@AllowUserSysNo", null);
            cmd.SetParameterValue("@AllowTime", null);
            cmd.SetParameterValue("@FirstOnlineTime", null);
            cmd.SetParameterValue("@LastOnlineTime", null);
            cmd.SetParameterValue("@virtualPicStatus", 0);
            cmd.SetParameterValue("@AccessoryStatus", null);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductPrice(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductPrice");
            cmd.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            cmd.SetParameterValue("@BasicPrice", productInfo.ProductPriceInfo.BasicPrice);
            cmd.SetParameterValue("@CurrentPrice", productInfo.ProductPriceInfo.CurrentPrice);
            cmd.SetParameterValue("@UnitCost", productInfo.ProductPriceInfo.UnitCost);
            cmd.SetParameterValue("@Discount", productInfo.ProductPriceInfo.DiscountAmount);
            cmd.SetParameterValue("@PointType", productInfo.ProductPriceInfo.PayType);
            cmd.SetParameterValue("@MaxPerOrder", productInfo.ProductPriceInfo.MaxCountPerDay);
            cmd.SetParameterValue("@IsWholeSale", 0);
            cmd.SetParameterValue("@Q1", null);
            cmd.SetParameterValue("@P1", null);
            cmd.SetParameterValue("@Q2", null);
            cmd.SetParameterValue("@P2", null);
            cmd.SetParameterValue("@Q3", null);
            cmd.SetParameterValue("@P3", null);
            cmd.SetParameterValue("@CashRebate", productInfo.ProductPriceInfo.CashRebate);
            cmd.SetParameterValue("@Point", productInfo.ProductPriceInfo.Point);
            cmd.SetParameterValue("@ClearanceSale", 0);
            cmd.SetParameterValue("@CreateTime", DateTime.Now);
            cmd.SetParameterValue("@UnitCostWithoutTax", productInfo.ProductPriceInfo.UnitCostWithoutTax);
            cmd.SetParameterValue("@IsExistRankPrice", 0);
            cmd.SetParameterValue("@PMMemo", null);
            cmd.SetParameterValue("@TLMemo", null);
            cmd.SetParameterValue("@LastOnSaleTime", null);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@CurrencySysNo", 1);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@ReservedMaxPerOrder", null);
            cmd.SetParameterValue("@VirtualPrice", productInfo.ProductPriceInfo.VirtualPrice);
            cmd.SetParameterValue("@SupplyMemo", null);
            cmd.SetParameterValue("@SupplyPrice", null);
            cmd.SetParameterValue("@MinCommission", null);
            cmd.SetParameterValue("@IsUseAlipayVipPrice", null);
            cmd.SetParameterValue("@AlipayVipPrice", null);
            cmd.SetParameterValue("@MinCountPerOrder", 1);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductVirtualPriceLog(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductVirtualPriceLog");
            cmd.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            cmd.SetParameterValue("@NewVirtualPrice", productInfo.ProductPriceInfo.VirtualPrice);
            cmd.SetParameterValue("@OldVirtualPrice", 0);
            cmd.SetParameterValue("@Note", null);
            cmd.SetParameterValue("@OptIP", ServiceContext.Current.ClientIP);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@CreateUser", productInfo.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@CreateDate", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductDimension(ProductInfo productInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductDimension");
            cmd.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            cmd.SetParameterValue("@Length", productInfo.ProductBasicInfo.ProductDimensionInfo.Length);
            cmd.SetParameterValue("@Width", productInfo.ProductBasicInfo.ProductDimensionInfo.Width);
            cmd.SetParameterValue("@Height", productInfo.ProductBasicInfo.ProductDimensionInfo.Height);
            cmd.SetParameterValue("@InDate", DateTime.Now);
            cmd.SetParameterValue("@InUser", productInfo.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@EditDate", DateTime.Now);
            cmd.SetParameterValue("@EditUser", productInfo.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            cmd.SetParameterValue("@CountryCode", null);
            cmd.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            cmd.SetParameterValue("@Status", "A");
            cmd.ExecuteNonQuery();
        }

        private int InsertProductProperty(int productSysNo, int? categorySysNo, ProductProperty productProperty)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductProperty");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            int propertyGroupSysNo = 0;
            if (productProperty.PropertyGroup == null || !productProperty.PropertyGroup.SysNo.HasValue)
            {
                //添加Similar Product时无法取到ProductPropertyGroup信息，因此这里重新取
                if (productProperty.Property.PropertyInfo.SysNo.HasValue)
                {
                    propertyGroupSysNo =
                        ObjectFactory<IPropertyDA>.Instance
                        .GetPropertyGroupSysNoByPropertySysNo(categorySysNo, productProperty.Property.PropertyInfo.SysNo.Value);
                }
            }
            else
            {
                propertyGroupSysNo = productProperty.PropertyGroup.SysNo.Value;
            }
            dc.SetParameterValue("@GroupSysNo", propertyGroupSysNo);
            dc.SetParameterValue("@PropertySysNo", productProperty.Property.PropertyInfo.SysNo);
            dc.SetParameterValue("@ValueSysNo", productProperty.Property.SysNo.HasValue ? productProperty.Property.SysNo.Value : 0);
            dc.SetParameterValue("@ManualInput", productProperty.PersonalizedValue.Content);
            dc.SetParameterValue("@InUserSysNo", productProperty.OperationUser.SysNo);
            dc.SetParameterValue("@CompanyCode", productProperty.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productProperty.LanguageCode);
            return dc.ExecuteScalar<int>();
        }

        #endregion

        #region 更新商品信息

        public int GetProductCountExcept(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductCountExcept");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@BrandSysNo", productInfo.ProductBasicInfo.ProductBrandInfo.SysNo);
            dc.SetParameterValue("@CategorySysNo", productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo);
            dc.SetParameterValue("@ProductModel", productInfo.ProductBasicInfo.ProductModel.Content);
            dc.SetParameterValue("@MechantSysNo", productInfo.Merchant.SysNo);
            return dc.ExecuteScalar<int>();
        }

        public void UpdateProductBasicInfo(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductBasicInfo");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@BrandSysNo", productInfo.ProductBasicInfo.ProductBrandInfo.SysNo);
            dc.SetParameterValue("@CategorySysNo", productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo);
            dc.SetParameterValue("@ProductName", productInfo.ProductName);
            dc.SetParameterValue("@PromotionTitle", productInfo.PromotionTitle.Content);
            dc.SetParameterValue("@ProductBriefTitle", productInfo.ProductBasicInfo.ProductBriefTitle.Content);
            dc.SetParameterValue("@ProductBriefAddition", productInfo.ProductBasicInfo.ProductBriefAddition.Content);
            dc.SetParameterValue("@ProductConsignFlag", productInfo.ProductConsignFlag);
            dc.SetParameterValue("@TaxNo", productInfo.ProductBasicInfo.TaxNo);
            dc.SetParameterValue("@EntryRecord", productInfo.ProductBasicInfo.EntryRecord);
            dc.SetParameterValue("@TariffPrice", productInfo.ProductBasicInfo.TariffPrice);
            dc.SetParameterValue("@ShoppingGuideURL", productInfo.ProductBasicInfo.ShoppingGuideURL);
            dc.SetParameterValue("@TradeType", productInfo.ProductBasicInfo.TradeType);
            dc.SetParameterValue("@StoreType", productInfo.ProductBasicInfo.StoreType);
            dc.SetParameterValue("@SafeQty", productInfo.ProductBasicInfo.SafeQty);
            dc.ExecuteNonQuery();
        }

        //批量修改商品信息是调用（修改的内容：商品标题（和商品名称）、商品简名、简名附加、商品的描述）
        public void UpdateProductBasicInfoWhenBatchUpdate(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductBasicInfoWhenBatchUpdate");
            dc.SetParameterValue("@ProductTitle", productInfo.ProductBasicInfo.ProductTitle.Content);
            dc.SetParameterValue("@ProductDesc", productInfo.ProductBasicInfo.ShortDescription.Content);
            dc.SetParameterValue("@BriefName", productInfo.ProductBasicInfo.ProductBriefTitle.Content);
            dc.SetParameterValue("@BriefNameAddition", productInfo.ProductBasicInfo.ProductBriefAddition.Content);
            dc.SetParameterValue("@PackageList", productInfo.ProductBasicInfo.PackageList.Content);
            dc.SetParameterValue("@Attention", productInfo.ProductBasicInfo.Attention.Content);
            dc.SetParameterValue("@Note", productInfo.ProductBasicInfo.Note);
            dc.SetParameterValue("@ProductLink", productInfo.ProductBasicInfo.ProductLink);
            dc.SetParameterValue("@BrandSysNo", productInfo.ProductBasicInfo.ProductBrandInfo.SysNo);
            dc.SetParameterValue("@CategorySysNo", productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo);
            dc.SetParameterValue("@TradeType", productInfo.ProductBasicInfo.TradeType);
            dc.SetParameterValue("@StoreType", productInfo.ProductBasicInfo.StoreType);
            dc.SetParameterValue("@SafeQty", productInfo.ProductBasicInfo.SafeQty);
            dc.SetParameterValue("@ProductConsignFlag", productInfo.ProductConsignFlag);
            dc.SetParameterValue("@BMCode", productInfo.ProductBasicInfo.BMCode);
            dc.SetParameterValue("@UPCCode", productInfo.ProductBasicInfo.UPCCode);
            dc.SetParameterValue("@SysNo", productInfo.SysNo);
            dc.ExecuteNonQuery();
        }


        public void UpdateGroupProductBrandInfo(ProductGroup productGroup, BrandInfo brandInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateGroupProductBrandInfo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroup.SysNo);
            dc.SetParameterValue("@BrandSysNo", brandInfo.SysNo);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateGroupProductCategoryInfo(ProductGroup productGroup, CategoryInfo categoryInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateGroupProductCategoryInfo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroup.SysNo);
            dc.SetParameterValue("@CategorySysNo", categoryInfo.SysNo);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public int UpdateProductStatus(int productSysNo, ProductStatus status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductStatus");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.SetParameterValue("@ProductStatus", status);
            int count = dc.ExecuteNonQuery();
            return count;
        }

        public void UpdateProductPurchaseInfo(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductPurchaseInfo");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@MinPackNumber", productInfo.ProductPOInfo.MinPackNumber);
            dc.SetParameterValue("@POMemo", productInfo.ProductPOInfo.POMemo);
            dc.SetParameterValue("@ERPProductID", productInfo.ProductPOInfo.ERPProductID);
            dc.SetParameterValue("@EditUser", productInfo.OperateUser.UserDisplayName);
            dc.SetParameterValue("@EditUserSysNo", productInfo.OperateUser.SysNo);
            dc.SetParameterValue("@InventoryType", (int)productInfo.ProductPOInfo.InventoryType);
            dc.ExecuteNonQuery();
        }

        //public void UpdateProductThirdPartyInventory(ProductInfo productInfo)
        //{
        //    var productMapping = productInfo.ProductMappingList.First();
        //    DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductThirdPartyInventory");
        //    dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
        //    dc.SetParameterValue("@ProductID", productInfo.ProductID);
        //    dc.SetParameterValue("@SynProductID", productMapping.SynProductID);
        //    dc.SetParameterValue("@PartnerType", productMapping.ThirdPartner);
        //    #region 2012-10-30 update Bug:91243
        //    //修改原因:枚举不能转换值，存储错误
        //    //修改类容:下面手动转换

        //    string vfType = string.Empty;
        //    switch (productMapping.StockRules)
        //    {
        //        case StockRules.Limit:
        //            vfType = "L";
        //            break;
        //        case StockRules.Direct:
        //            vfType = "U";
        //            break;
        //        case StockRules.Customer:
        //            vfType = "C";
        //            break;
        //        default:
        //            break;
        //    }
        //    #endregion
        //    dc.SetParameterValue("@VFType", vfType);
        //    dc.SetParameterValue("@LimitVFQty", productMapping.LimitQty);
        //    dc.SetParameterValue("@Status", productMapping.Status);
        //    dc.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
        //    dc.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
        //    dc.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
        //    dc.SetParameterValue("@InUser", productInfo.OperateUser.UserDisplayName);
        //    dc.SetParameterValue("@EditUser", productInfo.OperateUser.UserDisplayName);
        //    dc.ExecuteNonQuery();
        //}

        public void UpdateProductPromotionType(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductPromotionType");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@PromotionType", productInfo.PromotionType);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductOnlineTime(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductOnlineTime");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductInfoFinishStatus(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductInfoFinishStatus");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@ProductInfoFinishStatus", productInfo.ProductBasicInfo.ProductInfoFinishStatus);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductIsNoExtendWarranty(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductIsNoExtendWarranty");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@ProductID", productInfo.ProductID);
            dc.SetParameterValue("@Status", productInfo.ProductWarrantyInfo.IsNoExtendWarranty ? 'A' : 'D');//该存储方式需要重构
            dc.SetParameterValue("@InUser", productInfo.OperateUser.UserDisplayName);
            dc.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
            dc.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductAutoPriceInfo(ProductInfo productInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductAutoPriceInfo");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@AutoAdjustPrice", productInfo.AutoAdjustPrice.IsAutoAdjustPrice);
            dc.SetParameterValue("@AutoPricingStartDate", productInfo.AutoAdjustPrice.NotAutoPricingBeginDate);
            dc.SetParameterValue("@AutoPricingEndDate", productInfo.AutoAdjustPrice.NotAutoPricingEndDate);
            dc.SetParameterValue("@EditUser", productInfo.OperateUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateSourceProductID(string sourceProductID, int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateSourceProductID");
            dc.SetParameterValue("@SourceProductID", sourceProductID);
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        #endregion

        #region 获取商品信息

        public ProductInfo GetProductInfoBySysNo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductInfoBySysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntity<ProductInfo>();
            sourceEntity = GetProductRelationObject(sourceEntity);
            return sourceEntity;
        }

        public ProductInfo GetSimpleProductInfoBySysNo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetSimpleProductInfoBySysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntity<ProductInfo>();
            return sourceEntity;
        }

        public ProductInfo GetProductInfoByID(string productID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductInfoByID");
            dc.SetParameterValue("@ProductID", productID);
            var sourceEntity = dc.ExecuteEntity<ProductInfo>();
            sourceEntity = GetProductRelationObject(sourceEntity);
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品关联信息
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        private ProductInfo GetProductRelationObject(ProductInfo productInfo)
        {
            if (productInfo != null && productInfo.ProductCommonInfoSysNo.HasValue)
            {
                productInfo.ProductPriceInfo.ProductRankPrice = _priceDA.GetProductRankPriceBySysNo(productInfo.SysNo);
                productInfo.ProductPriceInfo.ProductWholeSalePriceInfo = _priceDA.GetWholeSalePriceInfoBySysNo(productInfo.SysNo);
                if (productInfo.ProductPriceRequest.SysNo.HasValue)
                {
                    productInfo.ProductPriceRequest.ProductRankPrice =
                        _priceRequestDA.GetProductRequestRankPriceBySysNo(productInfo.SysNo);
                    productInfo.ProductPriceRequest.ProductWholeSalePriceInfo =
                        _priceRequestDA.GetProductRequestWholeSalePriceInfoBySysNo(productInfo.SysNo);
                }
                productInfo.ProductSalesAreaInfoList = GetProductSalesAreaInfoBySysNo(productInfo.SysNo);
                if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                    productInfo.ProductBasicInfo.ProductAccessories = GetProductAccessoryListByProductSysNo(productInfo.SysNo, productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
                productInfo.ProductBasicInfo.ProductResources =
                    _productResourceDA.GetNeweggProductResourceListByProductCommonInfoSysNo(productInfo.ProductCommonInfoSysNo.Value);
                if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                    productInfo.ProductBasicInfo.ProductProperties =
                        GetProductPropertyListByProductCommonInfoSysNo(productInfo.ProductCommonInfoSysNo.Value, productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
                productInfo.ProductTimelyPromotionTitle = GetProductTimelyPromotionTitle(productInfo.SysNo);
                productInfo.ProductMappingList = GetProductMappingList(productInfo.SysNo);
                productInfo.ProductAttachmentList = _productAttachmentDA.GetProductAttachmentList(productInfo.SysNo);
                productInfo.LastProductPriceRequestInfo = _priceRequestDA.GetProductLastProductPriceRequestInfo(productInfo.SysNo);
                productInfo.ProductBatchManagementInfo = _productExtDA.GetProductBatchManagementInfo(productInfo.SysNo);
            }
            return productInfo;
        }

        /// <summary>
        /// 获取商品配件列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        private IList<ProductAccessory> GetProductAccessoryListByProductSysNo(int productSysNo, int categorySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductAccessoryListByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.SetParameterValue("@CategorySysNo", categorySysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductAccessory>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        private IList<ProductProperty> GetProductPropertyListByProductCommonInfoSysNo(int productCommonInfoSysNo, int categorySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductPropertyListByProductCommonInfoSysNo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            dc.SetParameterValue("@CategorySysNo", categorySysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductProperty>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品销售区域列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private IList<ProductSalesAreaInfo> GetProductSalesAreaInfoBySysNo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductSalesAreaInfoBySysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductSalesAreaInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品时效性促销语
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private IList<ProductTimelyPromotionTitle> GetProductTimelyPromotionTitle(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductTimelyPromotionTitle");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductTimelyPromotionTitle>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品映射列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private IList<ProductMapping> GetProductMappingList(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductMappingList");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductMapping>();
            return sourceEntity;
        }

        #endregion

        #region 获取组内商品信息

        public List<ProductInfo> GetProductListByProductGroupSysNo(int productGroupSysNo)
        {
            return GetProductSysNoListByProductGroupSysNo(productGroupSysNo).Select(GetProductInfoBySysNo).ToList();
        }

        /// <summary>
        /// 获取组内商品ProductSysNo列表
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        public IEnumerable<int> GetProductSysNoListByProductGroupSysNo(int productGroupSysNo)
        {
            var list = new List<int>();
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductSysNoListByProductGroupSysNo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            using (IDataReader reader = dc.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.GetInt32(0));
                }
            }
            return list;
        }

        #endregion

        #region 处理商品关联信息

        public void InsertProductAccessory(int productSysNo, ProductAccessory productAccessory)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductAccessory");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.SetParameterValue("@AccessoriesSysno", productAccessory.AccessoryInfo.SysNo);
            dc.SetParameterValue("@Status", productAccessory.Status);
            dc.SetParameterValue("@Qty", productAccessory.Qty);
            dc.SetParameterValue("@Description", productAccessory.Description.Content);
            dc.SetParameterValue("@ListOrder", productAccessory.Priority);
            dc.SetParameterValue("@InUserSysNo", productAccessory.OperationUser.SysNo);
            dc.SetParameterValue("@CompanyCode", productAccessory.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productAccessory.LanguageCode);
            dc.ExecuteNonQuery();
        }

        public void DeleteProductAccessory(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductAccessory");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        public void InsertProductSalesArea(ProductInfo productInfo, ProductSalesAreaInfo productSalesAreaInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductSalesArea");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@ProductID", productInfo.ProductID);
            dc.SetParameterValue("@ProductName", productInfo.ProductBasicInfo.ProductTitle.Content);
            dc.SetParameterValue("@StockSysNo", productSalesAreaInfo.Stock.SysNo);
            dc.SetParameterValue("@StockName", productSalesAreaInfo.Stock.StockName);
            dc.SetParameterValue("@ProvinceSysNo", productSalesAreaInfo.Province.ProvinceSysNo);
            dc.SetParameterValue("@ProvinceName", productSalesAreaInfo.Province.ProvinceName);
            dc.SetParameterValue("@CitySysNo", productSalesAreaInfo.Province.CitySysNo);
            dc.SetParameterValue("@CityName", productSalesAreaInfo.Province.CityName);
            dc.SetParameterValue("@InUser", productSalesAreaInfo.OperationUser.UserDisplayName);
            dc.SetParameterValue("@CompanyCode", productSalesAreaInfo.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productSalesAreaInfo.LanguageCode);
            dc.ExecuteNonQuery();
        }

        public void DeleteProductSalesArea(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductSalesArea");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        public void InsertProductTimelyPromotionTitle(int productSysNo, ProductInfo productInfo)
        {
            productInfo.ProductTimelyPromotionTitle.ForEach(delegate(ProductTimelyPromotionTitle promotionTitle)
                                                 {
                                                     DataCommand dc = DataCommandManager.GetDataCommand("InsertProductTimelyPromotionTitle");
                                                     dc.SetParameterValue("@ProductSysNo", productSysNo);
                                                     dc.SetParameterValue("@PromotionTitle", promotionTitle.PromotionTitle.Content);
                                                     dc.SetParameterValue("@Type", promotionTitle.PromotionTitleType);
                                                     dc.SetParameterValue("@BeginDate", promotionTitle.BeginDate);
                                                     dc.SetParameterValue("@EndDate", promotionTitle.EndDate);
                                                     dc.SetParameterValue("@Status", promotionTitle.Status);
                                                     dc.SetParameterValue("@InUser", productInfo.OperateUser.UserDisplayName);
                                                     dc.SetParameterValue("@CompanyCode", productInfo.CompanyCode);
                                                     dc.SetParameterValue("@LanguageCode", productInfo.LanguageCode);
                                                     dc.SetParameterValue("@StoreCompanyCode", productInfo.CompanyCode);
                                                     dc.ExecuteNonQuery();
                                                 });
        }

        public void DeleteProductTimelyPromotionTitle(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductTimelyPromotionTitle");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }



        #endregion


        /// <summary>
        /// 获取商品克隆次数
        /// </summary>
        /// <param name="productID">商品ID</param>
        /// <param name="sixthLetter">克隆类型</param>
        /// <returns>商品次数</returns>
        public int GetCloneCount(string productID, string sixthLetter)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCloneCount");
            dc.SetParameterValue("@ProductID", productID);
            dc.SetParameterValue("@SixthLetter", sixthLetter);
            return (int)dc.ExecuteScalar();
        }

        /// <summary>
        /// 根据商品ID查询商品列表，用于查询返修品，ProductID类似 xxxxR（目前仅RMA调用）
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetSimpleProductListByID(string productID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetSimpleProductListByID");
            dc.SetParameterValue("@ProductID", productID);
            var sourceEntity = dc.ExecuteEntityList<ProductInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        public int GetProductGroupSysNo(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IM_Product_GetProductGroupSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);

            return dc.ExecuteScalar<int>();
        }

        /// <summary>
        /// 根据商品编号列表 获取商品关税信息列表
        /// </summary>
        /// <param name="sysNos">商品编号列表</param>
        /// <returns></returns>
        public DataTable GetProductTariffInfoProductSysNos(List<int> sysNos)
        {
            CustomDataCommand dc = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductTariffInfoByProductSysNos");

            dc.CommandText = dc.CommandText.Replace("#SysNos#", String.Join(",", sysNos));

            return dc.ExecuteDataTable();
        }

        /// <summary>
        /// 获取同系商品ID前缀最大的商品ID
        /// </summary>
        /// <param name="code">商品ID前缀，如：xx_xxx_xxx</param>
        /// <returns></returns>
        public string GetProductSameIDMaxProductID(string code)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IM_Product_GetProductSameIDMaxProductID");
            cmd.SetParameterValue("@Code", code);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        /// <summary>
        /// 插入商品产地信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="origin">产地</param>
        public void InsertProductEntryInfo(int productSysNo, string origin)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IM_Product_InsertProductEntryInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Origin", origin);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        public List<ProductCountry> GetProductCountryList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IM_Product_GetProductCountryList");
            return cmd.ExecuteEntityList<ProductCountry>();
        }

        /// <summary>
        /// 更新商品采购价格
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="virtualPrice">采购价格</param>
        public void UpdateProductVirtualPrice(string productSysNo, string virtualPrice)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IM_Product_UpdateProductVirtualPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@VirtualPrice", virtualPrice);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 商品审核
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <param name="status"></param>
        public void BatchAuditProduct(List<int> productSysNos, ProductStatus status)
        {
            StringBuilder productSysNoXml = new StringBuilder();
            productSysNoXml.Append("<SysNos>");
            foreach (var sysNo in productSysNos)
            {
                productSysNoXml.Append(string.Format("<SysNo>{0}</SysNo>", sysNo));
            }
            productSysNoXml.Append("</SysNos>");
            DataCommand dc = DataCommandManager.GetDataCommand("BatchAuditProduct");
            dc.SetParameterValue("@ProductSysNoXml", productSysNoXml.ToString());
            dc.SetParameterValue("@ProductStatus", (int)status);
            dc.ExecuteNonQuery();
        }
    }
}
