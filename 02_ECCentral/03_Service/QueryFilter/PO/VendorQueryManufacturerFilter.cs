using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class VendorQueryManufacturerFilter
    {
        public VendorQueryManufacturerFilter()
        {
            PageInfo = new PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }

        public string ManufacturerID { get; set; }

        public string ManufacturerName { get; set; }

        public string BriefName { get; set; }

        public string Status { get; set; }

        public string CompanyCode { get; set; }
    }
}
