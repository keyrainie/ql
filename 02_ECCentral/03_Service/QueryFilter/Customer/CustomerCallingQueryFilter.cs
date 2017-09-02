using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerCallingQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }
        public int? WebChannel { get; set; }
        public int? SystemNumber { get; set; }
        public string OrderSysNo { get; set; }
        public int? LastUpdateUserSysNo { get; set; }
        public string CustomerName { get; set; }
        public string PhoneORCellphone { get; set; }
        public string CustomerID { get; set; }
        public string Address { get; set; }
        public bool IsReopen { get; set; }
        public int? ReopenCount { get; set; }
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? CloseDateFrom { get; set; }
        public DateTime? CloseDateTo { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishDateFrom { get; set; }
        public DateTime? FinishDateTo { get; set; }
        public SOComplainStatus? ComplainStatus { get; set; }
        public CallingRMAStatus? RMAStatus { get; set; }
        public SOStatus? SOStatus { get; set; }
        public CallsEventsStatus? CallingStatus { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 来电次数比较符
        /// </summary>
        public OperationSignType? OperaterCallingTimes { get; set; }
        public int? CallingTimes { get; set; }
        /// <summary>
        /// 来电时数比较符
        /// </summary>
        public int? OperaterCallingHours { get; set; }
        public int? CallingHours { get; set; }

        public int? CallReason { get; set; }
        public string LogTitle { get; set; }


    }

    public class CustomerCallsEventLogFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }
        public int? WebChannel { get; set; }
        public int? CallsEventsSysNo { get; set; }
    }
}
