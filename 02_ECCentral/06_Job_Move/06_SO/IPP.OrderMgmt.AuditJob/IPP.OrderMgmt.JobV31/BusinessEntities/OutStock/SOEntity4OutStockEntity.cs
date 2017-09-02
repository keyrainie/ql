using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.OutStock
{
  public  class SOEntity4OutStockEntity
    {
      [DataMapping("SysNo", DbType.Int32)]
      public int SysNo { get; set; }

      [DataMapping("OrderDate", DbType.DateTime)]
      public DateTime OrderDate { get; set; }

      [DataMapping("LocalWHSysNo", DbType.String)]
      public string LocalWHSysNo { get; set; }
    }
}
