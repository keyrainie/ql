using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.OutStock
{
  public  class SOItem4OutStockEntity
    {
      [DataMapping("SysNo", DbType.Int32)]
      public int SysNo { get; set; }

      [DataMapping("SOSysNo", DbType.Int32)]
      public int SOSysNo { get; set; }

      [DataMapping("ProductSysNo", DbType.Int32)]
      public int ProductSysNo { get; set; }

      [DataMapping("ProductID", DbType.String)]
      public string ProductID { get; set; }

      [DataMapping("Quantity", DbType.Int32)]
      public int Quantity { get; set; }

      [DataMapping("WarehouseNumber", DbType.String)]
      public string WarehouseNumber { get; set; }
    }
}
