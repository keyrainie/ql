using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class ComplainQueryFilter
    {
        public ComplainQueryFilter() 
        {
            ValidCase = true;
        }

        public PagingInfo PagingInfo { get; set; }
        public string ComplainID { get; set; }

        public int? SOSysNo { get; set; }

        public string Subject { get; set; }

        public string ComplainType { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public SOComplainStatus? Status { get; set; }

        public string ComplainSourceType { get; set; }

        public string ResponsibleDept { get; set; }

        public string ResponsibleUser { get; set; }

        public string ProductID { get; set; }

        public int? DomainSysNo { get; set; }

        public string CustomerID { get; set; }

        public string CustomerIDSingle { get; set; }

        public OutdatedType? OutdatedTimeType { get; set; }

        public int? AssignerSysNo { get; set; }

        public DateTime? StartFollowUpdate { get; set; }

        public DateTime? EndFollowUpdate { get; set; }

        public bool ValidCase { get; set; }

        public bool OnlyCustomer { get; set; }

        public int? OperatorSysNo { get; set; }

        public DateTime? StartCompleteDate { set; get; }

        public DateTime? EndCompleteDate { set; get; }

        public string OrderType { get; set; }

        public bool IsReOpen { get; set; }

        public int? ReOpenCount { get; set; }

        public int? SpendHours { get; set; }

        public string Operator4SpendHours { get; set; }

        public string CompanyCode { get; set; }

        public int ? SystemNumber { get; set; }
    }
}
