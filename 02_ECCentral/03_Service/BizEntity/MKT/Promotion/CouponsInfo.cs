using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 优惠券活动
    /// </summary>
    public class CouponsInfo : IIdentity, IWebChannel, ICompany
    {
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
        /// 编辑时间
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

        /// <summary>
        /// 活动状态
        /// </summary>
        public CouponsStatus? Status { get; set; }        
        /// <summary>
        /// 用户表示
        /// </summary>
        public string UserDescription { get; set; }
        /// <summary>
        /// 生产商系统板ianhao
        /// </summary>
        public int? MerchantSysNo { get; set; }
        /// <summary>
        /// EIMS系统编号
        /// </summary>
        public int? EIMSSysNo { get; set; }

        /// <summary>
        /// MKT活动类型, channelType
        /// </summary>
        public CouponsMKTType? CouponChannelType { get; set; }

        /// <summary>
        /// 活动优惠类型, coupontype
        /// </summary>
        public CouponsRuleType? CouponRuleType { get; set; }

        /// <summary>
        /// 商品范围, rulestype
        /// </summary>
        public CouponsProductRangeType? ProductRangeType { get; set; }

        #endregion

        /// <summary>
        /// 是否自动应用
        /// </summary>
        public bool? IsAutoUse { get; set; }
        
        #region 发放规则
        /// <summary>
        /// 是否邮件通知
        /// </summary>
        public bool? IsSendMail { get; set; }

        /// <summary>
        /// 是否定时发送
        /// </summary>
        public bool? IsAutoBinding { get; set; }

        /// <summary>
        /// 定时发放到账户中心日期
        /// </summary>
        public DateTime? BindingDate { get; set; }

        /// <summary>
        /// 触发条件
        /// </summary>
        public CouponsBindConditionType? BindCondition { get; set; }

        /// <summary>
        /// 优惠券有效期设置类型
        /// </summary>
        public CouponsValidPeriodType? ValidPeriod { get; set; }

        /// <summary>
        /// 自定义有效期开始时间
        /// </summary>
        public DateTime? CustomBindBeginDate { get; set; }

        /// <summary>
        /// 自定义有效期结束时间
        /// </summary>
        public DateTime? CustomBindEndDate { get; set; }
        /// <summary>
        /// 绑定状态
        /// </summary>
        public string BindingStatus { get; set; }

        public CouponBindRule BindRule { get; set; }
        
        #endregion

        #region 优惠券代码设置        
        /// <summary>
        /// 优惠券代码设置
        /// </summary>
        public CouponCodeSetting CouponCodeSetting { get; set; }

        /// <summary>
        /// 是否存在投放型优惠券
        /// </summary>
        public bool? IsExistThrowInTypeCouponCode { get; set; }
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
         
        /// <summary>
        /// 客户范围条件：
        /// </summary>
        public PSCustomerCondition CustomerCondition { get; set; }
         
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

        #region 规则，优惠券中，使用PriceDiscountRuleList和PriceDiscountRuleList
        
        /// <summary>
        /// 商品优惠价格设置        
        /// </summary>
        public List<PSPriceDiscountRule> PriceDiscountRule { get; set; }

        /// <summary>
        /// 订单金额折扣规则
        ///     折扣金额 -> PSAmountDiscountRule.OrderAmountDiscountRule，直接折扣金额类型
        ///     折扣百分比 -> PSAmountDiscountRule.OrderAmountDiscountRule，百分比类型
        /// </summary>
        public PSOrderAmountDiscountRule OrderAmountDiscountRule { get; set; }

        #endregion
    }



    /// <summary>
    /// 活动优惠券设置条件：在促销计算引擎中，用来判断是否符合使用优惠券的条件
    /// </summary>
    public class CouponCodeSetting
    {
        /// <summary>
        /// 优惠券代码系统编号
        /// </summary>
        public int? CodeSysNo { get; set; }

        /// <summary>
        /// 优惠券活动系统编号
        /// </summary>
        public int? CouponsSysNo { get; set; }

        /// <summary>
        /// 代码 开始日期
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 代码 结束日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 优惠券类型
        /// </summary>
        public CouponCodeType? CouponCodeType { get; set; }

        /// <summary>
        /// 预计转化率
        /// </summary>
        public decimal? DueInvertRate { get; set; }

        /// <summary>
        /// 代码 每个Customer ID限用次数 :CCCustomerMaxFrequency
        /// </summary>
        public int? CCCustomerMaxFrequency { get; set; }

        /// <summary>
        /// 代码 全网限用次数 :TotalCount
        /// </summary>
        public int? CCMaxFrequency { get; set; }

        /// <summary>
        /// 代码 已使用次数 :UsedCount
        /// </summary> 
        public int? UsedCount { get; set; }

        /// <summary>
        /// 代码 已使用金额
        /// </summary>
        public int? UsedAmount { get; set; }

        /// <summary>
        /// 总折扣数
        /// </summary>
        public decimal? TotalDiscount { get; set; }
 

        /// <summary>
        /// 优惠券代码 
        /// </summary>
        public string CouponCode { get; set; }

        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 投放型本次生成多少张
        /// </summary>
        public int? ThrowInCodeCount { get; set; }
    }

    public class CouponBindRule
    {
        /// <summary>
        /// 触发门槛金额
        /// </summary>
        public decimal? AmountLimit { get; set; }

        /// <summary>
        /// 商品范围限定
        /// </summary>
        public ProductRangeType ProductRangeType { get; set; }

        /// <summary>
        /// 商品范围设置信息
        /// </summary>
        public RelProduct RelProducts { get; set; }
    }
}
