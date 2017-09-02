using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class SMSTypeInfo
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int ShipTypeSysNo { get; set; }

        [DataMapping("SMSType", DbType.Int32)]
        public int SMSType { get; set; }

        [DataMapping("SMSContent", DbType.String)]
        public string SMSContent { get; set; }

    }
}
