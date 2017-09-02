using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 促销活动信息的基类
    /// </summary>
    public class PromotionBaseInfo : IIdentity, IWebChannel, ICompany 
    {
        public PromotionBaseInfo()
        {
            WebChannel = new WebChannel();
            ProductCondition = new PSProductCondition();
            CustomerCondition = new PSCustomerCondition();
            OrderCondition = new PSOrderCondition();
            UsingFrequencyCondition = new PSActivityFrequencyCondition();
        }

        #region 基础
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public LanguageContent Title { get; set; }

        /// <summary>
        /// 促销类型
        /// </summary>
        public PromotionType? PromotionType { get; set; }

        /// <summary>
        /// 审核人 
        /// </summary>
        public string AuditUser { get; set; }
               
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 最后一次编辑用户
        /// </summary>
        public string EditUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public WebChannel WebChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        #endregion

        #region 条件
        /// <summary>
        /// 时间条件：起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 时间条件：结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /*  组合销售中，只填充RelProduct，并且RelProduct只能是Include类型
         *  优惠券中，所有商品范围模式都可能使用
         *  赠品中，所有商品范围模式都可能使用
         * */
        /// <summary>
        /// 商品范围条件        
        /// </summary>
        public PSProductCondition ProductCondition { get; set; }


        /**  目前有优惠券使用 **/
        /// <summary>
        /// 客户范围条件：
        /// </summary>
        public PSCustomerCondition CustomerCondition { get; set; }

        /**  目前有优惠券，赠品使用 **/
        /// <summary>
        /// 订单条件
        /// </summary>
        public PSOrderCondition OrderCondition { get; set; }

        /**  目前有优惠券使用 **/
        /// <summary>
        /// 活动使用次数的限制条件
        /// </summary>
        public PSActivityFrequencyCondition UsingFrequencyCondition { get; set; }

        #endregion

        #region 规则
        /* 组合销售中，优惠折扣类型只能是商品价格中的ProductPriceDiscount类型
         * 限时销售中，优惠折扣类型只能是商品价格中的ProductPriceFinal类型
         * 优惠券中，使用PriceDiscountRuleList和PriceDiscountRuleList
         * */
        /// <summary>
        /// 商品优惠价格设置        
        /// </summary>
        public List<PSPriceDiscountRule> PriceDiscountRule { get; set; }

        /**  目前有优惠券使用 **/
        /// <summary>
        /// 订单金额折扣规则
        ///     折扣金额 -> PSAmountDiscountRule.OrderAmountDiscountRule，直接折扣金额类型
        ///     折扣百分比 -> PSAmountDiscountRule.OrderAmountDiscountRule，百分比类型
        /// </summary>
        public PSOrderAmountDiscountRule OrderAmountDiscountRule { get; set; }

        /**  目前有赠品使用 **/
        /// <summary>
        /// 赠品规则
        /// </summary>
        public PSGiftItemRule GiftItemRule { get; set; }
        /// <summary>
        /// 积分规则
        /// </summary>
        public PSPointScoreRule PointScoreRule { get; set; }
        /// <summary>
        /// 优惠券规则
        /// </summary>
        public PSCouponsRebateRule  CouponsRebateRule { get; set; }

        #endregion

 

        

    }
}
