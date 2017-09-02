using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class EIMSPayMessage : IEventMessage
    {
        public int AcctInvoiceNumber
        {
            get;
            set;
        }

        public int PayStatus
        {
            get;
            set;
        }

        public int InvoiceNumber
        {
            get;
            set;
        }

        public int InvoiceStatus
        {
            get;
            set;
        }

        public decimal ReceiveAmount
        {
            get;
            set;
        }

        public DateTime ReceiveDate
        {
            get;
            set;
        }

        public string PostUser
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
            get { return "EIMSPayMessage"; }
        }
    }
}