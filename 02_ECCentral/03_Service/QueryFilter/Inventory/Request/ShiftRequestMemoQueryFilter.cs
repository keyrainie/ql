using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class ShiftRequestMemoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public int? RequestSysNo { get; set; }

        public ShiftRequestMemoStatus? MemoStatus { get; set; }

        public int? CreateUserSysNo { get; set; }

        public int? EditUserSysNo { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public string CompanyCode { get; set; }
        
    }
}
