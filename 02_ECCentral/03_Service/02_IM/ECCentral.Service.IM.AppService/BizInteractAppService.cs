using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(IIMBizInteract))]
    public class BizInteractAppService : IIMBizInteract
    {
        public ProductInfo GetProductInfo(int productSysNo)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetProductInfo(productSysNo);
        }

        public ProductInfo GetProductInfo(string productID)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetProductInfoByID(productID);
        }
        public List<ProductInfo> GetSimpleProductList(string productID)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetSimpleProductListByID(productID);
        }
        public List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            var productInfoList = new List<ProductInfo>();
            productInfoList.AddRange(productSysNoList.Select(productSysNo => ObjectFactory<ProductProcessor>.Instance.GetProductInfo(productSysNo)).Where(productInfo => productInfo != null));
            return productInfoList;
        }

        public ProductInfo GetSimpleProductInfo(int productSysNo)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetSimpleProductInfo(productSysNo);
        }

        public ProductGroup GetProductsInSameGroupWithProductSysNo(int productSysNo)
        {
            return ObjectFactory<ProductGroupProcessor>.Instance.GetProductGroup(productSysNo);
        }

        public BrandInfo GetBrandInfoBySysNo(int sysNo)
        {
            return ObjectFactory<BrandProcessor>.Instance.GetBrandInfoBySysNo(sysNo);
        }

        public List<BrandInfo> GetBrandInfoList()
        {
            return ObjectFactory<BrandProcessor>.Instance.GetBrandInfoList();
        }


        public CategoryInfo GetCategory1Info(int c1SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory1BySysNo(c1SysNo);
        }
        public CategoryInfo GetCategory2Info(int c2SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory2BySysNo(c2SysNo);
        }
        public CategoryInfo GetCategory3Info(int c3SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory3BySysNo(c3SysNo);
        }

        public CategorySetting GetCategorySetting(int categorySysNo)
        {
            return ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingBySysNo(categorySysNo);
        }

        public ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo)
        {
            return ObjectFactory<ProductManagerGroupProcessor>.Instance.GetPMListByUserSysNo(userSysNo);
        }

        public List<ProductManagerInfo> GetPMListByType(BizEntity.Common.PMQueryType queryType, string currentUserName, string companyCode)
        {
            return ObjectFactory<ProductManagerProcessor>.Instance.GetPMList(queryType, currentUserName, companyCode);
        }

        public void UpdateProductPromotionType(int productSysNo, string promotionType)
        {
            if (String.IsNullOrEmpty(promotionType) || promotionType == "DC" || promotionType == "GB")
            {
                var productInfo = new ProductInfo { SysNo = productSysNo, PromotionType = promotionType };
                ObjectFactory<ProductProcessor>.Instance.UpdateProductPromotionType(productInfo);
            }
            else
            {
                throw new BizException("PromotionType Arg Error");
            }
        }

        public void RollBackPriceWhenCountdownInterrupted(CountdownInfo countdownInfo)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.RollBackPriceWhenCountdownInterrupted(countdownInfo);
        }

        public void UpdateProductVirtualPrice(int productSysNo, decimal virtualPrice)
        {
            //Todo:Need Implements
            throw new NotImplementedException();
        }

        public void UpdateProductNotAutoSetVirtual(int productSysNo, int status, string note)
        {
            throw new NotImplementedException();
        }

        public bool CheckIsAttachment(int productSysNo)
        {
            //Todo:Need Implements
            return ObjectFactory<ProductAttachmentProcessor>.Instance.CheckIsAttachment(productSysNo); ;
        }

        public ProductDomain GetDomainByProductID(string productID)
        {
            //Todo:Need Implements
            return new ProductDomain { ProductDomainName = new LanguageContent("NewProductDomain") };
        }

        public PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime)
        {
            return ObjectFactory<ProductPriceProcessor>.Instance.GetPriceChangeLogInfoByProductSysNo(productSysNo, startTime, endTime);
        }

        #region GiftCard

        public string GiftCardRMARefund(List<GiftCard> cardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardRMARefund(cardList, companyCode);
        }

        public string CancelUsedGiftCard(int soSysNo, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.CancelUsedGiftCard(soSysNo, companyCode);
        }

        public string MandatoryVoidGiftCard(List<string> cardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.MandatoryVoidGiftCard(cardList, companyCode);
        }

        public string GiftCardSplitSO(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardSplitSO(mainSOSysNo, customerSysNo, cardList, companyCode);
        }

        public string GiftCardSplitSORollback(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardSplitSORollback(mainSOSysNo, customerSysNo, cardList, companyCode);
        }

        public string CreateElectronicGiftCard(int soSysNo, int customerSysNo, int quantity, decimal cashAmt, GiftCardType internalType, string source, string memo, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.CreateElectronicGiftCard(soSysNo, customerSysNo, quantity, cashAmt, internalType, source, memo, companyCode);
        }

        public GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardInfoByReferenceSOSysNo(soSysNo, customerSysNo, internalType, type);
        }

        public List<GiftCardInfo> GetGiftCardsByCodeList(List<string> codeList)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardsByCodeList(codeList);
        }

        public List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardInfoBySOSysNo(soSysNo, internalType);
        }

        public List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardRedeemLog(actionSysNo, actionType);
        }

        /// <summary>
        /// 礼品卡扣减接口(改单）
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public string GiftCardDeduction(List<GiftCard> usedGiftCardList, string companyCode)
        {
            return "ObjectFactory<GiftCardProcessor>.Instance.GiftCardDeduction(usedGiftCardList, companyCode)";
        }

        /// <summary>
        /// 礼品卡使用接口(订单创建）
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public string GiftCardConsume(List<GiftCard> usedGiftCardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardConsume(usedGiftCardList, companyCode);
        }

        /// <summary>
        /// 礼品卡使用接口(创建订单使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        public string GiftCardConsumeForSOCreate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardConsumeForSOCreate(giftCardPay, usedGiftCardList, companyCode);
        }

        /// <summary>
        /// 礼品卡使用接口(更新订单使用 更新更新礼品卡使用方式为 先作废礼品卡使用  在创建礼品卡使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        public string GiftCardVoidForSOUpdate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GiftCardVoidForSOUpdate(giftCardPay, usedGiftCardList, companyCode);
        }

        #endregion

        #region Obsolete

        public string GetCategoryC2AndC3NameBySysNo(int? c2SysNo, int? c3SysNo)
        {
            throw new NotImplementedException();
        }

        public void ManageCountrySideProduct(int countrySideProductID, int orderSysNo, List<int> productSysNoList)
        {
            throw new NotImplementedException();
        }

        public ProductPriceInfo UpdateProductPriceForOther(ProductPriceInfo entity)
        {
            throw new NotImplementedException();
        }

        public List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            return ObjectFactory<CategoryPropertyProcessor>.Instance.GetCategoryPropertyByCategorySysNo(categorySysNo).ToList();
        }

        #endregion

        public Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList)
        {
            return ObjectFactory<PropertyProcessor>.Instance.GetPropertyValueInfoByPropertySysNoList(sysNoList);
        }

        public List<ManufacturerInfo> GetManufacturerList(string companyCode)
        {
            return ObjectFactory<ManufacturerProcessor>.Instance.GetAllManufacturer(companyCode);
        }

        public decimal GetProductMargin(decimal currentPrice, int point, decimal unitCost, decimal discount)
        {
            return ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(currentPrice, point, unitCost, discount);
        }

        public decimal GetGiftProductMargin(decimal currentPrice, int point, decimal unitCost, decimal discount)
        {
            return ObjectFactory<ProductPriceProcessor>.Instance.GetGiftMargin(currentPrice, point, unitCost, discount);
        }

        public decimal GetProductMarginAmount(decimal currentPrice, int point, decimal unitCost)
        {
            return ObjectFactory<ProductPriceProcessor>.Instance.GetMarginAmount(currentPrice, point, unitCost);
        }

        public List<ProductPromotionMarginInfo> GetProductPromotionMargin(ProductPriceRequestInfo productPriceReqInfo,
                                                                            int productSysNo,
                                                                            string priceName,
                                                                            decimal discount,
                                                                            ref string productMarginReturnMsg)
        {
            //Check各类型（PromotionType）折扣的最大值
            //若不符合毛利率规则，则返回折扣率对象
            return ObjectFactory<ProductPriceProcessor>.Instance.CheckMargin(
                                productPriceReqInfo, productSysNo, priceName, discount, ref productMarginReturnMsg);
        }


        /// <summary>
        /// 修改商品关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="keywords"></param>
        /// <param name="keywords0"></param>
        /// <param name="editUserSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public bool UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0, int editUserSysNo, string companyCode)
        {
            return ObjectFactory<ProductExtProcessor>.Instance.UpdateProductExKeyKeywords(sysNo, keywords, keywords0, editUserSysNo, companyCode);
        }
        /// <summary>
        /// 根据C3SysNo得到C1CategoryInfo
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public CategoryInfo GetC1ByC3(int c3SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory1ByCategory3SysNo(c3SysNo);
        }



        /// <summary>
        /// 修改联通合约机号码状态
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <param name="operationUser"></param>
        public void UpdateUnicomContractPhoneNumberStatus(string phone, UnicomContractPhoneNumberStatus status, UserInfo operationUser)
        {
            ObjectFactory<UnicomContractPhoneProcessor>.Instance.UpdateUnicomContractPhoneNumberStatus(phone, status, operationUser);
        }

        #region IIMBizInteract Members


        public List<ProductRMAPolicyInfo> GetProductRMAPolicyList(List<int> productSysNos)
        {
            return ObjectFactory<ProductRMAPolicyProcessor>.Instance.GetProductRMAPolicyList(productSysNos);
        }

        #endregion


        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<string> GetProductGroupIDSFromProductID(string productID)
        {
            return ObjectFactory<ProductGroupProcessor>.Instance.GetProductGroupIDSFromProductID(productID);
        }

        public virtual void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.UpdateProductVirtualPrice(productSysNo, originalVirtualPrice, newVirtualPrice);
        }

        public virtual void UpdateProductBasicPrice(int productSysNo, decimal newPrice)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.UpdateProductBasicPrice(productSysNo, newPrice);
        }

        public virtual void UpdateProductCurrentPrice(int productSysNo, decimal newPrice)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.UpdateProductCurrentPrice(productSysNo, newPrice);
        }

        /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        public int GetProductGroupSysNo(int productSysNo)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetProductGroupSysNo(productSysNo);
        }

        /// <summary>
        /// 计算商品关税
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns>List&lt;商品编号,关税&gt;</returns>
        public List<KeyValuePair<int, decimal>> GetProductTax(List<int> productSysNoList)
        {
            return ObjectFactory<ProductProcessor>.Instance.GetProductTax(productSysNoList);
        }


        public List<ProductEntryInfo> GetProductEntryInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IProductEntryInfoDA>.Instance.GetProductEntryInfoList(productSysNoList);
        }

        public List<CategoryInfo> GetCategory1List()
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory1List();
        }

        #region 申报商品
        /// <summary>
        /// 申报时，获取不同状态下的商品信息
        /// </summary>
        /// <param name="entryStatus">商品备案状态</param>
        /// <param name="entryStatusEx">商品备案扩展状态</param>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetProduct(ProductEntryStatus entryStatus, ProductEntryStatusEx? entryStatusEx)
        {
            return ObjectFactory<IProductEntryInfoDA>.Instance.GetProduct(entryStatus, entryStatusEx);
        }
        /// <summary>
        /// 商品批量备案操作
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <param name="Note"></param>
        /// <param name="entryStatus"></param>
        /// <param name="entryStatusEx"></param>
        public void ProductBatchEntry(List<int> productSysNoList, string Note, ProductEntryStatus entryStatus, ProductEntryStatusEx entryStatusEx)
        {
            ObjectFactory<ProductAppService>.Instance.ProductBatchEntry(productSysNoList, Note, entryStatus, entryStatusEx);
        }
        /// <summary>
        /// 申报时获取申报商品详细信息
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public List<ProductDeclare> DeclareGetProduct(List<WaitDeclareProduct> products)
        {
            return ObjectFactory<IProductEntryInfoDA>.Instance.DeclareGetProduct(products);
        }
        /// <summary>
        /// 申报商品成功（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        public bool ProductCustomsSuccess(ProductEntryInfo entryInfo)
        {
            return ObjectFactory<IProductEntryInfoDA>.Instance.ProductCustomsSuccess(entryInfo);
        }
        /// <summary>
        /// 申报商品失败（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        public bool ProductCustomsFail(ProductEntryInfo entryInfo)
        {
            return ObjectFactory<IProductEntryInfoDA>.Instance.ProductCustomsFail(entryInfo);
        }
        #endregion
    }
}
