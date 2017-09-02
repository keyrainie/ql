using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.EIMS
{
    public class EIMSInvoiceInfoForConsignMessage : IEventMessage
    {
        public int InvoiceNumber { get; set; }
        public int ReceiveType { get; set; }
        public string CompanyCode { get; set; }

        public bool IsError { get; set; }
        public string PM { get; set; }
        public decimal RemnantReturnPoint { get; set; }
        public int SysNo { get; set; }
        public string ReturnPointName { get; set; }
        public int VendorSysNo { get; set; }

        public string Subject
        {
            get { return "EIMSInvoiceInfoForConsignMessage"; }
        }
    }
}
