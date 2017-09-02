using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 赠品促销
    /// </summary>
    public class SaleGiftInfo
    {

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string PromotionName { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string PromotionDesc { get; set; }


        /// <summary>
        /// 赠品类型:满赠=F，单品=S，厂商赠品=V，同时购买=M
        /// </summary>
        public SaleGiftType SaleGiftType { get; set; }

        /// <summary>
        /// 指定总数量，如果指定总数量有值并大于0时，则每个赠品商品数量不会使用(即赠品池中可选数量）
        /// </summary>
        public int? ItemGiftCount { get; set; }

        /// <summary>
        /// 门槛金额
        /// </summary>
        public decimal AmountLimit { get; set; }

        /// <summary>
        /// 是否是赠品池模式，false=不是( O=赠品池, A=非赠品池)
        /// </summary>
        public bool IsGiftPool { get; set; }

        /// <summary>
        /// Y 全网       N 非全网
        /// </summary>
        public bool IsGlobal { get; set; }
        /// <summary>
        /// 活动链接
        /// </summary>
        public string PromotionLink { get; set; }

        /// <summary>
        /// 折扣计入方式:BelongGiftItem="G"， BelongMasterItem,"M" 
        /// </summary>
        public string DisCountType { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }


        /// <summary>
        /// 赠品
        /// </summary>
        public List<GiftItem> GiftItemList { get; set; }

        /// <summary>
        /// 赠品赠送规则
        /// </summary>
        public List<GiftSaleRule> GiftSaleRuleList { get; set; }
    }

    /// <summary>
    /// 条件规则
    /// </summary>
    public class GiftSaleRule
    {
        /// <summary>
        /// 活动编号
        /// </summary>
        public int PromotionSysNo { get; set; }

        /// <summary>
        /// 范围类型：品牌=B，品牌和C3组合=C，C3=L，商品=I
        /// </summary>
        public string RelRangeType { get; set; }

        /// <summary>
        /// 设置的C3编号
        /// </summary>
        public int RelC3SysNo { get; set; }

        /// <summary>
        /// 设置的品牌编号
        /// </summary>
        public int RelBrandSysNo { get; set; }

        /// <summary>
        /// 设置的商品编号
        /// </summary>
        public int RelProductSysNo { get; set; }

        /// <summary>
        /// 最小购买数量
        /// </summary>
        public int RelMinQty { get; set; }
        /// <summary>
        /// 关系模式：And=A, Or= O, No=N
        /// </summary>
        public string ComboType { get; set; }
    }


    /// <summary>
    /// 赠品
    /// </summary>
    [Serializable]
    public class GiftItem
    {
        /// <summary>
        /// 是否是赠品池模式，false=不是
        /// </summary>
        public bool IsGiftPool { get; set; }

        /// <summary>
        /// 活动编号
        /// </summary>
        public int PromotionSysNo { get; set; }

        /// <summary>
        /// 商品系统主键
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 消费该商品的日期；价格库存与日期无关的商品（如实物类商品）该字段无意义；
        /// 价格库存与日期有关的商品（如服务类商品）该字段表示消费商品的日期
        /// </summary>
        public DateTime? ConsumptionDate { get; set; }

        /// <summary>
        /// 单位数量
        /// </summary>
        public int UnitQuantity { get; set; }


        /// <summary>
        /// 加购价
        /// </summary>
        public decimal PlusPrice { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        public int Weight { get; set; }

        public string Warranty { get; set; }

        public int UnitRewardedPoint { get; set; }
    }
}
