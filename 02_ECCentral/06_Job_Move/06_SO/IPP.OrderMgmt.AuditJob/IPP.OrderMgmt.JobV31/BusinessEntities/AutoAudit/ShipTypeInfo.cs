using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class ShipTypeInfo
    {

        [DataMapping("ShipTypeDesc", DbType.String)]
        public string ShipTypeDesc { get; set; }

        [DataMapping("ShipTypeEnum", DbType.Int32)]
        public int ShipTypeEnum { get; set; }

        [DataMapping("ShipTypeID", DbType.String)]
        public string ShipTypeID { get; set; }

        [DataMapping("ShipTypeName", DbType.String)]
        public string ShipTypeName { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("DeliveryType", DbType.AnsiStringFixedLength)]
        public string DeliveryType { get; set; }

        [DataMapping("DeliveryPromise", DbType.AnsiStringFixedLength)]
        public string DeliveryPromise { get; set; }

        [DataMapping("IntervalDays", DbType.Int32)]
        public int IntervalDays { get; set; }

    }
}
