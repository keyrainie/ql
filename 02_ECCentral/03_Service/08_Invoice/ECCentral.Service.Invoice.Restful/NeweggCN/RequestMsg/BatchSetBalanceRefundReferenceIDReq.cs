using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class BatchSetBalanceRefundReferenceIDReq
    {
        public List<int> SysNoList
        {
            get;
            set;
        }

        public string ReferenceID
        {
            get;
            set;
        }
    }
}