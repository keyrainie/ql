using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.Common
{
    public class SingleValueEntity
    {
        [DataMapping("Int32Value", DbType.Int32)]
        public int Int32Value { get; set; }

        [DataMapping("Int64Value", DbType.Int32)]
        public int Int64Value { get; set; }

        [DataMapping("StringValue", DbType.String)]
        public string StringValue { get; set; }

        [DataMapping("DateTimeValue", DbType.DateTime)]
        public DateTime DateTimeValue { get; set; }
    }
}