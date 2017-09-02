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
    public class RmaRefundEntity : DetailBaseEntity
    {
        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CashAmt", DbType.Decimal)]
        public decimal CashAmt { get; set; }

        [DataMapping("PointAmt", DbType.Int32)]
        public int PointAmt { get; set; }
    }
}
