using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.MKT.BizProcessor.Promotion;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(ProductPayTypeAppService))]
    public class ProductPayTypeAppService
    {
        private readonly ProductPayTypeProcessor _productPayTypeprocessor = ObjectFactory<ProductPayTypeProcessor>.Instance;


        public void BatchCreateProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            _productPayTypeprocessor.BatchCreateProductPayType(productPayTypeInfo);
        }

        public void BathAbortProductPayType(string sysNo, string editUser)
        {
            _productPayTypeprocessor.BathAbortProductPayType(sysNo, editUser);
        }
    }
}
