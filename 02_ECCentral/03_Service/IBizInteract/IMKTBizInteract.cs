/*********************************************************************************************
// Copyright (c) 2012, Newegg (Chengdu) Co., Ltd. All rights reserved.
// Created by Jin.J.Qin at 3/23/2012   
// Target Framework : 4.0
// Class Name : IMKTBizInteract
// Description : MKT Tools Domain向其它Domain提供Biz服务的接口//
//*********************************************************************************************/

using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.IBizInteract
{
    public interface IMKTBizInteract
    {

        /// <summary>
        /// 判断商品在赠品活动中是否作为赠品存在，但是要排除已“作废”“完成”“中止”状态的记录的赠品活动
        /// 商品上架时需要调用
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool ProductIsGift(int productSysNo);         

        /// <summary>
        /// 提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno
        /// 然后检查商品对应捆绑规则是否有低于成本价的情况(价格和+折扣 《 成本价格和)，有的就将其变为待审核(status=1),并发送记录LOG,邮件通知
        /// </summary>
        /// <param name="productSysNo"></param>
        void CheckComboPriceAndSetStatus(int productSysNo);

        /// <summary>
        ///判断当前商品是否存在正在生效的促销活动中，促销活动：团购，限时促销，以及作为赠品的赠品活动(验证商品是否可以调价等需要调用的接口。例如是赠品或团购以及限时抢购不能调价。条件：赠品-本身作为赠品；生效状态-只有运行性中的)
        ///返回值为True表示存在运行中的这几种促销活动
        /// </summary>
        /// <param name="productSysNo"></param>
        bool CheckMarketIsActivity(int productSysNo);

        /// <summary>
        /// 根据优惠券编号检查优惠券是否有效，不管优惠券是否有效都要返回优惠券代码.
        /// </summary>
        /// <param name="couponSysNo">优惠券编号</param>
        /// <param name="couponCode">优惠券代码</param>
        /// <returns></returns>
        bool CheckCouponIsValid(int couponSysNo, out string couponCode);

        /// <summary>
        /// 根据订单信息计算所有促销活动的优惠信息，目前包括：Combo，Coupons
        /// [Jin]已确认， -- OK
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns>订单参与的促销记录</returns>
        List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo);

        /// <summary>
        /// 根据订单信息计算所有促销活动的优惠信息，目前包括：Combo，Coupons
        /// [Jin]已确认， -- OK
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="isModifyCoupons">是否是修改了优惠券</param>
        /// <returns>订单参与的促销记录</returns>
        List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, bool isModifyCoupons);

        /// <summary>
        /// 优惠券应用
        /// [Jin]已确认， -- OK
        /// </summary>
        /// <param name="couponSysNo">优惠券活动系统编号</param>
        /// <param name="couponCode">优惠券代码</param>
        /// <param name="customerSysNo">客户系统编号</param>
        /// <param name="shoppingCartSysNo">购物车系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="redeemAmount">折扣总金额</param>
        void CouponCodeApply(int couponSysNo, string couponCode, int customerSysNo, 
            int shoppingCartSysNo, int soSysNo, decimal redeemAmount);

        /// <summary>
        /// 订单取消使用优惠券 ，使用场景：订单作废.
        /// IPP3的现实请作为参考：IActionCouponInfoV33.CancelCouponCode(CouponInfoV33 coupon);
        /// [Jin]已确认
        /// </summary>
        /// <param name="couponCode">优惠券代码</param>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="shoppingCartSysNo"></param>        
        void CouponCodeCancel(string couponCode, int soSysNo, int shoppingCartSysNo);
 

        /// <summary>
        /// 根据组合系统编号列表获取详细信息列表
        /// [Jin]已确认 --OK
        /// </summary>
        /// <param name="comboSysNoList"></param>
        /// <returns></returns>
        List<ComboInfo> GetComboList(List<int> comboSysNoList);

        /// <summary>
        /// 获取订单产生的优惠卷信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>优惠卷Log信息</returns>
        PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo);

 
        /// <summary>
        /// 获取就绪/运行的限时促销计划列表
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns>就绪/运行的限时促销计划列表</returns>
        List<CountdownInfo> GetReadyOrRunningCountDownByProductSysNo(int productSysNo);

        /// <summary>
        /// 根据商品编号获取正在参加团购的商品编号
        /// </summary>
        /// <param name="products">待验证的商品编号</param>
        /// <returns>正在参加团购的商品编号</returns>
        List<int> GetProductsOnGroupBuying(IEnumerable<int> products);

        /// <summary>
        /// 根据商品编号获取商品促销折扣信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductPromotionDiscountInfo> GetProductPromotionDiscountInfoList(int productSysNo);

        void GetGiftSNAndCouponSNByProductSysNo(int productsysno, out int giftsysno, out int couponsysno);

        #region Job 相关
        /// <summary>
        /// 检查订单是否满足赠送优惠券条件，如果满足则赠送优惠券给订单用户
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        bool CheckAndGivingPromotionCodeForSO(SOInfo soInfo);
        /// <summary>
        /// 取得团购编号取得团购信息
        /// </summary>
        /// <param name="sysNo"> 团购编号</param>
        /// <returns></returns>
        GroupBuyingInfo GetGroupBuyInfoBySysNo(int sysNo);
        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        List<GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode);
        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        void UpdateGroupBuySettlementStatus(int sysNo, GroupBuyingSettlementStatus settlementStatus);
        #endregion
    }
}
