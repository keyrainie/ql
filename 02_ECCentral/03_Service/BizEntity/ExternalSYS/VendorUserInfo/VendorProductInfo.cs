using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class VendorProductInfo
    {
        public int SysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductMode { get; set; }

        public int C3SysNo { get; set; }

        public int? ManufacturerSysNo { get; set; }

        public int Status { get; set; }

        public string VendorName { get; set; }
    }
}
