using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductLineAppService))]
    public class ProductLineAppService
    {
        public ProductLineInfo LoadBySysNo(int sysno)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.LoadBySysNo(sysno);
        }
        public ProductLineInfo Create(ProductLineInfo entity)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.Create(entity);
        }
        public ProductLineInfo Update(ProductLineInfo entity)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.Update(entity);
        }
        public void Delete(int sysno)
        {
            ObjectFactory<ProductLineProcessor>.Instance.Delete(sysno);
        }
        public bool HasRightByPMUser(ProductLineInfo entity) 
        {
            return ObjectFactory<ProductLineProcessor>.Instance.HasRightByPMUser(entity);
        }
        public void BatchUpdate(BatchUpdatePMEntity entity)
        {
            ObjectFactory<ProductLineProcessor>.Instance.BatchUpdate(entity);
        }
    }
}
