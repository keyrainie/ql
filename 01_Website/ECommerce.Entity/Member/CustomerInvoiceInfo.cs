using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 用户发票信息
    /// </summary>
    public class CustomerInvoiceInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        public bool NeedInvoice { get; set; }
    }
}
