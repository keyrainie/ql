using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    public class RMAInventoryLog
    {        
        public string Memo { get; set; }

        public DateTime? OperationTime { get; set; }

        public string OperationType { get; set; }

        public int? OwnbyCustomerQty { get; set; }

        public int? OwnbyNeweggQty { get; set; }

        public int? ProductSysNo { get; set; }

        public int? RegisterSysNo { get; set; }

        public int? RMAOnVendorQty { get; set; }

        public int? RMAStockQty { get; set; }

        public int? ShiftQty { get; set; }

        public int? SysNo { get; set; }

        public int? WarehouseSysNo { get; set; }
    }
}
