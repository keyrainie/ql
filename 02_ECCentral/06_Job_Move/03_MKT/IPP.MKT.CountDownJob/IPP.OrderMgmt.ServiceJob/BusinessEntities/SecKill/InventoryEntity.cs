using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Newegg.Oversea.Framework.Entity;


namespace IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill
{
    public class InventoryEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int InventorySysNo { get; set; }

        [DataMapping("AccountQty", DbType.Int32)]
        public int AccountQty { get; set; }

        [DataMapping("AllocatedQty", DbType.Int32)]
        public int AllocatedQty { get; set; }

        [DataMapping("AvailableQty", DbType.Int32)]
        public int AvailableQty { get; set; }

        [DataMapping("ConsignQty", DbType.Int32)]
        public int ConsignQty { get; set; }

        [DataMapping("OrderQty", DbType.Int32)]
        public int OrderQty { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }      

        [DataMapping("VirtualQty", DbType.Int32)]
        public int VirtualQty { get; set; }

        [DataMapping("ReservedQty", DbType.Int32)]
        public int ReservedQty { get; set; }
    }

    public class InventoryStockEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("AccountQty", DbType.Int32)]
        public int AccountQty { get; set; }

        [DataMapping("AvailableQty", DbType.Int32)]
        public int AvailableQty { get; set; }

        [DataMapping("AllocatedQty", DbType.Int32)]
        public int AllocatedQty { get; set; }

        [DataMapping("OrderQty", DbType.Int32)]
        public int OrderQty { get; set; }

        [DataMapping("ConsignQty", DbType.Int32)]
        public int ConsignQty { get; set; }

        [DataMapping("VirtualQty", DbType.Int32)]
        public int VirtualQty { get; set; }
      
        public int SubCountdownQty { get; set; }     

        [DataMapping("ReservedQty", DbType.Int32)]
        public int ReservedQty { get; set; }
    }
}
