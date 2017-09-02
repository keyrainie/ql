using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class CreateRefundNetPayTaskMessage : Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_RefundNetPayTask_Created"; }
        }
        /// <summary>
        /// [OverseaInvoiceReceiptManagement].[dbo].[RefundNetPayJobTask].[SysNo]
        /// </summary>
        public int TaskSysNo { get; set; } 
    }
}
