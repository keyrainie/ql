using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ThirdPart.JobV31.BusinessEntities.IngramMicro
{
    public class InternalMemoEntity
    {
        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }

        [DataMapping("DisplayName", DbType.String)]
        public string UserName { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }
    }
}
