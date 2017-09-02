using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.Utility;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class OrderProductItemModel:OrderItem
    {
        /// <summary>
        /// 该商品明细上销售价格的小计
        /// </summary>
        public decimal TotalSalePrice { get; set; }

        /// <summary>
        /// 该商品明细上的赠送积分小计
        /// </summary>
        public int TotalRewardedPoint { get; set; }

        /// <summary>
        /// 会影响成单的标记：0=无影响，整单订单商品； 1=团购商品；2=抢购商品；
        /// </summary>
        public int SpecialActivityType { get; set; }

        /// <summary>
        /// 团购或抢购的活动编号
        /// </summary>
        public int SpecialActivitySysNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<OrderAttachment> AttachmentList { get; set; }

        /// <summary>
        /// 赠品
        /// </summary>
        public List<OrderGiftItem> GiftList { get; set; }

        /// <summary>
        /// 是否被收藏
        /// </summary>
        public bool IsWished { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool ProductChecked { get; set; }

        public override ExtensibleObject CloneObject()
        {
            OrderProductItemModel gi = new OrderProductItemModel()
            {
                ProductSysNo = this.ProductSysNo,
                ConsumptionDate = this.ConsumptionDate,
                ProductID = this.ProductID,
                ProductName = this.ProductName,
                UnitQuantity = this.UnitQuantity,
                UnitCostPrice = this.UnitCostPrice,
                UnitSalePrice = this.UnitSalePrice,
                DefaultImage = this.DefaultImage,
                Weight = this.Weight,
                UnitMarketPrice = this.UnitMarketPrice,
                MerchantSysNo = this.MerchantSysNo,
                SplitGroupPropertyDescList = this.SplitGroupPropertyDescList == null ? null : this.SplitGroupPropertyDescList,
                TotalInventory = this.TotalInventory,
                UnitTaxFee = this.UnitTaxFee,
                WarehouseCountryCode = this.WarehouseCountryCode,
                WarehouseName = this.WarehouseName,
                WarehouseNumber = this.WarehouseNumber,
                UnitRewardedPoint = this.UnitRewardedPoint,
                TotalSalePrice = this.TotalSalePrice,
                TotalRewardedPoint = this.TotalRewardedPoint,
                SpecialActivityType = this.SpecialActivityType,
                SpecialActivitySysNo = this.SpecialActivitySysNo,
                Quantity = this.Quantity,
                AttachmentList = this.AttachmentList,
                GiftList = this.GiftList,
                IsWished=this.IsWished,
                UnitPrice = this.UnitPrice,
                ProductChecked = this.ProductChecked
            };

            if (this.SplitGroupPropertyDescList != null)
            {
                gi.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                this.SplitGroupPropertyDescList.ForEach(f => gi.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(f.Key, f.Value)));
            }

            return gi;
        }
    }
}