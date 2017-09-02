using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ProductPriceCompareAppService))]
    public class ProductPriceCompareAppService
    {
        private ProductPriceCompareProcessor _ProductPriceCompareAppService = ObjectFactory<ProductPriceCompareProcessor>.Instance;
        //价格举报有效
        public void UpdateProductPriceCompareValid(int sysNo)
        {
            _ProductPriceCompareAppService.UpdateProductPriceCompareValid(sysNo);
        }

        //价格举报无效
        public void UpdateProductPriceCompareInvalid(int sysNo, string commaSeperatedReasonCodes)
        {
            _ProductPriceCompareAppService.UpdateProductPriceCompareInvalid(sysNo, commaSeperatedReasonCodes);
        }

        //价格举报恢复
        public void UpdateProductPriceCompareResetLinkShow(int sysNo)
        {
            _ProductPriceCompareAppService.UpdateProductPriceCompareResetLinkShow(sysNo);
        }

        public List<CodeNamePair> GetInvalidReasons()
        {
            return _ProductPriceCompareAppService.GetInvalidReasons();
        }

        public ProductPriceCompareEntity Load(int sysNo)
        {
            return _ProductPriceCompareAppService.Load(sysNo);
        }
    }
}
