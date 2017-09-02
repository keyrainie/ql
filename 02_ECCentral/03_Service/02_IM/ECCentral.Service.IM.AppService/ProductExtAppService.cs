using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductExtAppService))]
    public class ProductExtAppService
    {
        public  void UpdatePermitRefund(List<ProductExtInfo> list)
        {
             ObjectFactory<ProductExtProcessor>.Instance.UpdatePermitRefund(list);
        }

        public ProductBatchManagementInfo UpdateIsBatch(ProductBatchManagementInfo extInfo)
        {
            return ObjectFactory<ProductExtProcessor>.Instance.UpdateIsBatch(extInfo);
        }
    }
}
