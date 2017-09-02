using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerPointLogQueryFilter 
    {
        public PagingInfo PagingInfo { get; set; }

        public bool IsUseCreateDate { get; set; }

        public DateTime? CreateTimeFrom { get; set; }

        public DateTime? CreateTimeTo { get; set; }

        public int? CustomerSysNo { get; set; }

        public YNStatus? IsCashPoint { get; set; }

        public int? OrderSysNo { get; set; }

        public int? PointType { get; set; }

        /// <summary>
        /// -1:消费记录；1:获取记录
        /// </summary>
        public int? ResultType { get; set; }

        public string Status { get; set; }

    }
}
