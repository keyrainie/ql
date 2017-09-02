using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.RMA.Restful.ResponseMsg
{
    public class RefundDetailInfoRsp
    {
        public RefundInfo RefundInfo { get; set; }
        public string CustomerName { get; set; }
        public CustomerContactInfo CustomerContact { get; set; }
        public PromotionCode_Customer_Log CouponCodeLog { get; set; }
    }
}
