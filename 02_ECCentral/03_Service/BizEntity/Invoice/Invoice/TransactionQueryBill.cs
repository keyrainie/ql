using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Invoice
{
    public class TransactionQueryBill
    {

        public TransactionQueryBill()
        {
            IsTrue = false;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public string PayAmount { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public string TrxState { get; set; }

        /// <summary>
        /// 支付平台处理时间
        /// </summary>
        public string RdoTime { get; set; }

        /// <summary>
        /// 查询是否成功
        /// </summary>
        public bool IsTrue { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string Message { get; set; }
    }
}
