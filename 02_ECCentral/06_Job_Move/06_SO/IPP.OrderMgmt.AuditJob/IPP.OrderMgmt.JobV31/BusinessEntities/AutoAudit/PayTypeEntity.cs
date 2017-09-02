using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
   public class PayTypeEntity
    {
       [DataMapping("SysNo", DbType.Int32)]
       public int SysNo { get; set; }
       [DataMapping("IsNet", DbType.Int32)]
       public int IsNet { get; set; }
       [DataMapping("IsPayWhenRecv", DbType.Int32)]
       public int IsPayWhenRecv { get; set; }
       [DataMapping("IsOnlineShow", DbType.Int32)]
       public int IsOnlineShow { get; set; }
       [DataMapping("PayTypeName", DbType.String)]
       public string PayTypeName { get; set; }
    }
}
