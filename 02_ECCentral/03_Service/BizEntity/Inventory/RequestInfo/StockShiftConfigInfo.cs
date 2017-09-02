using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity;

namespace ECCentral.BizEntity.Inventory
{
    public class StockShiftConfigInfo : IIdentity
    {
        public int? SysNo
        {
            get;
            set;
        }
        public int OutStockSysNo { get; set; }

        public int InStockSysNo { get; set; }

        public int SPLInterval { get; set; }

        public int ShipInterval { get; set; }

        public string ShiftType { get; set; }

        public string CompanyCode { get; set; }
    }
}
