using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using System.Data;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductDomainDA
    {
        ProductManagerInfo GetPMInfoByC3SysNoAndBrandSysNo(int c3SysNo, int brandSysNo);

        List<ProductDomain> LoadForListing(string companyCode);

        ProductDomain LoadBySysNo(int sysNo);

        List<ProductDepartmentCategory> LoadProductDepartmentCategorysByDomainSysNo(int productDomainSysNo);

        ProductDepartmentCategory LoadDepartmentCategoryBySysNo(int sysNo);

        List<ProductDepartmentCategory> GetExistingProductCategories(List<int> c2SysNoLsit, int productDomainSysNo);

        List<ProductDepartmentCategory> GetDepartmentCategories(List<int> departmentCategorySysNoList);

        ProductDomain Create(ProductDomain entity);

        ProductDomain Update(ProductDomain entity);

        void DeleteProductDomain(int sysNo);

        void DeleteDepartmentCategoryByDomainSysNo(int productDomainSysNo);

        void DeleteDepartmentCategoryBySysNo(int sysNo);

        void CreateDepartmentCategory(ProductDepartmentCategory category);

        void UpdateDepartmentCategory(ProductDepartmentCategory entity);

        void CreateProductDomainChangePool(int productDomainSysNo);

        void DeleteDepartmentMerchandiserByDomainSysNo(int productDomainSysNo);

        void CreateDepartmentMerchandiser(int productDomainSysNo, int merchandiserSysNo);

        bool ExistSameProductDomainName(string productDomainName, int sysNo, string companyCode);

        bool ExisingAnyProductNotContainedInTheRange(int c2SysNo, List<int> brandSysNoList);

        DataTable GetTheCountOfDepartmentCategoryRelatedInfo(ProductDepartmentCategory entity);

        void BatchUpdatePM(List<int> departmentCategorySysNoList, int pmSysNo);

        void BatchCreateDepartmentCategoriesForEmptyCategory(int domainSysNo, List<ProductDepartmentCategory> departmentCategoryList, int pmSysNo);
    }
}
