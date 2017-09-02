using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Newegg.Oversea.Framework.Entity;


namespace IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities.Common
{
    public class SingleValueEntity
    {
        [DataMapping("Int32Value", DbType.Int32)]
        public int Int32Value { get; set; }

        [DataMapping("Int64Value", DbType.Int32)]
        public int Int64Value { get; set; }

        [DataMapping("StringValue", DbType.String)]
        public int StringValue { get; set; }
    }
}