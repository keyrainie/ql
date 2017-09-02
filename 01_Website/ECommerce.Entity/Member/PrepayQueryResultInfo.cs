using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class PrepayQueryResultInfo
    {
        public QueryResult<PrepayLogInfo> LogList { get; set; }
        public decimal PrepayAmount { get; set; }
    }

    public class PrepayLogInfo
    {
        public int CustomerID { get; set; }

        public PrepayLogType LogType { get; set; }

        public string Memo { get; set; }

        public string Note { get; set; }

        public decimal PrepayAmount { get; set; }

        public int OrderID { get; set; }

        public string TimeString { get; set; }
    }
}
