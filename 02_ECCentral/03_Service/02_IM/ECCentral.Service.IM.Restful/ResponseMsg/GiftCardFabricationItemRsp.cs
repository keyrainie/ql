using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.IM.Restful
{
    public class GiftCardFabricationItemRsp
    {
        public List<ECCentral.BizEntity.IM.GiftCardFabrication> GiftCardFabricationList { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal TotalCount { get; set; }
    }
}
