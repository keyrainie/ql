using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class EIMSCancelPayMessage : IEventMessage
    {
        public int AcctinvoiceNumber
        {
            get;
            set;
        }

        public int EIMSInvoiceNumber
        {
            get;
            set;
        }

        public int InvoiceStatus
        {
            get;
            set;
        }

        public int PayStatus
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string UserID
        {
            get;
            set;
        }

        public string Subject
        {
            get { return "EIMSCancelPayMessage"; }
        }
    }
}