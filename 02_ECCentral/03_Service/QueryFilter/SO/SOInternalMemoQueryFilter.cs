using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SOInternalMemoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? CreateUserSysNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? CreateTimeBegin { get; set; }
        public DateTime? CreateTimeEnd { get; set; }
        public string Content { get; set; }
        public int? UpdateUserSysNo { get; set; }
        public DateTime? UpdateTime { get; set; }

        public SOInternalMemoStatus? Status { get; set; }
        public DateTime? RemindTime { get; set; }
        public string Note { get; set; }
        public int? CallType { get; set; }

        public int? CustomerSysNo { get; set; }

        public DateTime? UpdateTimeBegin { get; set; }
        public DateTime? UpdateTimeEnd { get; set; }
        public string ResponsibleDep { get; set; }

        public int? DepartmentCode { get; set; }
        public int? Importance { get; set; }

        public string LastEditUserName { get; set; }

        public int? SOSysNo { get; set; }

        public int? SysNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
