using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvoiceMgmt.JobV31.BusinessEntities
{
    public class AdjustPointRequestInfo : EntityBase
    {

        public int? AccountValidScore { get; set; }

        public string ConfirmNote { get; set; }

        public DateTime? ConfirmTime { get; set; }

        public int ConfirmUserSysNo { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public int CustomerSysNo { get; set; }

        public DateTime? MaxPointExpiringDate { get; set; }

        public string Memo { get; set; }

        public string NewEggAccount { get; set; }

        public string Note { get; set; }

        public int? OptType { get; set; }

        public string OwnByDepartment { get; set; }

        public int? Point { get; set; }

        public DateTime? PointExpiringDate { get; set; }

        public int PointLogType { get; set; }

        public int? SOSysNo { get; set; }

        public int Status { get; set; }

        public int? ValidScore { get; set; }
    }
}
