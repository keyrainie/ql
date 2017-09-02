using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class CommissionRuleTemplateQueryFilter
    {

        public CommissionRuleTemplateQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }
        public int? C3SysNo { get; set; }
        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public CommissionRuleStatus? Status { get; set; }
        public int? BrandSysNo { get; set; }

    }
}
