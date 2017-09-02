using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductRmaPolicyAppService))]
  public  class ProductRmaPolicyAppService
    {
        public ProductRMAPolicyInfo GetProductRMAPolicyByProductSysNo(int? productSysNo)
        {
            return ObjectFactory<ProductRMAPolicyProcessor>.Instance.GetProductRMAPolicyByProductSysNo(productSysNo);
        }

        public void CreateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            ObjectFactory<ProductRMAPolicyProcessor>.Instance.CreateProductRMAPolicy(info);
        }


        public void UpdateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            ObjectFactory<ProductRMAPolicyProcessor>.Instance.UpdateProductRMAPolicy(info);
        }
    }
}
