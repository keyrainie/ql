using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.BusinessEntities
{
    [Serializable]
    public class SOMasterEntity : DetailBaseEntity
    {
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int ShipTypeSysNo { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo { get; set; }

        [DataMapping("SOAmt", DbType.Decimal)]
        public decimal SOAmt { get; set; }

        [DataMapping("DiscountAmt", DbType.Decimal)]
        public decimal DiscountAmt { get; set; }

        [DataMapping("CashPay", DbType.Decimal)]
        public decimal CashPay { get; set; }

        [DataMapping("PointPay", DbType.Int32)]
        public int PointPay { get; set; }

        [DataMapping("AgentSysNo", DbType.Int32)]
        public int AgentSysNo { get; set; }

        [DataMapping("PointLogType", DbType.Int32)]
        public int PointLogType { get; set; }

        [DataMapping("SOType", DbType.Int32)]
        public int SOType { get; set; }    
        
    }
}
