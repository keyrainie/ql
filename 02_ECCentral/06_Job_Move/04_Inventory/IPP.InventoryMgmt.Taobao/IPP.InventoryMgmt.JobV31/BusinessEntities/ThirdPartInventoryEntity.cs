using System;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    public class ThirdPartInventoryEntity
    {
        [DataMapping("ProductMappingSysno",DbType.Int32)]
        public int ProductMappingSysno { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("PartnerType", DbType.String)]
        public string PartnerType { get; set; }

        /// <summary>
        /// 第三方标识ID
        /// </summary>
        [DataMapping("SKU", DbType.String)]
        public string SKU { get; set; }

        /// <summary>
        /// 上次同步完成后的第三方的库存
        /// </summary>
        [DataMapping("SynInventoryQty", DbType.Int32)]
        public int SynInventoryQty { get; set; }

        /// <summary>
        /// 当前的库存预警值
        /// </summary>
        [DataMapping("InventoryAlamQty", DbType.Int32)]
        public int? InventoryAlamQty { get; set; }

        /// <summary>
        /// 当前newegg的库存量
        /// </summary>
        [DataMapping("InventoryOnlineQty", DbType.Int32)]
        public int InventoryOnlineQty { get; set; }

        /// <summary>
        /// 上次同步完成后的库存预警值
        /// </summary>
        [DataMapping("OldInventoryAlamQty", DbType.Int32)]
        public int OldInventoryAlamQty { get; set; }
    }
}
