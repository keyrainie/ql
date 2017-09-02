using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class PayItemDetailInfoReq
    {
        public int? PaySysNo
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }

        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}