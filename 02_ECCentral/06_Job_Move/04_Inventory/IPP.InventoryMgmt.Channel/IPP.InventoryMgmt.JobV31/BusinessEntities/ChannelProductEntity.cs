using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities
{
    /// <summary>
    /// 最终同步的结果
    /// </summary>
    [Serializable]
    public abstract class ChannelProductEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ChannelCode", DbType.String)]
        public string ChannelCode { get; set; }

        [DataMapping("ChannelSysNo", DbType.Int32)]
        public int ChannelSysNo { get; set; }

        [DataMapping("SynProductID", DbType.String)]
        public string SynProductID { get; set; }

        public abstract int SynChannelQty
        {
            get;
        }

        [DataMapping("SkuID", DbType.String)]
        public string SkuID { get; set; }

    }

    /// <summary>
    /// 比例库存数据
    /// </summary>
    [Serializable]
    public class ChannelProductPercentEntity : ChannelProductEntity
    {
        //[DataMapping("ProductSysNo", DbType.Int32)]
        //public int ProductSysNo { get; set; }

        //[DataMapping("ChannelSysNo", DbType.Int32)]
        //public int ChannelSysNo { get; set; }

        //[DataMapping("ChannelCode", DbType.String)]
        //public string ChannelCode { get; set; }

        //[DataMapping("SynProductID", DbType.String)]
        //public string SynProductID { get; set; }

        [DataMapping("InventoryPercent", DbType.Decimal)]
        public decimal InventoryPercent { get; set; }

        [DataMapping("SafeInventoryCount", DbType.Int32)]
        public int SafeInventoryCount { get; set; }

        [DataMapping("MaxOnlineQty", DbType.Int32)]
        public int MaxOnlineQty { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("IsClearInventory", DbType.AnsiStringFixedLength)]
        public string IsClearInventory { get; set; }

        [DataMapping("Status", DbType.AnsiStringFixedLength)]
        public string Status { get; set; }

        




        public override int SynChannelQty
        {
            get
            {
                if (IsClearInventory == "Y")
                {
                    return 0;
                }
                int qty = Convert.ToInt32(Math.Floor(MaxOnlineQty * InventoryPercent)) - SafeInventoryCount;
                return qty > 0 ? qty : 0;
            }
        }

    }

    /// <summary>
    /// 指定库存数据
    /// </summary>
    [Serializable]
    public class ChannelProductAppointEntity : ChannelProductEntity
    {
        //[DataMapping("ProductSysNo", DbType.Int32)]
        //public int ProductSysNo { get; set; }

        //[DataMapping("ChannelSysNo", DbType.Int32)]
        //public int ChannelSysNo { get; set; }

        //[DataMapping("ChannelCode", DbType.String)]
        //public string ChannelCode { get; set; }

        //[DataMapping("SynProductID", DbType.String)]
        //public string SynProductID { get; set; }

        [DataMapping("ChannelQty", DbType.Int32)]
        public int ChannelQty { get; set; }

        [DataMapping("IsClearInventory", DbType.AnsiStringFixedLength)]
        public string IsClearInventory { get; set; }

        public override int SynChannelQty
        {
            get
            {
                return IsClearInventory == "Y" ? 0 : ChannelQty;
            }
        }
    }
}
