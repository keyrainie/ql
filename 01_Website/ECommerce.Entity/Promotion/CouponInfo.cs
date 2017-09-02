using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 优惠券信息
    /// </summary>
    public class CouponInfo
    {
        public int SysNo { get; set; }

        public string CouponName { get; set; }

        public string CouponDesc { get; set; }

        /// <summary>
        /// 活动优惠类型：D=商品折扣，S=免运费（暂不提供）
        /// </summary>
        public string CouponType { get; set; }

        /// <summary>
        /// MKT活动类型, O=MKT线上推广优惠券
        /// </summary>
        public string ChannelType { get; set; }

        /// <summary>
        /// 商品范围:A=所有商品，X=只限制类别和品牌，I=限制商品
        /// </summary>
        public string RulesType { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        public string Status { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public int EIMSSysNo { get; set; }

        public int MerchantSysNo { get; set; }

        public string MerchantName { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }

        /// <summary>
        /// 优惠折扣清单
        /// </summary>
        public List<Coupon_DiscountRules> DiscountRuleList { get; set; }

        /// <summary>
        /// 使用的范围规则
        /// </summary>
        public List<Coupon_SaleRules> SaleRulesList { get; set; }

        /// <summary>
        /// 使用的一些特殊规则
        /// </summary>
        public Coupon_SaleRules_Ex SaleRulesEx { get; set; }

        /// <summary>
        /// 指定Customer使用的CustomerSysNo List
        /// </summary>
        public List<int> AssignCustomerList { get; set; }

        /// <summary>
        /// 当前拿来使用的优惠券号码
        /// </summary>
        public string CurrentUsingCouponCode { get; set; }
    }

    public class Coupon_DiscountRules
    {
        public int CouponSysNo { get; set; }

        public CouponDiscountType DiscountType
        {
            get
            {
                switch (RulesType.ToUpper().Trim())
                {
                    case "P":
                        return CouponDiscountType.OrderAmountPercentage;
                    case "D":
                        return CouponDiscountType.OrderAmountDiscount;
                    case "Z":
                        return CouponDiscountType.ProductPriceDiscount;
                    case "F":
                        return CouponDiscountType.ProductPriceFinal;
                    default:
                        return CouponDiscountType.OrderAmountDiscount;
                }
            }
        }

        public string RulesType { get; set; }

        public decimal Amount { get; set; }

        public decimal Value { get; set; }

        public int Quantity { get; set; }

        public int ProductSysNo { get; set; }

    }

    public class Coupon_SaleRules
    {
        public int CouponSysNo { get; set; }
        public string Type { get; set; }
        public CouponSaleRuleType SaleRuleType
        {
            get
            {
                switch (Type.ToUpper().Trim())
                {
                    case "B":
                        return CouponSaleRuleType.RelBrand;
                    case "C":
                        return CouponSaleRuleType.RelCategory;
                    case "S":
                        return CouponSaleRuleType.RelSeller;
                    case "R":
                        return CouponSaleRuleType.RelCustomerRank;
                    case "A":
                        return CouponSaleRuleType.RelArea;
                    case "I":
                        return CouponSaleRuleType.RelProduct;
                    default:
                        return CouponSaleRuleType.RelProduct;
                }
            }
        }
        public int C3SysNo { get; set; }
        public int BrandSysNo { get; set; }
        public int ProductSysNo { get; set; }
        public int CustomerRank { get; set; }
        public int AreaSysNo { get; set; }
        public int SellerSysNo { get; set; }
        /// <summary>
        /// Y=包含，N=排除
        /// </summary>
        public string RelationType { get; set; }
    }

    public class Coupon_SaleRules_Ex
    {
        public int CouponSysNo { get; set; }
        public decimal OrderAmountLimit { get; set; }
        public int? PayTypeSysNo { get; set; }
        public int? ShippingTypeSysNo { get; set; }
        public decimal? OrderMaxDiscount { get; set; }
        public int? CustomerMaxFrequency { get; set; }
        public int? MaxFrequency { get; set; }
        public int? UsedFrequency { get; set; }
        /// <summary>
        /// N=不需要，Y=需要
        /// </summary>
        public string NeedEmailVerification { get; set; }
        /// <summary>
        /// N=不需要，Y=需要
        /// </summary>
        public string NeedMobileVerification { get; set; }
        /// <summary>
        /// N=不限制大使，Y=需要限制大使
        /// </summary>
        public string InvalidForAmbassador { get; set; }

        public string IsAutoUse { get; set; }
    }




}
