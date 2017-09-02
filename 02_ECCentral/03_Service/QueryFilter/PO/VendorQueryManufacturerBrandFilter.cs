using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class VendorQueryManufacturerBrandFilter
    {
        public VendorQueryManufacturerBrandFilter()
        {
            PageInfo = new PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }

        public string BrandName_CH { get; set; }

        public string BrandName_EN { get; set; }

        public string BrandName { get; set; }

        public string Status { get; set; }

        public int? ManufacturerSysNo { get; set; }

        public string ManufacturerName { get; set; }

        public string CompanyCode { get; set; }
    }
}
