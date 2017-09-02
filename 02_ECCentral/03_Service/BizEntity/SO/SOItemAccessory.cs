using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    public class SOItemAccessory : IIdentity
    {
        public int? SOSysNo { get; set; }

        public int? PromotionSysNo { get; set; }

        public int? MasterProductSysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? Quantity { get; set; }

        public string Type { get; set; }

        public string WarehouseNumber { get; set; }



        public int? SysNo { get; set; }
        
    }
}
