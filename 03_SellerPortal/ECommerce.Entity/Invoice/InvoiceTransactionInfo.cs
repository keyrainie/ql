using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Invoice
{
    public class InvoiceTransactionInfo
    {
        /// <summary>
        /// 发票Master信息编号
        /// </summary>
        public int? MasterSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 产品编号（可以是商品编号、优惠券编号等。）
        /// </summary>
        public string ItemCode
        {
            get;
            set;
        }

        /// <summary>
        /// 产品类别
        /// </summary>
        public SOItemType? ItemType
        {
            get;
            set;
        }

        /// <summary>
        /// 打印发票时的产品描述信息
        /// </summary>
        public string PrintDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 商品系编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 该条为延保的时候, 对应的主商品productsysno.(多个不同的主商品对应同一个延保, 则是多条记录)
        /// </summary>
        public string MasterProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string ItemDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 单品售价
        /// </summary>
        public decimal? UnitPrice
        {
            get;
            set;
        }

        /// <summary>
        ///  商品原始售价
        /// </summary>
        public decimal? OriginalPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 成本价格
        /// </summary>
        public decimal? UnitCost
        {
            get;
            set;
        }

        /// <summary>
        /// 无税成本价格
        /// </summary>
        public decimal? UnitCostWithoutTax
        {
            get;
            set;
        }

        /// <summary>
        /// 单品数量
        /// </summary>
        public int? Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展价格=单品售价*单品数量
        /// </summary>
        public decimal? ExtendPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 关联订单编号
        /// </summary>
        public int? ReferenceSONumber
        {
            get;
            set;
        }

        /// <summary>
        /// 单品重量
        /// </summary>
        public int? Weight
        {
            get;
            set;
        }

        /// <summary>
        /// 获赠积分
        /// </summary>
        public int? GainPoint //Point
        {
            get;
            set;
        }

        /// <summary>
        /// 商品支付类型
        /// </summary>
        public int? PayType //PointType
        {
            get;
            set;
        }

        /// <summary>
        /// 质保信息
        /// </summary>
        public string Warranty
        {
            get;
            set;
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string BriefName
        {
            get;
            set;
        }

        /// <summary>
        /// 商品售价类型
        /// </summary>
        public SOProductPriceType? PriceType//IsMemberPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡编号
        /// </summary>
        public int? GiftSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 保价费
        /// </summary>
        public decimal? PremiumAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal? ShippingCharge
        {
            get;
            set;
        }

        /// <summary>
        /// 附加费
        /// </summary>
        public decimal? ExtraAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal? CashPaid
        {
            get;
            set;
        }

        /// <summary>
        /// 积分支付金额
        /// </summary>
        public decimal? PointPaid
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal? DiscountAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预付款金额
        /// </summary>
        public decimal? PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 优惠券抵扣金额
        /// </summary>
        public decimal? PromotionDiscount
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡抵扣金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// CompanyCode，冗余
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }
}
