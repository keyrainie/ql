using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    public class NetPayResp
    {
        public NetPayInfo NetPay
        {
            get;
            set;
        }

        public SOIncomeRefundInfo Refund
        {
            get;
            set;
        }

        public SOBaseInfo SOBaseInfo
        {
            get;
            set;
        }
    }
}