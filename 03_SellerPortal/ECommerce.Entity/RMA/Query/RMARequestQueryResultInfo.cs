using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.RMA
{
    public class RMARequestQueryResultInfo
    {
        /// <summary>
        /// 申请编号
        /// </summary>
        public string SysNo { get; set; }

        /// <summary>
        /// 申请单ID
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 退换货类型
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string SOSysNo { get; set; }

        /// <summary>
        ///  客户编号
        /// </summary>
        public string CustomerSysNo { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceiveMan { get; set; }

        /// <summary>
        /// 申请单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public string RequestTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public string AuditTime { get; set; }
    }
}
