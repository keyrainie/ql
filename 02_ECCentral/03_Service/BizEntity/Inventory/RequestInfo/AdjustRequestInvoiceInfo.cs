using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 损益单发票
    /// </summary>
    public class AdjustRequestInvoiceInfo : IIdentity
    {
        /// <summary>
        /// 发票系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiveName { get; set; }       

        /// <summary>
        /// 联系地址
        /// </summary>
        public string ContactAddress { get; set; }

        /// <summary>
        /// 收件地址
        /// </summary>
        public string ContactShippingAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

    }
}
