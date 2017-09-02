using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class CreateInvoiceSSBMessage : IEventMessage
    {
        int soSysNo;

        public int SOSysNo
        {
            get { return soSysNo; }
            set { soSysNo = value; }
        }
        int stockSysNo;

        public int StockSysNo
        {
            get { return stockSysNo; }
            set { stockSysNo = value; }
        }
        string invoiceNo;

        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }
        string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { companyCode = value; }
        }

        public string Subject
        {
            get { return "CreateInvoiceSSBMessage"; }
        }
    }
}
