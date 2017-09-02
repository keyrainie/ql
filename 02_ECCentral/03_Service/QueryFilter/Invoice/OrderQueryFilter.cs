using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.QueryFilter.Invoice
{
    public class OrderQueryFilter
    {
        public SOIncomeOrderType OrderType
        {
            get;
            set;
        }

        public string OrderSysNo
        {
            get;
            set;
        }
    }
}