using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
//using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using System.Data;
using System.Transactions;


namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(RepeatPromotionAppService))]
    public class RepeatPromotionAppService
    {
        public ProductInfo GetProductInfo(string productId)
        {
            var product = ExternalDomainBroker.GetProductInfo(productId);
            return product;
        }


    }
}
