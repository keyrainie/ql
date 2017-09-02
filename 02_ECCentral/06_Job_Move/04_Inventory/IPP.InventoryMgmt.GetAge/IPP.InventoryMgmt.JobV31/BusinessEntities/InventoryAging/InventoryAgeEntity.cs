using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities;
using System.Data;


namespace IPP.InventoryMgmt.JobV31.BusinessEntities.InventoryAging
{
    [Serializable]
    public class InventoryAgeEntity
    {
        [DataMapping("InventoryDate", DbType.DateTime )]
        public DateTime InventoryDate { get; set; }

        [DataMapping("Item_Key", DbType.String )]
        public string Item_Key { get; set; }

        [DataMapping("VendorNumber", DbType.Int32 )]
        public int VendorNumber { get; set; }

        [DataMapping("Quantity", DbType.Int32 )]
        public int? Quantity { get; set; }

        [DataMapping("DateRange", DbType.String )]
        public string DateRange { get; set; }

        [DataMapping("InStockDays", DbType.Int32 )]
        public int? InStockDays { get; set; }

        [DataMapping("UnitCost", DbType.Decimal )]
        public decimal? UnitCost { get; set; }
        
    }
}