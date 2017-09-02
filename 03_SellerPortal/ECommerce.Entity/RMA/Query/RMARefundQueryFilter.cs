using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.RMA
{
    public class RMARefundQueryFilter : QueryFilter
    {
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RefundID { get; set; }

        /// <summary>
        /// 退款单开始日期
        /// </summary>
        public string CreateDateFrom { get; set; }

        /// <summary>
        /// 退款单结束日期
        /// </summary>
        public string CreateDateTo { get; set; }

        /// <summary>
        ///  顾客帐号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string SOSysNo { get; set; }

        /// <summary>
        /// 退款单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int SellerSysNo { get; set; }
    }
}
