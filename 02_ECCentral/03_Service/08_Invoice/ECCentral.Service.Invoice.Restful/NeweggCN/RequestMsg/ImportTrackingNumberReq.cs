using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class ImportTrackingNumberReq
    {
        public string FileIdentity
        {
            get;
            set;
        }

        public int? StockSysNo
        {
            get;
            set;
        }
    }
}