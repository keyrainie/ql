using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.RMA
{
    public class RMARefundQueryResultInfo
    {
        public string RefundSysNo { get; set; }

        public string RefundID { get; set; }

        public string Status { get; set; }

        public string SOSysNo { get; set; }

        public string CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        public string RefundAmt { get; set; }

        public string ReceiveMan { get; set; }

        public string BankName { get; set; }

        public string BankCardNo { get; set; }

        public string CreateTime { get; set; }

    }
}
