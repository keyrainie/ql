using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductDomainAppService))]
    public class ProductDomainAppService
    {
        public virtual List<ProductDomain> LoadForListing(string companyCode)
        {
            return ObjectFactory<ProductDomainProcessor>.Instance.LoadForListing(companyCode);
        }

        public virtual List<ProductDepartmentCategory> LoadProductDepartmentCategorysByDomainSysNo(int productDomainSysNo)
        {
            return ObjectFactory<ProductDomainProcessor>.Instance.LoadProductDepartmentCategorysByDomainSysNo(productDomainSysNo);
        }

        public virtual ProductDomain Create(ProductDomain entity)
        {
            return ObjectFactory<ProductDomainProcessor>.Instance.Create(entity);
        }

        public virtual ProductDomain Update(ProductDomain entity)
        {
            return ObjectFactory<ProductDomainProcessor>.Instance.Update(entity);
        }

        public virtual void Delete(int sysNo, int? departmentCategorySysNo)
        {
            ObjectFactory<ProductDomainProcessor>.Instance.Delete(sysNo, departmentCategorySysNo);
        }

        public virtual ProductDepartmentCategory CreateDepartmentCategory(ProductDepartmentCategory entity)
        {
            return ObjectFactory<ProductDomainProcessor>.Instance.CreateDepartmentCategory(entity);
        }

        public virtual void UpdateDepartmentCategory(ProductDepartmentCategory entity)
        {
            ObjectFactory<ProductDomainProcessor>.Instance.UpdateDepartmentCategory(entity);
        }

        public virtual void DeleteProductDepartmentCategory(int sysNo)
        {
            ObjectFactory<ProductDomainProcessor>.Instance.DeleteProductDepartmentCategory(sysNo);
        }

        public virtual void BatchUpdatePM(int? domainSysNo, int pmSysNo, List<ProductDepartmentCategory> departmentCategoryList)
        {
            ObjectFactory<ProductDomainProcessor>.Instance.BatchUpdatePM(domainSysNo, pmSysNo, departmentCategoryList);
        }
    }
}
