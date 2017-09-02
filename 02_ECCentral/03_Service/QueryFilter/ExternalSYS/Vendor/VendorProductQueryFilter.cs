using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class VendorProductQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? ManufacturerSysNo { get; set; }
        public int? VendorManufacturerSysNo { get; set; }
        public int? UserSysNo { get; set; }
        public bool IsMapping { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }
        public bool IsAuto { get; set; }
        public bool NotAuto
        {
            get
            {
                if (IsAuto)
                    return false;
                else
                    return true;

            }
        }
        public int? VendorSysNo { get; set; }
    }
}
