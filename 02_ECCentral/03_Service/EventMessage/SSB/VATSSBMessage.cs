using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class ImportVATSSBMessage : IEventMessage
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
        public ImportVATOrderType OrderType
        { get; set; }

        public string Subject
        {
            get { return "ImportVATSSBMessage"; }
        }
    }

    public enum ImportVATOrderType
    {
        SO
    }
}
