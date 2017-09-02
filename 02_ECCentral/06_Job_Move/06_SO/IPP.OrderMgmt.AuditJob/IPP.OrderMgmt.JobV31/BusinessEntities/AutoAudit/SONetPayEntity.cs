using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class SONetPayEntity
    {

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo { get; set; }

        [DataMapping("PayAmount", DbType.Decimal)]
        public decimal PayAmount { get; set; }

        [DataMapping("Source", DbType.Int32)]
        public int Source { get; set; }

        [DataMapping("InputTime", DbType.DateTime)]
        public DateTime? InputTime { get; set; }

        [DataMapping("InputUserSysNo", DbType.Int32)]
        public int InputUserSysNo { get; set; }

        [DataMapping("ApproveUserSysNo", DbType.Int32)]
        public int ApproveUserSysNo { get; set; }

        [DataMapping("ApproveTime", DbType.DateTime)]
        public DateTime? ApproveTime { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        public bool isReTry { get; set; }
    }
}