using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 退款仓库信息
    /// </summary>
    public class WarehouseRefund
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseNumber;
        /// <summary>
        /// 退款单件列表
        /// </summary>
        public List<RefundRegister> RefundRegisters;
    }
    /// <summary>
    /// 退款单件
    /// </summary>
    public class RefundRegister
    {
        /// <summary>
        /// 折扣价值
        /// </summary>
        public decimal? DiscountAmount;
        /// <summary>
        /// 积分
        /// </summary>
        public int? Point;
        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? Price;
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost;
        /// <summary>
        /// 活动折扣
        /// </summary>
        public decimal? PromotionDiscount;
        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity;
        /// <summary>
        /// 支付方式
        /// </summary>
        public ProductPayType? PointType;
        /// <summary>
        /// 主商品编号
        /// </summary>
        public int? MasterProductSysNo;
        /// <summary>
        /// 原始价格
        /// </summary>
        public decimal? OriginalPrice;
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo;
        /// <summary>
        /// 订单商品类型
        /// </summary>
        public SOProductType? ProductType;
        /// <summary>
        /// 未税促销价格
        /// </summary>
        public decimal? UnitCostWithoutTax;
        /// <summary>
        /// 单件信息
        /// </summary>
        public RMARegisterInfo Register;
        /// <summary>
        /// 积分支付
        /// </summary>
        public int? PointPay;
    }
}
