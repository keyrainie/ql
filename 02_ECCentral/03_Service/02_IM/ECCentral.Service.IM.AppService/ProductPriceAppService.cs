using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductPriceAppService))]
    public class ProductPriceAppService
    {
        public void UpdateProductPriceInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.UpdateProductPriceInfo(productInfo);
        }

        public void AuditRequestProductPrice(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.AuditRequestProductPrice(productInfo);
        }

        public void CancelAuditProductPriceRequest(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceProcessor>.Instance.CancelAuditProductPriceRequest(productInfo);
        }

        public PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime)
        {
            return ObjectFactory<ProductPriceProcessor>.Instance.GetPriceChangeLogInfoByProductSysNo(productSysNo, startTime, endTime);
        }
    }
}
