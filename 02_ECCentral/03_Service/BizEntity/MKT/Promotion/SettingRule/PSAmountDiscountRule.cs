using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 订单金额折扣规则
    /// </summary>
    public class PSOrderAmountDiscountRule
    {
        /// <summary>
        /// 每单折扣上限
        /// </summary>
        public decimal? OrderMaxDiscount { get; set; }
        /// <summary>
        /// 折扣和等级对应关系信息
        /// </summary>
        public List<OrderAmountDiscountRank> OrderAmountDiscountRank { get; set; }
         
    }
    /// <summary>
    /// 折扣和等级对应关系信息
    /// </summary>
    public class OrderAmountDiscountRank
    {
        /// <summary>
        /// 门槛金额,限定金额
        /// </summary>
        public decimal? OrderMinAmount { get; set; }

        /// <summary>
        /// 折扣方式：比率，直扣
        /// </summary>
        public PSDiscountTypeForOrderAmount? DiscountType { get; set; }

        /// <summary>
        /// 订单金额折扣所设置的数值
        /// </summary>
        public decimal? DiscountValue { get; set; }
    }


    
}
