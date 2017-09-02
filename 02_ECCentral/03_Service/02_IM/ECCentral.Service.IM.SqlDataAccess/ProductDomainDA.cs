using System.Collections.Generic;
using System.Data;
using System.Linq;

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductDomainDA))]
    public class ProductDomainDA : IProductDomainDA
    {
        public ProductManagerInfo GetPMInfoByC3SysNoAndBrandSysNo(int c3SysNo, int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetPMInfoByCategoryAndBrandSysNo");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            //cmd.SetParameterValue("@BrandSysNo", brandSysNo);
           
            var list = cmd.ExecuteEntityList<ProductDepartmentCategory>();
            int? pmSysNo = default(int?);
            var category = list.FirstOrDefault(p => p.BrandSysNo == brandSysNo);
            if (category != null)
            {
                pmSysNo = category.PMSysNo;
            }
            else
            {
                /*
                * 如果只配类别，品牌不配置的话，那么就代表这个类别和所有品牌全都配置这个PM
                */
                category = list.FirstOrDefault(p => p.BrandSysNo == null);
                if (category != null)
                {
                    pmSysNo = category.PMSysNo;
                }
            }
            if (pmSysNo != null)
            {
                return ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(pmSysNo.Value);
            }
            return null;
        }

        public List<ProductDomain> LoadForListing(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetProductDomainForListing");
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntityList<ProductDomain>();
        }

        public ProductDomain LoadBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetProductDomain");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<ProductDomain>();
        }

        public List<ProductDepartmentCategory> LoadProductDepartmentCategorysByDomainSysNo(int productDomainSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetProductDepartment_CategoryList");
            cmd.SetParameterValue("@ProductDomainSysNoList", productDomainSysNo);

            return cmd.ExecuteEntityList<ProductDepartmentCategory>();
        }

        public ProductDepartmentCategory LoadDepartmentCategoryBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetDepartmentCategory");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<ProductDepartmentCategory>();
        }

        public List<ProductDepartmentCategory> GetExistingProductCategories(List<int> c2SysNoLsit, int productDomainSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetExistingProductCategories");
            cmd.SetParameterValue("@C2SysNoLsit", c2SysNoLsit.Join(","));
            cmd.SetParameterValue("@ProductDomainSysNo", productDomainSysNo);

            return cmd.ExecuteEntityList<ProductDepartmentCategory>();
        }

        public List<ProductDepartmentCategory> GetDepartmentCategories(List<int> departmentCategorySysNoList)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetDepartmentCategories");

            cmd.SetParameterValue("@departmentCategorySysNos", departmentCategorySysNoList.Join(","));

            return cmd.ExecuteEntityList<ProductDepartmentCategory>();
        }

        public ProductDomain Create(ProductDomain entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_CreateProductDomain");

            cmd.SetParameterValue<ProductDomain>(entity);

            cmd.ExecuteNonQuery();

            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");

            return entity;
        }

        public void CreateProductDomainChangePool(int productDomainSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_CreateProductDomainChangePool");
            cmd.SetParameterValue("@ProductDomainSysNo", productDomainSysNo);

            cmd.ExecuteNonQuery();
        }

        public ProductDomain Update(ProductDomain entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_UpdateProductDomain");

            cmd.SetParameterValue<ProductDomain>(entity);

            cmd.ExecuteNonQuery();            

            return entity;
        }

        public void DeleteProductDomain(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_DeleteProductDomain");
            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        public void DeleteDepartmentCategoryBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_DeleteDepartmentCateogriesBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        public void DeleteDepartmentCategoryByDomainSysNo(int productDomainSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_DeleteDepartmentCateogriesByDomainSysNo");
            cmd.SetParameterValue("@ProductDomainSysNo", productDomainSysNo);

            cmd.ExecuteNonQuery();
        }

        public void CreateDepartmentCategory(ProductDepartmentCategory category)
        {
            var c = DataCommandManager.GetDataCommand("ProductDomain_CreateDepartmentCategory");
           
            c.SetParameterValue<ProductDepartmentCategory>(category);
            var sysNo = c.ExecuteScalar();
            if (sysNo is System.DBNull || sysNo == null)
            {
                return;
            }
            category.SysNo = System.Convert.ToInt32(sysNo);
        }

        public void UpdateDepartmentCategory(ProductDepartmentCategory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_UpdateDepartmentCategory");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@C2SysNo", entity.C2SysNo);
            cmd.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            cmd.SetParameterValue("@PMSysNo", entity.PMSysNo);

            cmd.ExecuteNonQuery();
        }

        public void DeleteDepartmentMerchandiserByDomainSysNo(int productDomainSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_DeleteDepartmentMerchandisersByDomainSysNo");
            cmd.SetParameterValue("@ProductDomainSysNo", productDomainSysNo);
            cmd.ExecuteNonQuery();
        }

        public void CreateDepartmentMerchandiser(int productDomainSysNo, int merchandiserSysNo)
        {
            var c = DataCommandManager.GetDataCommand("ProductDomain_CreateMerchandiser");
            c.SetParameterValue("@ProductDomainSysNo", productDomainSysNo);
            c.SetParameterValue("@MerchandiserSysNo", merchandiserSysNo);

            c.ExecuteNonQuery();
        }

        public bool ExistSameProductDomainName(string productDomainName, int sysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_HasSameProductDomainName");
            cmd.SetParameterValue("@ProductDomainName", productDomainName);
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool ExisingAnyProductNotContainedInTheRange(int c2SysNo, List<int> brandSysNoList)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_IsExisingAnyProductNotContainedInTheRange");
            cmd.SetParameterValue("@C2SysNo", c2SysNo);
            cmd.SetParameterValue("@BrandSysNos", brandSysNoList.Join(","));

            return cmd.ExecuteScalar<int>() > 0;
        }

        public DataTable GetTheCountOfDepartmentCategoryRelatedInfo(ProductDepartmentCategory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_GetTheCountOfDepartmentCategoryReltedInfo");
            cmd.SetParameterValue("@PMSysNo", entity.PMSysNo);
            cmd.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            cmd.SetParameterValue("@C2SysNo", entity.C2SysNo);

            return cmd.ExecuteDataSet().Tables[0];
        }

        public void BatchUpdatePM(List<int> departmentCategorySysNoList, int pmSysNo)
        {           
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_BatchUpdatePM");

            cmd.SetParameterValue("@DepartmentCategorySysNos", departmentCategorySysNoList.Join(","));
            cmd.SetParameterValue("@PMSysNo", pmSysNo);

            cmd.ExecuteNonQuery();
        }

        public void BatchCreateDepartmentCategoriesForEmptyCategory(int domainSysNo, List<ProductDepartmentCategory> departmentCategoryList, int pmSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductDomain_BatchCreateDepartmentCategoriesForEmptyCategory");
            var c2SysNolist = departmentCategoryList.Select(e => e.C2SysNo).Join(",");

            cmd.SetParameterValue("@DomainSysNo", domainSysNo);
            cmd.SetParameterValue("@PMSysNo", pmSysNo);
            cmd.SetParameterValue("@C2SysNoList", c2SysNolist);

            cmd.ExecuteNonQuery();
        }
    }
}
