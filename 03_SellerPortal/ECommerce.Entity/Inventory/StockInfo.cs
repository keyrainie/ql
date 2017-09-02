using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Inventory
{
    public class StockInfo
    {
        public int SysNo { get; set; }
        public string StockID { get; set; }
        public string StockName { get; set; }
        public ValidStatus Status { get; set; }
        public int WebChannelSysNo { get; set; }
        public int WarehouseSysNo { get; set; }
    }
}
