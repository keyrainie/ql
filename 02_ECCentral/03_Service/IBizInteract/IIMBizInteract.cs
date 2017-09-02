 /*********************************************************************************************
// Copyright (c) 2012, Newegg (ShangHai) Co., Ltd. All rights reserved.
// Created by Howard.L.Wei at 3/27/2012   
// Target Framework : 4.0
// Class Name : IIMBizInteract
// Description : IM Domain向其它Domain提供Biz服务的接口//
//*********************************************************************************************/

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using System.Data;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.IBizInteract
{
    public interface IIMBizInteract
    {
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductInfo GetProductInfo(int productSysNo);

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductInfo GetProductInfo(string productID);

        /// <summary>
        /// 获取商品简单信息列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<ProductInfo> GetSimpleProductList(string productID);

        /// <summary>
        /// 获取商品简单信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductInfo GetSimpleProductInfo(int productSysNo);

        /// <summary>
        /// 获取商品信息列表
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList);

        /// <summary>
        /// 返回一个商品所属分组中所有
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns>返回商品组信息（包含组内所有商品、分组属性）</returns>
        ProductGroup GetProductsInSameGroupWithProductSysNo(int productSysNo);

        /// <summary>
        /// 获取Brand信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        BrandInfo GetBrandInfoBySysNo(int sysNo);

        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <returns></returns>
        List<BrandInfo> GetBrandInfoList();

        /// <summary>
        /// 获取3级类别信息
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        CategoryInfo GetCategory3Info(int c3SysNo);

        /// <summary>
        /// 获取2级类别信息
        /// </summary>
        /// <param name="c2SysNo"></param>
        /// <returns></returns>
        CategoryInfo GetCategory2Info(int c2SysNo);

        CategoryInfo GetCategory1Info(int c1SysNo);

        /// <summary>
        /// 获取Category详细设置属性，包含给予类别的配件、毛利率、相关指标等
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        CategorySetting GetCategorySetting(int categorySysNo);

        /// <summary>
        /// 根据userSysNo获取PM组信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo);

        /// <summary>
        /// 根据不同权限获取PMList:
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="currentUserName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<ProductManagerInfo> GetPMListByType(PMQueryType queryType, string currentUserName, string companyCode);

        /// <summary>
        /// 更新商品的PromotionType，调用方MKT
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="promotionType"></param>
        void UpdateProductPromotionType(int productSysNo, string promotionType);

        /// <summary>
        /// 当限时抢购和促销计划被终止的时候，将产品的价格恢复到活动开始时价格
        /// </summary>
        /// <param name="countdownInfo"></param>
        void RollBackPriceWhenCountdownInterrupted(CountdownInfo countdownInfo);

        /// <summary>
        /// 更新商品正常采购价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="virtualPrice"></param>
        void UpdateProductVirtualPrice(int productSysNo, decimal virtualPrice);

        void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice);

        void UpdateProductBasicPrice(int productSysNo, decimal newPrice);

        void UpdateProductCurrentPrice(int productSysNo, decimal newPrice);

        /// <summary>
        /// 将[IPP3].[dbo].[Product_NotAutoSetVirtual]表设置的禁止设置虚库记录作废
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="status"></param>
        /// <param name="note"></param>
        void UpdateProductNotAutoSetVirtual(int productSysNo, int status, string note);

        /// <summary>
        /// 判断一个商品是否是附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool CheckIsAttachment(int productSysNo);

        /// <summary>
        /// 根据商品获取Domain信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductDomain GetDomainByProductID(string productID);

        /// <summary>
        /// 获取某个时间段内的某个商品的价格变动日志
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime);

        #region GiftCard

        /// <summary>
        /// 退款到原礼品卡,在内部组装sp所需的xml message
        /// </summary>
        /// <param name="cardList">待退礼品卡列表</param>
        /// <returns>操作结果状态码</returns>
        string GiftCardRMARefund(List<GiftCard> cardList, string companyCode);

        /// <summary>
        /// 作废订单的时候，将SO使用的GiftCard金额还原
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>操作结果状态码</returns>
        string CancelUsedGiftCard(int soSysNo, string companyCode);

        /// <summary>
        /// 强制作废礼品卡,在内部组装sp所需的xml message
        /// </summary>
        /// <param name="cardList">礼品卡号列表</param>
        /// <returns></returns>
        string MandatoryVoidGiftCard(List<string> cardList, string companyCode);

        string GiftCardSplitSO(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode);

        string GiftCardSplitSORollback(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode);

        /// <summary>
        /// 创建电子礼品卡,在内部组装sp所需的xml message
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="cashAmt"></param>
        /// <param name="memo">金额</param>
        /// <returns>操作结果状态码</returns>
        string CreateElectronicGiftCard(int soSysNo, int customerSysNo, int quantity, decimal cashAmt, GiftCardType internalType, string source, string memo, string companyCode);

        /// <summary>
        /// 根据订单号，internalType，客户编号，类型查询礼品卡信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="internalType">卡类型</param>
        /// <param name="type">卡的材质类型</param>
        /// <returns>礼品卡信息</returns>
        GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type);

        /// <summary>
        /// 根据礼品卡Code列表获取礼品卡信息
        /// </summary>
        /// <param name="codeList"></param>
        /// <returns></returns>
        List<GiftCardInfo> GetGiftCardsByCodeList(List<string> codeList);

        /// <summary>
        /// 根据订单号和internalType查询礼品卡信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="internalType">卡类型</param>
        /// <returns>礼品列表</returns>
        List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType);

        /// <summary>
        /// 获取礼品卡使用记录
        /// </summary>
        /// <param name="actionSysNo">行为系统编号</param>
        /// <param name="actionType">行为类型</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType);

        /// <summary>
        /// 礼品卡扣减接口(改单）
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        string GiftCardDeduction(List<GiftCard> usedGiftCardList, string companyCode);

        /// <summary>
        /// 礼品卡使用接口(创建订单使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        string GiftCardConsumeForSOCreate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode);

        /// <summary>
        /// 礼品卡使用接口(更新订单使用 更新更新礼品卡使用方式为 先作废礼品卡使用  在创建礼品卡使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        string GiftCardVoidForSOUpdate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode);

        #endregion

        #region Obsolete

        [Obsolete]
        string GetCategoryC2AndC3NameBySysNo(int? c2SysNo, int? c3SysNo);

        [Obsolete]
        void ManageCountrySideProduct(int countrySideProductID, int orderSysNo, List<int> productSysNoList);

        [Obsolete]
        ProductPriceInfo UpdateProductPriceForOther(ProductPriceInfo entity);

        [Obsolete]
        List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo);

        #endregion

        /// <summary>
        /// 根据属性编号集合获取属性集合
        /// </summary>
        /// <param name="sysNoList">属性编号集合</param>
        /// <returns>属性集合</returns>
        Dictionary<int, List<PropertyValueInfo>> GetPropertyValueInfoByPropertySysNoList(List<int> sysNoList);

        /// <summary>
        /// 获取生产商列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<ManufacturerInfo> GetManufacturerList(string companyCode);

        /// <summary>
        /// 获取商品毛利率
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="point"></param>
        /// <param name="unitCost"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        decimal GetProductMargin(decimal currentPrice, int point, decimal unitCost, decimal discount);

        /// <summary>
        /// 获取赠品毛利率
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="point"></param>
        /// <param name="unitCost"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        decimal GetGiftProductMargin(decimal currentPrice, int point, decimal unitCost, decimal discount);

        /// <summary>
        /// 获取商品毛利
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="point"></param>
        /// <param name="unitCost"></param>
        /// <returns></returns>
        decimal GetProductMarginAmount(decimal currentPrice, int point, decimal unitCost);

        /// <summary>
        /// 在商品价格信息（CurrentPrice、Point、UnitCost）更新时，
        /// 返回再各个促销类型（PromotionType）不符合毛利率规则的折扣对象，若都符合则返回列表为空
        /// 返回值中将保存毛利率
        /// </summary>
        /// <param name="discountList">折扣对象列表</param>
        /// <returns></returns>
        List<ProductPromotionMarginInfo> GetProductPromotionMargin(ProductPriceRequestInfo productPriceReqInfo,
                                                                            int productSysNo,
                                                                            string priceName,
                                                                            decimal discount,
                                                                            ref string productMarginReturnMsg);
        /// <summary>
        /// 修改产品扩展信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Keywords"></param>
        /// <param name="Keywords0"></param>
        /// <param name="EditUser"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        bool UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0, int editUserSysNo, string companyCode);

        /// <summary>
        /// 根据C3获取C1
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        CategoryInfo GetC1ByC3(int c3SysNo);

        /// <summary>
        /// 修改联通合约机号码状态
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <param name="operationUser"></param>
        void UpdateUnicomContractPhoneNumberStatus(string phone, UnicomContractPhoneNumberStatus status, UserInfo operationUser);

        #region 商品退换货管理对外提供的借口

        /// <summary>
        /// 得到商品的退换货信息
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <returns></returns>
        List<ProductRMAPolicyInfo> GetProductRMAPolicyList(List<int> productSysNos);
        #endregion



        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<string> GetProductGroupIDSFromProductID(string productID);

        /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        int GetProductGroupSysNo(int productSysNo);

        /// <summary>
        /// 计算商品关税
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns>List&lt;商品编号,关税&gt;</returns>
        List<KeyValuePair<int, decimal>> GetProductTax(List<int> productSysNoList);

       /// <summary>
        /// 取得商品入境设置
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        List<ProductEntryInfo> GetProductEntryInfoListByProductSysNoList(List<int> productSysNoList);

        /// <summary>
        /// 获取所有有效的C1
        /// </summary>
        /// <returns></returns>
        List<CategoryInfo> GetCategory1List();

        #region 申报商品
        /// <summary>
        /// 申报时，获取不同状态下的商品信息
        /// </summary>
        /// <param name="entryStatus">商品备案状态</param>
        /// <param name="entryStatusEx">商品备案扩展状态</param>
        /// <returns></returns>
        List<WaitDeclareProduct> GetProduct(ProductEntryStatus entryStatus, ProductEntryStatusEx? entryStatusEx);
        /// <summary>
        /// 商品批量备案操作
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <param name="Note"></param>
        /// <param name="entryStatus"></param>
        /// <param name="entryStatusEx"></param>
        void ProductBatchEntry(List<int> productSysNoList, string Note, ProductEntryStatus entryStatus, ProductEntryStatusEx entryStatusEx);
        /// <summary>
        /// 申报时获取申报商品详细信息
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        List<ProductDeclare> DeclareGetProduct(List<WaitDeclareProduct> products);
        /// <summary>
        /// 申报商品成功（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        bool ProductCustomsSuccess(ProductEntryInfo entryInfo);
        /// <summary>
        /// 申报商品失败（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        bool ProductCustomsFail(ProductEntryInfo entryInfo);
        #endregion
    }
}
