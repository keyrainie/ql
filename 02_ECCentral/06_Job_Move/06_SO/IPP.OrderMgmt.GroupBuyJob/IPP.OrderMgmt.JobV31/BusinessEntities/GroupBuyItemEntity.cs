using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class GroupBuyItemEntity
    {

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ReferenceSysNo", DbType.Int32)]
        public int ReferenceSysNo { get; set; }

        [DataMapping("SettlementStatus", DbType.AnsiStringFixedLength)]
        public string SettlementStatus { get; set; }

        [DataMapping("Type", DbType.AnsiStringFixedLength)]
        public string Type { get; set; }

        
    }
}
