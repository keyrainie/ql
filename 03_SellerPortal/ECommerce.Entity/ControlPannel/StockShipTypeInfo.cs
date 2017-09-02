using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class StockShipTypeInfo : EntityBase
    {
        public int SysNo { get; set; }
        public int StockSysNo { get; set; }
        public int ShipTypeSysNo { get; set; }
        public int Status { get; set; }
    }
}
