using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存信息
    /// </summary>
    public class ProductInventoryInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 渠道库存编号
        /// </summary> 
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 渠道库存编号
        /// </summary>
        public StockInfo StockInfo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 财务库存
        /// </summary>
        public int AccountQty { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty { get; set; }

        /// <summary>
        /// 被占用库存
        /// </summary>
        public int AllocatedQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        public int ConsignQty { get; set; }

        /// <summary>
        /// 被订购数量
        /// </summary>
        public int OrderQty { get; set; }

        /// <summary>
        /// 可接受预订的数量(虚库数量)
        /// </summary>
        public int VirtualQty { get; set; }

        /// <summary>
        /// 移仓在途数量
        /// </summary>
        public int? ShiftQty { get; set; }

        /// <summary>
        /// 移仓在途数量(移入)
        /// </summary>
        public int? ShiftInQty { get; set; }

        /// <summary>
        /// 移仓在途数量(移出)
        /// </summary>
        public int? ShiftOutQty { get; set; }

        /// <summary>
        /// 待入库数量
        /// </summary>
        public int PurchaseQty { get; set; }

        /// <summary>
        /// 预留库存数量
        /// </summary>
        public int ReservedQty { get; set; }

        /// <summary>
        /// 线上库存
        /// </summary>
        public int OnlineQty 
        { 
            get
            {
                return AvailableQty + ConsignQty + VirtualQty;
            }
        }

        /// <summary>
        /// 滞销库存?
        /// </summary>
        public int InvalidQty { get; set; }

        /// <summary>
        /// 渠道库存(中蛋网特有)
        /// </summary>
        public int ChannelQty { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public ProductInventoryInfo()
        {
            AccountQty = 0;
            AvailableQty = 0;
            AllocatedQty = 0;
            ConsignQty = 0;
            OrderQty = 0;
            VirtualQty = 0;
            ShiftQty = 0;
            ShiftInQty = 0;
            ShiftOutQty = 0;
            PurchaseQty = 0;
            ReservedQty = 0;            
            InvalidQty = 0;
            ChannelQty = 0;
        }
    }
    /// <summary>
    /// 成本库存锁定操作
    /// </summary>
    public enum CostLockType
    {
        /// <summary>
        /// 不处理锁定成本库存
        /// </summary>
        NoUse,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock,
        /// <summary>
        /// 解锁
        /// </summary>
        Unlock
    }
}
