using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.MKT
{
    public class BusinessCooperationQueryFilter
    {
        public BusinessCooperationQueryFilter()
        {
            PagingInfo = new PagingInfo { PageIndex = 0, PageSize = 10 };
        }
        
        public int? GroupBuyingType { get; set; }
        public string VendorName { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public int? AreaSysNo { get; set; }

        public BusinessCooperationStatus? Status { get; set; }
        public DateTime? HandleDateFrom { get; set; }
        public DateTime? HandleDateTo { get; set; }
        public string Telephone { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }
    }
}
