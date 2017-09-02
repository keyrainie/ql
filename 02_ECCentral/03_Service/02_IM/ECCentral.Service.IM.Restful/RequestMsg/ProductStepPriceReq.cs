using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class ProductStepPriceReq
    {
        public PagingInfo PagingInfo { get; set; }

        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

    }
}
