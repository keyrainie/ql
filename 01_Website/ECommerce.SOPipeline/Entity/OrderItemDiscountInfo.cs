using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public class OrderItemDiscountInfo : ExtensibleObject
    {
        /// <summary>
        /// 商品系统主键,如果是整单满减这类，那么ProductSysNo=0
        /// </summary>
        public int ProductSysNo { get; set; }


        /// <summary>
        /// 优惠活动的类型（1=套餐）
        /// </summary>
        public int DiscountType { get; set; }

        /// <summary>
        /// 优惠类型下的活动编号 ，OrderItemDiscountInfo中的DiscountActivityNo（不过可扩展为更多促销类型的编号）
        /// </summary>
        public int DiscountActivityNo { get; set; }

        /// <summary>
        /// 套餐编号，非套餐为0;等同于OrderItemGroup中的PackageNo，赠品和附件对象中的ParentPackageNo
        /// </summary>
        public int PackageNo { get; set; }

        /// <summary>
        /// 优惠类型下的活动名称
        /// </summary>
        public string DiscountActivityName { get; set; }

        /// <summary>
        /// 享受该优惠的商品数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单个折扣,统一正数
        /// </summary>
        public decimal UnitDiscount { get; set; }


        /// <summary>
        /// 该优惠活动奖励的账户余额
        /// </summary>
        public decimal UnitRewardedBalance { get; set; }

        /// <summary>
        /// 该优惠活动奖励的积分
        /// </summary>
        public int UnitRewardedPoint { get; set; }

        /// <summary>
        /// 该优惠活动减免的运费金额
        /// </summary>
        public decimal UnitShipFeeDiscountAmt { get; set; }

        public override ExtensibleObject CloneObject()
        {
            return new OrderItemDiscountInfo()
            {
                DiscountType = this.DiscountType,
                DiscountActivityNo = this.DiscountActivityNo,
                Quantity = this.Quantity,
                UnitDiscount = this.UnitDiscount,
                UnitRewardedBalance = this.UnitRewardedBalance,
                UnitRewardedPoint = this.UnitRewardedPoint,
                UnitShipFeeDiscountAmt = this.UnitShipFeeDiscountAmt,
                DiscountActivityName = this.DiscountActivityName,
                PackageNo = this.PackageNo,
                ProductSysNo = this.ProductSysNo

            };
        }
    }
}
