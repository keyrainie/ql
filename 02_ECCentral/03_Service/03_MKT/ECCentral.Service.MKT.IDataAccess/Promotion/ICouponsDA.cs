using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.Promotion
{
    public interface ICouponsDA
    {
        #region  全局行为

        /// <summary>
        /// 获取优惠券所有信息，但是不Load优惠券代码信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CouponsInfo Load(int? sysNo);

        /// <summary>
        /// 修改优惠券活动状态，包括：提交审核，撤销审核，作废，提前中止
        /// </summary>
        /// <param name="couponSysNo">系统编号</param>
        /// <param name="status">Update状态</param>
        /// <param name="userName">用户全名</param>
        void UpdateStatus(int couponSysNo, CouponsStatus status, string userName);

        /// <summary>
        /// 审核优惠券活动，包括：审核通过，审核拒绝
        /// </summary>
        /// <param name="couponSysNo">系统编号</param>
        /// <param name="status">Update状态</param>
        /// <param name="userName">用户全名</param>
        void Audit(int couponSysNo, CouponsStatus status, string userName);

        bool CheckCouponCodeIsHave(int? couponSysNo);

        #endregion

        #region 局部对象处理
        /// <summary>
        /// Load 活动基本信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CouponsInfo LoadMaster(int? sysNo);

        /// <summary>
        /// 根据优惠券SysNo 获取对应优惠券的折扣活动信息
        /// </summary>
        /// <param name="sysNo">优惠券系统SysNo</param>
        /// <returns></returns>
        CouponsInfo GetCouponsInfoByCouponCodeSysNo(int couponCodeSysNo);

        /// <summary>
        /// 创建优惠券主信息，因为优惠券是分步进行创建的
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        int? CreateMaster(CouponsInfo info);

        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        void UpdateMaster(CouponsInfo info);
        
        /// <summary>
        /// 设置商品范围
        /// </summary>
        /// <param name="info"></param>
        void SetProductCondition(CouponsInfo info);

        /// <summary>
        /// 设置客户范围
        /// </summary>
        /// <param name="info"></param>
        void SetCustomerCondition(CouponsInfo info);

        /// <summary>
        /// 设置优惠券规则:设置订单条件，使用频率条件和每单折扣上限
        /// </summary>
        /// <param name="info"></param>
        void SetSaleRuleEx(CouponsInfo info);

        /// <summary>
        /// 设置折扣规则
        /// </summary>
        /// <param name="info"></param>
        void SetAmountDiscountRule(CouponsInfo info);

        /// <summary>
        /// 设置客户提醒规则
        /// </summary>
        /// <param name="info"></param>
        void SetCustomerNotifyRule(CouponsInfo info);


        /// <summary>
        /// 新增优惠券
        /// </summary>
        /// <param name="info"></param>
        void CreateCouponCode(CouponCodeSetting info);
        /// <summary>
        /// 新增优惠券(自动赠送)
        /// </summary>
        /// <param name="info"></param>
        void CreateCouponCodeForSend(CouponCodeSetting info);
        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="couponCodeSysNoList"></param>
        void DelCouponCode(List<int?> couponCodeSysNoList);
        /// <summary>
        /// 删除全部优惠券
        /// </summary>
        /// <param name="couponsSysNo"></param>
        void DelAllCouponCode(int? couponsSysNo);


        bool CheckExistThisTypeCouponCode(int? couponSysNo, CouponCodeType codeType);

        /// <summary>
        /// 校验是否有重复的优惠券代码
        /// </summary>
        bool CheckExistCode(string couponcode);

        /// <summary>
        /// 根据优惠券编号检查优惠券是否有效，不管优惠券是否有效都要返回优惠券代码
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        bool CheckCouponIsValidAndGetCode(int couponSysNo, out string couponCode);

        #endregion 

        bool IsExistEMIS(int emisSysNo);

        #region 促销引擎相关
        /// <summary>
        /// 根据优惠券号码获取活动的SysNo
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        CouponCodeSetting GetActivedCouponCodeInfoByCode(string couponCode);

        /// <summary>
        /// 根据优惠券号码获取活动
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        CouponCodeSetting GetCouponCodeInfoByCode(string couponCode);

        /// <summary>
        /// 获取该活动总的使用次数，该活动当前客户的使用次数；当前代码的使用总使用次数，当前代码当前客户的使用次数
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="couponsSysNo"></param>
        /// <returns></returns>
        DataRow GetCouponsUsedCount(int? customerSysNo, int? couponsSysNo, string couponCode);

        /// <summary>
        /// 触发方式为：注册，生日，支付宝金账户，Check是否存在发放记录
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        bool CheckExistCustomerIssueLog(int? customerSysNo, string couponCode);

        
        /// <summary>
        /// 优惠券应用时，写RedeemLog表,更新活动及优惠券的应用次数和折扣总额
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <param name="customerSysNo"></param>
        /// <param name="shoppingCartSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <param name="redeemAmount"></param>
        void CouponCodeApply(int couponSysNo, string couponCode, int customerSysNo,
            int shoppingCartSysNo, int soSysNo, decimal redeemAmount, string username);

        
        /// <summary>
        /// 已应用的优惠券取消时，更新RedeemLog表中原纪录状态,更新活动及优惠券的应用次数和折扣总额
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <param name="shoppingCartSysNo"></param>
        /// <param name="soSysNo"></param>
        void CouponCodeCancel(string couponCode, int soSysNo, int shoppingCartSysNo, string username);


        /// <summary>
        /// 获取订单产生的优惠卷信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo);
        #endregion

        List<ProductPromotionDiscountInfo> GetCouponAmount(int productSysNo);

        #region 限定商家相关

        /// <summary>
        /// 根据蛋卷获取商家限定信息
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        List<RelVendor> GetVendorSaleRulesByCouponSysNo(int couponSysNo);

        /// <summary>
        /// 创建商家限定
        /// </summary>
        /// <param name="info"></param>
        void CreateVendorSaleRules(CouponsInfo info, int SellerSysNo);

     

        #endregion

        #region 赠送优惠券相关
        /// <summary>
        /// 查询下单时有效或者已完成的商家优惠券活动
        /// </summary>
        /// <param name="merchantSysNo"></param>
        /// <param name="soDateTime"></param>
        /// <returns></returns>
        List<CouponsInfo> QueryCoupons(int merchantSysNo, DateTime soDateTime);
        /// <summary>
        /// 创建优惠卷获取记录
        /// </summary>
        /// <param name="couponCodeCustomerLog"></param>
        void CreateCouponCodeCustomerLog(CouponCodeCustomerLog couponCodeCustomerLog);
        /// <summary>
        /// 检查用户这个订单是否已经赠送过优惠券
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool CheckExistCouponCodeCustomerLog(int couponSysNo, int customerSysNo, int soSysNo);
        #endregion
    }
}
