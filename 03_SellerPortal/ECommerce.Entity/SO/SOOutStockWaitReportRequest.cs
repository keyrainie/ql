using ECommerce.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    public class SOOutStockWaitReportRequest
    {
        public int SOSysNo { get; set; }        
        public LoginUser User { get; set; }
        public LogisticsInfo Logistics { get; set; }
    }

    public class LogisticsInfo
    {
        public string ShipTypeName { get; set; }
        public string TrackingNumber { get; set; }
    }

}
