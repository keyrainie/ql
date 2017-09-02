using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.WMS
{
    public class PurchaseOrderCancelVerifyMessage : IEventMessage
    {
        public string PONumber { get; set; }
        public string CompanyCode { get; set; }

        public string Subject
        {
            get { return "PurchaseOrderCancelVerifyMessage"; }
        }
    }
}
