using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck
{
  public  class SOEntity4CheckEntity
    {
      [DataMapping("SysNo", DbType.Int32)]
      public int SysNo { get; set; }

      [DataMapping("SOSysNo", DbType.Int32)]
      public int SOSysNo { get; set; }

      [DataMapping("ProductSysNo", DbType.Int32)]
      public int ProductSysNo { get; set; }

      [DataMapping("CustomerSysNo", DbType.Int32)]
      public int CustomerSysNo { get; set; }

      [DataMapping("CreateTime", DbType.DateTime)]
      public DateTime CreateTime { get; set; }

      [DataMapping("IPAddress", DbType.String)]
      public string IPAddress { get; set; }

      [DataMapping("ProductID", DbType.String)] //产品编号 如： 98-c05-004P
      public string ProductID { get; set; }

      [DataMapping("C3SysNo", DbType.Int32)]    //PC3类种类编号(Category3表 主键ID) 
      public int C3SysNo { get; set; }

    }
}
