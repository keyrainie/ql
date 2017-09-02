using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductDomainProcessor))]
    public class ProductDomainProcessor
    {
        private IProductDomainDA da = ObjectFactory<IProductDomainDA>.Instance;

        public ProductManagerInfo GetPMInfoByC3SysNoAndBrandSysNo(int c3SysNo, int brandSysNo)
        {
            return da.GetPMInfoByC3SysNoAndBrandSysNo(c3SysNo, brandSysNo);
        }

        public virtual ProductDomain LoadBySysNo(int sysNo)
        {
            return da.LoadBySysNo(sysNo);
        }

        public virtual List<ProductDomain> LoadForListing(string companyCode)
        {
            return da.LoadForListing(companyCode);
        }

        public virtual List<ProductDepartmentCategory> LoadProductDepartmentCategorysByDomainSysNo(int productDomainSysNo)
        {
            var list = da.LoadProductDepartmentCategorysByDomainSysNo(productDomainSysNo);
            var domain = LoadBySysNo(productDomainSysNo);
            if (list != null)
            {
                list.ForEach(p =>
                {                   
                    p.BackupUserList= ObjectFactory<IProductDomainQueryDA>.Instance.GetUserNameList(p.BackupUserList, domain.CompanyCode);
                });
            }
            return list;
        }

        public virtual ProductDomain Create(ProductDomain entity)
        {
            PreCheckProductDomain(entity);

            entity.Status = ValidStatus.Active;

            using (TransactionScope scope = new TransactionScope())
            {
                da.Create(entity);

                entity.DepartmentMerchandiserSysNoList.ForEach(p =>
                {
                    da.CreateDepartmentMerchandiser(entity.SysNo.Value, p.Value);
                });

                da.CreateProductDomainChangePool(entity.SysNo.Value);

                scope.Complete();
            }

            return entity;
        }

        public virtual ProductDomain Update(ProductDomain entity)
        {
            PreCheckProductDomain(entity);

            using (TransactionScope scope = new TransactionScope())
            {
                da.Update(entity);

                da.DeleteDepartmentMerchandiserByDomainSysNo(entity.SysNo.Value);

                entity.DepartmentMerchandiserSysNoList.ForEach(p =>
                {
                    da.CreateDepartmentMerchandiser(entity.SysNo.Value, p.Value);
                });

                da.CreateProductDomainChangePool(entity.SysNo.Value);

                scope.Complete();
            }

            return entity;
        }

        /// <summary>
        /// 根据系统编号和DepartmentCategorySysNo删除ProductDomain
        /// (如果该Domain下只有一个Category或为空，则删除Domain的所有信息,否则只删除Category信息)
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="departmentCategorySysNo"></param>
        public virtual void Delete(int sysNo, int? departmentCategorySysNo)
        {
            var domain = da.LoadBySysNo(sysNo);
            if (domain == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainDeleteResult"));
            }
            //如果不是空的分类，则需要PreCheck
            if (departmentCategorySysNo.HasValue)
            {
                var departmentCategory = da.LoadDepartmentCategoryBySysNo(departmentCategorySysNo.Value);

                var restCategories = da.GetExistingProductCategories(new List<int> { departmentCategory.C2SysNo.Value, }, 0);
                restCategories = restCategories.Where(e => e.SysNo != departmentCategory.SysNo).ToList();

                //如果会导致原分类丢失PM
                ValidateDepartmentCategories(restCategories, departmentCategory.C2SysNo.Value);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                List<ProductDepartmentCategory> categoryList = LoadProductDepartmentCategorysByDomainSysNo(sysNo);

                //如果Domain下只有一个Category则删除Domain的相关所有信息
                if (categoryList.Count <= 1)
                {
                    da.DeleteDepartmentCategoryByDomainSysNo(sysNo);

                    da.DeleteDepartmentMerchandiserByDomainSysNo(sysNo);

                    da.DeleteProductDomain(sysNo);
                }
                else
                {
                    da.DeleteDepartmentCategoryBySysNo(departmentCategorySysNo.Value);
                }

                scope.Complete();
            }
        }

        public virtual ProductDepartmentCategory CreateDepartmentCategory(ProductDepartmentCategory entity)
        {
            ValidateProductDepartmentCategory(entity);

            var repeatedCategories = da.GetExistingProductCategories(new List<int> { entity.C2SysNo.Value, }, 0);
            //指定的二级类是否在其他Domain存在
            if (repeatedCategories.Any(e => e.ProductDomainSysNo != entity.ProductDomainSysNo && e.C2SysNo == entity.C2SysNo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainCreateDepartmentCategory1"));
            }
            //是否会导致新分类重复
            if (repeatedCategories.Any(e => e.C2SysNo == entity.C2SysNo && e.BrandSysNo == entity.BrandSysNo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainCreateDepartmentCategory2"));
            }
            
            using (TransactionScope scope = new TransactionScope())
            {
                da.CreateDepartmentCategory(entity);

                da.CreateProductDomainChangePool(entity.ProductDomainSysNo.Value);

                scope.Complete();
            }

            return entity;
        }

        public virtual void UpdateDepartmentCategory(ProductDepartmentCategory entity)
        {
            ValidateProductDepartmentCategory(entity);
            ValidateUpdateProductDepartmentCateogry(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                da.UpdateDepartmentCategory(entity);

                da.CreateProductDomainChangePool(entity.ProductDomainSysNo.Value);

                scope.Complete();
            }
        }

        /// <summary>
        /// 删除DepartmentCategory信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        public virtual void DeleteProductDepartmentCategory(int sysNo)
        {
            var departmentCategory = da.LoadDepartmentCategoryBySysNo(sysNo);

            if (departmentCategory == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainDeleteProductDepartmentCategoryResult"));
            }
            var restCategories = da.GetExistingProductCategories(new List<int> { departmentCategory.C2SysNo.Value, }, 0);
            restCategories = restCategories.Where(e => e.SysNo != departmentCategory.SysNo).ToList();

            //如果会导致原分类丢失PM
            ValidateDepartmentCategories(restCategories, departmentCategory.C2SysNo.Value);

            da.DeleteDepartmentCategoryBySysNo(sysNo);
        }

        /// <summary>
        /// 批量更新PM
        /// </summary>
        /// <param name="pmSysNo">PM系统编号</param>
        /// <param name="DepartmentCategoryList">分类列表</param>
        /// <param name="isForEmptyCategory">是否为空二级类创建ProductDepartment</param>
        public virtual void BatchUpdatePM(int? domainSysNo, int pmSysNo, List<ProductDepartmentCategory> departmentCategoryList)
        {
            if (departmentCategoryList == null || departmentCategoryList.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM1"));
            }
            bool flag = ObjectFactory<IProductManagerDA>.Instance.IsExistUserSysNo(pmSysNo);
            if (!flag)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM2"));
            }
            //为空二级类创建ProductDepartment
            if ((domainSysNo ?? 0) > 0)
            {
                var existsingDepartmentCategories = da.GetExistingProductCategories(departmentCategoryList.Select(e => e.C2SysNo.Value).ToList(), 0);
                if (existsingDepartmentCategories.Any())
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM3") + existsingDepartmentCategories.Select(e => e.C2Name).Join("、") + ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM4"));
                }

                var domain = LoadBySysNo(domainSysNo.Value);
                if (domain == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM5"));
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    da.BatchCreateDepartmentCategoriesForEmptyCategory(domainSysNo.Value, departmentCategoryList, pmSysNo);

                    da.CreateProductDomainChangePool(domainSysNo.Value);

                    scope.Complete();
                }
                return;
            }

            //批量修改现有DepartmentCategory的PM
            var departmentCategorySysNoList = departmentCategoryList.Select(e => e.SysNo ?? 0).ToList();
            var departmentCategories = da.GetDepartmentCategories(departmentCategorySysNoList);
            if (departmentCategories.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainBatchUpdatePM6"));
            }

            var domains = from e in departmentCategories
                          group e by e.ProductDomainSysNo into g
                          select g.Key;
            using (TransactionScope scope = new TransactionScope())
            {
                da.BatchUpdatePM(departmentCategorySysNoList, pmSysNo);

                foreach (var sysNo in domains)
                {
                    da.CreateProductDomainChangePool(sysNo.Value);
                }
                scope.Complete();
            }
        }

        private void PreCheckProductDomain(ProductDomain entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (string.IsNullOrEmpty(entity.ProductDomainName.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainPreCheckProductDomainResult1"));
            }
            if ((entity.ProductDomainLeaderUserSysNo ?? 0) <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainPreCheckProductDomainResult2"));
            }

            if (da.ExistSameProductDomainName(entity.ProductDomainName.Content, entity.SysNo??0, entity.CompanyCode))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainPreCheckProductDomainResult3"));
            }

            if (entity.DepartmentMerchandiserSysNoList != null && entity.DepartmentMerchandiserSysNoList.Any())
            {
                for (int i = 0; i < entity.DepartmentMerchandiserSysNoList.Count; i++)
                {
                    var e = entity.DepartmentMerchandiserSysNoList[i];

                    if ((e ?? 0) <= 0)
                    {
                        throw new BizException(i + ResouceManager.GetMessageString("IM.Product", "ProductDomainPreCheckProductDomainResult4"));
                    }
                }
            }
        }

        private void ValidateDepartmentCategories(List<ProductDepartmentCategory> restDepartmentCategoies, int c2SysNo)
        {
            //如果剩余的分类中有品牌为空的， 则对当前2级类的修改一定成功
            if (restDepartmentCategoies.Any(e => e.BrandSysNo == null))
            {
                return;
            }
            var brndSysNos = restDepartmentCategoies.Select(e => e.BrandSysNo.Value).ToList();
            //检察这些分类是否可以包含住所有的产品
            if (da.ExisingAnyProductNotContainedInTheRange(c2SysNo, brndSysNos))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateDepartmentCategoriesResult"));
            }
        }

        private void ValidateProductDepartmentCategory(ProductDepartmentCategory entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.C2SysNo == null)
            {
                throw new ArgumentNullException("entity.C2SysNo");
            }
            if (entity.PMSysNo == null)
            {
                throw new ArgumentNullException("entity.PMSysNo");
            }
            if (entity.ProductDomainSysNo == null)
            {
                throw new ArgumentNullException("entity.ProductDomainSysNo");
            }
            var productDomain = LoadBySysNo(entity.ProductDomainSysNo.Value);
            if (productDomain == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateProductDepartmentCategoryResult1"));
            }
            var dtInfo = da.GetTheCountOfDepartmentCategoryRelatedInfo(entity);
            var message = new List<string>();
            if ((int)dtInfo.Rows[0]["IsValidC2SysNo"] == 0)
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateProductDepartmentCategoryResult2"));
            }
            if (entity.BrandSysNo.HasValue && (int)dtInfo.Rows[0]["IsValidBrandSysNo"] == 0)
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateProductDepartmentCategoryResult3"));
            }
            if ((int)dtInfo.Rows[0]["IsValidPMSysNo"] == 0)
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateProductDepartmentCategoryResult4"));
            }
            if (message.Any())
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDomainValidateProductDepartmentCategoryResult5") + message.Join(","));
            }
        }

        private void ValidateUpdateProductDepartmentCateogry(ProductDepartmentCategory entity)
        {
            var existsingCategory = da.LoadDepartmentCategoryBySysNo(entity.SysNo.Value);
            if (existsingCategory == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "PorductDomainValidateUpdateProductDepartmentCateogry1"));
            }

            //如果没有修改过二级类及品牌， 直接允许通过
            if (entity.C2SysNo == existsingCategory.C2SysNo && entity.BrandSysNo == existsingCategory.BrandSysNo)
            {
                return;
            }

            var repeatedCategories = da.GetExistingProductCategories(new List<int> { entity.C2SysNo.Value, existsingCategory.C2SysNo.Value }, 0);
            repeatedCategories = repeatedCategories.Where(e => e.SysNo != entity.SysNo).ToList();
            //指定的二级类是否在其他Domain存在
            if (repeatedCategories.Any(e => e.ProductDomainSysNo != entity.ProductDomainSysNo && e.C2SysNo == entity.C2SysNo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "PorductDomainValidateUpdateProductDepartmentCateogry2"));
            }

            //是否会导致新分类重复
            if (repeatedCategories.Any(e => e.C2SysNo == entity.C2SysNo && e.BrandSysNo == entity.BrandSysNo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "PorductDomainValidateUpdateProductDepartmentCateogry3"));
            }
            //不重复， 将新数据插入列表， 模拟最后的结果集，验证之
            repeatedCategories.Add(entity);

            var oldCatgeories = repeatedCategories.Where(e => e.C2SysNo == existsingCategory.C2SysNo).ToList();
            //如果会导致原分类丢失PM
            ValidateDepartmentCategories(oldCatgeories, existsingCategory.C2SysNo.Value);
        }
    }
}
