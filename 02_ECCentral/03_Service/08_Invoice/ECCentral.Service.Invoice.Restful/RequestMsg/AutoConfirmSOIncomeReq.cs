using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class AutoConfirmSOIncomeReq
    {
        public string FileIdentity
        {
            get;
            set;
        }

        public DateTime? SOOutFromDate
        {
            get;
            set;
        }

        public DateTime? SOOutToDate
        {
            get;
            set;
        }
    }
}