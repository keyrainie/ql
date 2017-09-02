using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ECommerceMgmt.ServiceJob.BusinessEntities
{
    public class SOEntity
    {
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime OrderDate { get; set; }
        [DataMapping("IncomeStatus", DbType.String)]
        public String IncomeStatus { get; set; }
        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }
        [DataMapping("PayStatus", DbType.String)]
        public String PayStatus { get; set; }      
    }
}
