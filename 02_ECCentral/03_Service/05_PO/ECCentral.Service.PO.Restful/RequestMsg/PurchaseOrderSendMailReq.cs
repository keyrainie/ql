using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.Restful.RequestMsg
{
    public class PurchaseOrderSendMailReq
    {
        public string MailContent { get; set; }
        public int? PurchaseOrderSysNo { get; set; }
        public string MailAddress { get; set; }
    }
}
