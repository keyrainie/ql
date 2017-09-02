using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ECDynamicCategoryQueryFilter
    {
        public string CompanyCode { get; set; }
        public DynamicCategoryStatus? Status { get; set; }
        public DynamicCategoryType? CategoryType { get; set; }
    }

    public class ECDynamicCategoryMappingQueryFilter
    {
        public int? DynamicCategorySysNo { get; set; }
        public string CompanyCode { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

    public class ECCategoryMappingQueryFilter
    {
        public int? ECCategorySysNo { get; set; }
        public string CompanyCode { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
