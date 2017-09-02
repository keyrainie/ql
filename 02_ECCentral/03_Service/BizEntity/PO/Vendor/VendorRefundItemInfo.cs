using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商退款单商品信息
    /// </summary>
    public class VendorRefundItemInfo
    {

        /// <summary>
        /// Item SysNo
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 退款单SysNo
        /// </summary>
        public int? RefundSysNo { get; set; }

        /// <summary>
        /// 单件号
        /// </summary>
        public int? RegisterSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundCash { get; set; }
    }
}
