using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 退款单单件信息
    /// </summary>
    public class RefundItemInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 退款单系统编号
        /// </summary>
        public int? RefundSysNo { get; set; }
        /// <summary>
        /// 单件号
        /// </summary>
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 商品金额（销售金额-优惠金额）
        /// </summary>
        public decimal? OrgPrice { get; set; }
        /// <summary>
        /// 促销价值
        /// </summary>
        public decimal? UnitDiscount { get; set; }
        /// <summary>
        /// 商品价值（=OrgPrice + UnitDiscount）
        /// </summary>
        public decimal? ProductValue { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int? OrgPoint { get; set; }
        /// <summary>
        /// 退款价值
        /// </summary>
        public decimal? RefundPrice { get; set; }
        /// <summary>
        /// 同IPP一致，默认该价格为订单处理核算过的原始价格,此变量只是计算时临时保存数据
        /// fix bug 89685 by jack 2012-10-24
        /// </summary>
        public decimal? RealOrgPrice { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public ProductPayType? PointType { get; set; }
        /// <summary>
        /// 初算退款现金
        /// </summary>
        public decimal? RefundCash { get; set; }
        /// <summary>
        /// 初算退款积分
        /// </summary>
        public int? RefundPoint { get; set; }
        /// <summary>
        /// 退款计算方式
        /// </summary>
        public ReturnPriceType? RefundPriceType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundCost { get; set; }
        /// <summary>
        /// 退积分
        /// </summary>
        public int? RefundCostPoint { get; set; }
        /// <summary>
        /// 退款金额不含增值税
        /// </summary>
        public decimal? RefundCostWithoutTax { get; set; }
        /// <summary>
        /// 退礼品卡金额
        /// </summary>
        public decimal? OrgGiftCardAmt { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public SOProductType? ProductType
        {
            get;
            set;
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 所属公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
