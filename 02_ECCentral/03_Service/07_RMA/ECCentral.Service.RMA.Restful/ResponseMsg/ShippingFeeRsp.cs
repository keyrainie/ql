using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.RMA.Restful.ResponseMsg
{
    public class ShippingFeeRsp
    {         
        public decimal? TotalAmt { get; set; }
        public decimal? PremiumAmt { get; set; }
        public decimal? ShippingCharge { get; set; }
        public decimal? PayPrice { get; set; }
        public decimal? HistoryRefund { get; set; }
    }
}
