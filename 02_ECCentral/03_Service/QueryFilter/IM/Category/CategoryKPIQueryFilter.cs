using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.IM
{
    public class CategoryKPIQueryFilter
    {
        public int? C1SysNo { get; set; }

        public int? C2SysNo { get; set; }

        public int? C3SysNo { get; set; }

        //public int PMUserSysNo { get; set; }

        public int Status { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public CategoryType CategoryType
        {
            get;
            set;
        }
    }
}
