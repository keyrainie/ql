using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.BizEntity.Invoice;
using System.Data;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(ProductShiftDetailAppService))]
    public class ProductShiftDetailAppService
    {
        private ProductShiftProcessor processor = ObjectFactory<ProductShiftProcessor>.Instance;

        public int CreateProductShiftDetail(List<ProductShiftDetailQueryEntity> entity)
        {
            return processor.CreateProductShiftDetail(entity);
        }

        public bool ImportProductShiftDetail(string serverFilePath)
        {
            return processor.ImportProductShiftDetail(serverFilePath);
        }

    }
}
