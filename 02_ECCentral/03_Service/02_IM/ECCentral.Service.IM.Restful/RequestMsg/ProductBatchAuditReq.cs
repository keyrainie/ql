using ECCentral.BizEntity.IM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class ProductBatchAuditReq
    {
        public List<int> ProductSysNo { set; get; }
        public ProductStatus Status { set; get; }
    }
}
