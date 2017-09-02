using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;

namespace ECCentral.Service.EventMessage
{
    public class InvoiceROMessage : InvoiceMessageBase, IEventMessage
    {

        public string Subject
        {
            get { return "InvoiceROMessage"; }
        }
    }

    public class InvoiceADJUSTMessage : InvoiceMessageBase, IEventMessage
    {

        public string Subject
        {
            get { return "InvoiceADJUSTMessage"; }
        }
    }

}
