using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 订单中的商品购买信息
    /// </summary>
    public class OrderProductItem : OrderItem
    {
        /// <summary>
        /// 该商品明细上销售价格的小计
        /// </summary>
        public decimal TotalSalePrice { get { return UnitSalePrice * UnitQuantity; } }

        /// <summary>
        /// 该商品明细上的赠送积分小计
        /// </summary>
        public int TotalRewardedPoint { get { return UnitRewardedPoint * UnitQuantity; } }

        /// <summary>
        /// 会影响成单的标记：0=无影响，整单订单商品； 1=团购商品；2=抢购商品；3-虚拟团购商品
        /// </summary>
        public int SpecialActivityType { get; set; }

        /// <summary>
        /// 团购或抢购的活动编号
        /// </summary>
        public int SpecialActivitySysNo { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool ProductChecked { get; set; }

        public override ExtensibleObject CloneObject()
        {
            OrderProductItem gi = new OrderProductItem()
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
                MerchantName = this.MerchantName,
                SplitGroupPropertyDescList = this.SplitGroupPropertyDescList == null ? null : this.SplitGroupPropertyDescList,
                TotalInventory = this.TotalInventory,
                UnitTaxFee = this.UnitTaxFee,
                WarehouseCountryCode = this.WarehouseCountryCode,
                WarehouseName = this.WarehouseName,
                WarehouseNumber = this.WarehouseNumber,
                SpecialActivitySysNo = this.SpecialActivitySysNo,
                SpecialActivityType = this.SpecialActivityType,
                UnitRewardedPoint = this.UnitRewardedPoint,
                ProductChecked = this.ProductChecked,
            };

            if (this.SplitGroupPropertyDescList != null)
            {
                gi.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                this.SplitGroupPropertyDescList.ForEach(f => gi.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(f.Key, f.Value)));
            }

            return gi;
        }
    }

    /// <summary>
    /// 商品附件
    /// </summary>
    public class OrderAttachment : OrderItem
    {
        /// <summary>
        /// 该商品明细上销售价格的小计
        /// </summary>
        public decimal TotalSalePrice { get { return 0; } }

        /// <summary>
        /// 该商品明细上的赠送积分小计
        /// </summary>
        public int TotalRewardedPoint { get { return UnitRewardedPoint * UnitQuantity; } }


        /// <summary>
        /// 主商品编号，如果为0，则是订单LEVEL的满赠
        /// </summary>
        public int ParentProductSysNo { get; set; }

        /// <summary>
        /// 主商品或主商品所在套餐数量
        /// </summary>
        public int ParentCount { get; set; }

        /// <summary>
        /// 主商品所在套餐编号，如果为0，则不是套餐
        /// </summary>
        public int ParentPackageNo { get; set; }

        public override ExtensibleObject CloneObject()
        {
            OrderAttachment gi = new OrderAttachment()
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
                ParentProductSysNo = this.ParentProductSysNo,
                UnitMarketPrice = this.UnitMarketPrice,
                MerchantSysNo = this.MerchantSysNo,
                MerchantName = this.MerchantName,
                ParentCount = this.ParentCount,
                ParentPackageNo = this.ParentPackageNo,
                SplitGroupPropertyDescList = this.SplitGroupPropertyDescList == null ? null : this.SplitGroupPropertyDescList,
                TotalInventory = this.TotalInventory,
                UnitTaxFee = this.UnitTaxFee,
                WarehouseCountryCode = this.WarehouseCountryCode,
                WarehouseName = this.WarehouseName,
                WarehouseNumber = this.WarehouseNumber,
                UnitRewardedPoint = this.UnitRewardedPoint,
            };

            if (this.SplitGroupPropertyDescList != null)
            {
                gi.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                this.SplitGroupPropertyDescList.ForEach(f => gi.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(f.Key, f.Value)));
            }

            return gi;
        }

    }

    /// <summary>
    /// 订单中的赠品信息
    /// </summary>
    public class OrderGiftItem : OrderItem
    {
        /// <summary>
        /// 赠品类型
        /// </summary>
        public SaleGiftType SaleGiftType { get; set; }

        /// <summary>
        /// 赠品活动编号
        /// </summary>
        public int ActivityNo { get; set; }

        /// <summary>
        /// 赠品活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 赠品池时，最大数量
        /// </summary>
        public int PoolLimitCount { get; set; }

        /// <summary>
        /// 该商品明细上销售价格的小计
        /// </summary>
        public decimal TotalSalePrice { get { return 0; } }

        /// <summary>
        /// 该商品明细上的赠送积分小计
        /// </summary>
        public int TotalRewardedPoint { get { return UnitRewardedPoint * UnitQuantity; } }


        /// <summary>
        /// 主商品编号，如果为0，则是订单LEVEL的满赠
        /// </summary>
        public int ParentProductSysNo { get; set; }

        /// <summary>
        /// 主商品所在套餐编号，如果为0，则不是套餐
        /// </summary>
        public int ParentPackageNo { get; set; }

        /// <summary>
        /// 主商品或主商品所在套餐数量
        /// </summary>
        public int ParentCount { get; set; }


        /// <summary>
        /// 是否是赠品池模式，false=不是
        /// </summary>
        public bool IsGiftPool { get; set; }

        /// <summary>
        /// 若为赠品池赠品，则确认是否被选择
        /// </summary>
        private bool _IsSelect = false;
        public bool IsSelect
        {
            get
            {
                return _IsSelect;
            }
            set
            {
                _IsSelect = value;
            }
        }

        public override ExtensibleObject CloneObject()
        {
            OrderGiftItem gi = new OrderGiftItem()
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
                ParentProductSysNo = this.ParentProductSysNo,
                UnitMarketPrice = this.UnitMarketPrice,
                MerchantSysNo = this.MerchantSysNo,
                MerchantName = this.MerchantName,
                IsGiftPool = this.IsGiftPool,
                SaleGiftType = this.SaleGiftType,
                ActivityNo = this.ActivityNo,
                ActivityName = this.ActivityName,
                IsSelect = this.IsSelect,
                ParentCount = this.ParentCount,
                ParentPackageNo = this.ParentPackageNo,
                SplitGroupPropertyDescList = this.SplitGroupPropertyDescList == null ? null : this.SplitGroupPropertyDescList,
                TotalInventory = this.TotalInventory,
                UnitTaxFee = this.UnitTaxFee,
                WarehouseCountryCode = this.WarehouseCountryCode,
                WarehouseName = this.WarehouseName,
                WarehouseNumber = this.WarehouseNumber,
                UnitRewardedPoint = this.UnitRewardedPoint,
            };

            if (this.SplitGroupPropertyDescList != null)
            {
                gi.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                this.SplitGroupPropertyDescList.ForEach(f => gi.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(f.Key, f.Value)));
            }

            return gi;
        }
    }


    public abstract class OrderItem : ExtensibleObject
    {
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
        /// 商品分组属性列表
        /// </summary>
        public List<KeyValuePair<string, string>> SplitGroupPropertyDescList { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 消费该商品的日期；价格库存与日期无关的商品（如实物类商品）该字段无意义；
        /// 价格库存与日期有关的商品（如服务类商品）该字段表示消费商品的日期
        /// </summary>
        public DateTime? ConsumptionDate { get; set; }

        private int _UnitQuantity = 1;
        /// <summary>
        /// 单位数量
        /// </summary>
        public int UnitQuantity
        {
            get
            {
                return _UnitQuantity;
            }
            set
            {
                _UnitQuantity = value;
            }
        }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal UnitMarketPrice { get; set; }

        /// <summary>
        /// 下单购买时商品的成本单价
        /// </summary>
        public decimal? UnitCostPrice { get; set; }

        /// <summary>
        /// 下单购买时商品的销售单价
        /// </summary>
        public decimal UnitSalePrice { get; set; }

        /// <summary>
        /// 下单时购买商品的单个商品税费
        /// </summary>
        public decimal UnitTaxFee { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// 总库存数量
        /// </summary>
        public int TotalInventory { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int WarehouseNumber { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 仓库所在国家编码
        /// </summary>
        public string WarehouseCountryCode { get; set; }

        /// <summary>
        /// 商品所属商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 商品所属商家名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        ///  商品奖励的积分
        /// </summary>
        public int UnitRewardedPoint { get; set; }

    }
}
