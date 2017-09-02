using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class StockQueryFilter : QueryFilter
    {
        public int MerchantSysNo { get; set; }
        public string StockID { get; set; }
        public int? SysNo { get; set; }
        public string StockName { get; set; }
        public StockStatus? Status { get; set; }
        public TradeType? StockType { get; set; }
        public bool ContainKJT { get; set; }
    }
}
