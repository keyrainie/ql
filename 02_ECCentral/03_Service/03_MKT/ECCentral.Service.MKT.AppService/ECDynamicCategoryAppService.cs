using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ECDynamicCategoryAppService))]
    public class ECDynamicCategoryAppService
    {
        private ECDynamicCategoryProcessor processor = ObjectFactory<ECDynamicCategoryProcessor>.Instance;

        public ECDynamicCategory Create(ECDynamicCategory entity)
        {
            return processor.Create(entity);
        }

        public void Update(ECDynamicCategory entity)
        {
            processor.Update(entity);
        }

        public void Delete(int sysNo)
        {
            processor.Delete(sysNo);
        }

        public void InsertCategoryProductMapping(int dynamicCategorySysNo, List<int> productSysNoList)
        {
            processor.InsertCategoryProductMapping(dynamicCategorySysNo, productSysNoList);
        }

        public void DeleteCategoryProductMapping(int dynamicCategorySysNo, List<int> productSysNoList)
        {
            processor.DeleteCategoryProductMapping(dynamicCategorySysNo, productSysNoList);
        }

        public ECDynamicCategory GetCategoryTree(DynamicCategoryStatus? status, DynamicCategoryType? categoryType)
        {
            return processor.GetCategoryTree(status, categoryType);
        }
    }
}
