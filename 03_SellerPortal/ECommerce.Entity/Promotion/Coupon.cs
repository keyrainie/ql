using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class Coupon : EntityBase
    {
        public int? SysNo { get; set; }

        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券活动描述
        /// </summary>
        public string CouponDesc { get; set; }

        /// <summary>
        /// 活动状态  初始态、就绪、运行、作废、终止、完成
        /// </summary>
        public CouponStatus? Status { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        public int MerchantSysNo { get; set; }
        public DateTime? InDate { get; set; }
        public string InUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditUser { get; set; }
        public DateTime? AuditDate { get; set; }
        public string AuditUser { get; set; }

        /// <summary>
        /// 折扣方式列表
        /// </summary>
        public List<CouponDiscountRule> DiscountRules { get; set; }

        /// <summary>
        /// 使用规则
        /// </summary>
        public CouponSaleRule SaleRule { get; set; }

        /// <summary>
        /// 获取规则
        /// </summary>
        public CouponBindRule BindRule { get; set; }

        /// <summary>
        /// 优惠券列表
        /// </summary>
        public List<CouponCode> CouponCodes { get; set; }

        public CouponCode GeneralCode { get; set; }
        /// <summary>
        /// 批量投放优惠券字符串（以/n串联）
        /// </summary>
        public string ThrowInCodes { get; set; }

    }

    public class CouponDiscountRule : EntityBase
    {
        public int SysNo { get; set; }
        public int CouponSysNo { get; set; }
        /// <summary>
        /// 阶梯金额
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// 规则类型   折扣金额、折扣比例
        /// </summary>
        public CouponDiscountRuleType RulesType { get; set; }

        /// <summary>
        /// 折扣数值
        /// </summary>
        public decimal? Value { get; set; }
    }

    public class CouponSaleRule : EntityBase
    {
        public int SysNo { get; set; }
        public int CouponSysNo { get; set; }
        /// <summary>
        /// 订单金额下限
        /// </summary>
        public decimal? OrderAmountLimit { get; set; }
        /// <summary>
        /// 每单最大折扣
        /// </summary>
        public decimal? OrderMaxDiscount { get; set; }
        /// <summary>
        /// 整站最大使用次数
        /// </summary>
        public int? MaxFrequency { get; set; }
        /// <summary>
        /// 每个顾客最大使用次数
        /// </summary>
        public int? CustomerMaxFrequency { get; set; }
        /// <summary>
        /// 商品范围
        /// </summary>
        public ProductRelation ProductRange { get; set; }
        /// <summary>
        /// 顾客使用范围限制
        /// </summary>
        public CustomerRelation CustomerRange { get; set; }
    }

    public class CouponBindRule : EntityBase
    {
        public int SysNo { get; set; }
        public int CouponSysNo { get; set; }
        /// <summary>
        /// 获取方式 A-不限，S-购物
        /// </summary>
        public CouponsBindConditionType BindCondition { get; set; }
        /// <summary>
        /// 有效期 0-不限, 1-自发放日起一周, 2-自发放日起一个月, 3-自发放日起两个月, 4-自发放日起三个月, 5-自发放日起六个月, 6-自定义
        /// </summary>
        public CouponValidPeriodType ValidPeriod { get; set; }

        /// <summary>
        /// 自定义 有效期开始时间
        /// </summary>
        public DateTime? BindBeginDate { get; set; }
        /// <summary>
        /// 自定义 有效期结束时间
        /// </summary>
        public DateTime? BindEndDate { get; set; }
        /// <summary>
        /// 触发门槛金额
        /// </summary>
        public decimal? AmountLimit { get; set; }
        /// <summary>
        /// 商品范围
        /// </summary>
        public ProductRelation ProductRange { get; set; }
    }

    public class ProductRelation
    {
        public ProductRangeType ProductRangeType { get; set; }
        public RelationType RelationType { get; set; }
        public List<RelProduct> Products { get; set; }
    }

    public class RelProduct
    {
        public int SysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductStatus { get; set; }
    }

    public class CustomerRelation : EntityBase
    {
        public CouponCustomerRangeType CustomerRangeType { get; set; }

        public RelationType RelationType { get; set; }

        /// <summary>
        /// 顾客使用范围限制
        /// </summary>
        public List<RelCustomer> Customers { get; set; }
    }

    public class RelCustomer : EntityBase
    {
        public int SysNo { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }
    }

    public class CouponCode : EntityBase
    {
        public int SysNo { get; set; }
        public int CouponSysNo { get; set; }
        public string Code { get; set; }

        private string codes;
        /// <summary>
        /// 批量生成代码
        /// </summary>
        public string Codes
        {
            get;
            set;
            //get
            //{
            //    if (!string.IsNullOrWhiteSpace(Code))
            //    {
            //        codes = Code + "," + codes;
            //    }
            //    return codes;
            //}
            //set
            //{
            //    codes = value;
            //}
        } 
        public CouponCodeType CodeType { get; set; }

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? TotalCount { get; set; }
        public int? CustomerMaxFrequency { get; set; }

        public int? UseCount { get; set; }

        public DateTime? InDate { get; set; }
        public string InUser { get; set; }
    }


}
