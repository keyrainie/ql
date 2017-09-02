
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryProcessor))]
    public class CategoryProcessor
    {

        private ICategoryDA categoryDA = ObjectFactory<ICategoryDA>.Instance;


        public virtual CategoryInfo AddCategory(CategoryInfo entity)
        {
            //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
            if (entity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryInvalid"));
            }
            if (entity.CategoryName != null && !string.IsNullOrEmpty(entity.CategoryName.Content))
            {
                entity.CategoryName.Content = entity.CategoryName.Content.Trim();
            }
            if (entity.CategoryName == null || string.IsNullOrEmpty(entity.CategoryName.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryDisplayNameIsNull"));
            }
            var result = categoryDA.IsExistsCategoryByCategoryName(entity.CategoryName.Content);
            if (result)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryName"));
            }
            //result = categoryDA.IsExistsCategoryByCategoryID(entity.CategoryID);
            //if (result)
            //{
            //    throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryID"));
            //}

            categoryDA.AddCategory(entity);
            return entity;
        }

        /// <summary>
        /// 根据不同类别添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        public virtual void CeateCategoryByType(CategoryType type, CategoryInfo entity)
        {
            if (entity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryInvalid"));
            }
            if (entity.CategoryName != null && !string.IsNullOrEmpty(entity.CategoryName.Content))
            {
                entity.CategoryName.Content = entity.CategoryName.Content.Trim();
            }
            if (entity.CategoryName == null || string.IsNullOrEmpty(entity.CategoryName.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryDisplayNameIsNull"));
            }
            
            var result = categoryDA.IsIsExistCategoryByType(type, entity);
            if (result)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryName"));
            }
            categoryDA.CeateCategoryByType(type, entity);
        }

        /// <summary>
        /// 根据不同类别更新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        public virtual void UpdateCategoryByType(CategoryType type, CategoryInfo entity)
        {
            if (entity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryInvalid"));
            }
            if (entity.CategoryName == null || string.IsNullOrEmpty(entity.CategoryName.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryDisplayNameIsNull"));
            }
            if (entity.SysNo == null || entity.SysNo.Value <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategorySysNOIsNull"));
            }
            var result = categoryDA.IsIsExistCategoryByType(type, entity);
            if (result)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryName"));
            }
           
            if (entity.Status == CategoryStatus.DeActive)
            {
                //result = categoryDA.IsCategoryInUsing(entity.SysNo.Value);
                result = ObjectFactory<IProductLineDA>.Instance.Category2IsUsing(entity.SysNo.Value, (int)type);
                if (result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Category", "IsCategoryInUsing"));
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                categoryDA.UpdateCategoryByType(type, entity);
                if(entity.Status == CategoryStatus.DeActive)
                {
                    ObjectFactory<IProductLineDA>.Instance.DeleteByCategory( entity.SysNo.Value,(int)type);
                }
                scope.Complete();
            }
            
        }

        public virtual void UpdateCategory(CategoryRequestApprovalInfo info)
        {
            CategoryInfo category = new CategoryInfo()
            {
                ParentSysNumber = info.ParentSysNumber,
                SysNo = info.CategorySysNo,
                CategoryName = new LanguageContent() { Content = info.CategoryName },
                OperationType = info.OperationType,
                Status = info.CategoryStatus,
                CompanyCode = info.CompanyCode,
                LanguageCode = info.LanguageCode
            };
            if (info.CategoryType == CategoryType.CategoryType3)
            {
                category.C3Code = info.C3Code;
            }
           UpdateCategoryByType(info.CategoryType, category);
        }

        //public virtual CategoryInfo UpdateCategory(CategoryInfo entity)
        //{
        //    //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
        //    if (entity == null)
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryInvalid"));
        //    }
        //    if (entity.CategoryName == null || string.IsNullOrEmpty(entity.CategoryName.Content))
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryDisplayNameIsNull"));
        //    }
        //    if (entity.SysNo == null || entity.SysNo.Value <= 0)
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategorySysNOIsNull"));
        //    }
        //    var result = categoryDA.IsExistsCategoryByCategoryName(entity.CategoryName.Content, entity.SysNo.Value);
        //    if (result)
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryName"));
        //    }
        //    result = categoryDA.IsExistsCategoryByCategoryID(entity.CategoryID, entity.SysNo.Value);
        //    if (result)
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsCategoryID"));
        //    }
        //    if (entity.Status == CategoryStatus.DeActive)
        //    {
        //        result = categoryDA.IsCategoryInUsing(entity.SysNo.Value);
        //        if (result)
        //        {
        //            throw new BizException(ResouceManager.GetMessageString("IM.Category", "IsCategoryInUsing"));
        //        }
        //    }
        //    categoryDA.UpdateCategory(entity);
        //    return entity;
        //}

        public virtual List<CategoryInfo> GetAllCategory()
        {
            return categoryDA.GetAllCategory();
        }

        public virtual List<CategoryInfo> GetCategory1List()
        {
            return categoryDA.GetCategory1List();
        }

        public virtual CategoryInfo GetCategoryBySysNo(int sysNo)
        {
            return categoryDA.GetCategoryBySysNo(sysNo);
        }

        public virtual CategoryInfo GetCategoryByCategoryName(string categoryName)
        {
            return categoryDA.GetCategoryByCategoryName(categoryName);
        }

        public virtual CategoryInfo GetCategory3BySysNo(int c3SysNo)
        {
            return ObjectFactory<ICategoryDA>.Instance.GetCategory3BySysNo(c3SysNo);
        }

        public virtual CategoryInfo GetCategory2BySysNo(int c2SysNo)
        {
            return ObjectFactory<ICategoryDA>.Instance.GetCategory2BySysNo(c2SysNo);
        }

        public virtual CategoryInfo GetCategory1BySysNo(int c1SysNo)
        {
            return ObjectFactory<ICategoryDA>.Instance.GetCategory1BySysNo(c1SysNo);
        }

        public CategoryInfo GetC1CategoryInfoByProductID(string productID)
        {
            CategoryInfo c1 = null;
            bool isReadSuccess = false;
            var product = ObjectFactory<ProductProcessor>.Instance.GetProductInfoByID(productID);
            if (product != null)
            {
                var c3 = GetCategory3BySysNo(product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
                //Get C3 Category
                var c2 = GetCategory2BySysNo(c3.ParentSysNumber.Value);

                c1 = GetCategory1BySysNo(c2.ParentSysNumber.Value);

                isReadSuccess = true;
            }

            if (!isReadSuccess)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "UnkownC1")); 
            }
            return c1;
        }
        /// <summary>
        /// 根据C3SysNo得到C1CategoryInfo
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public CategoryInfo GetCategory1ByCategory3SysNo(int c3SysNo)
        {
            return ObjectFactory<ICategoryDA>.Instance.GetCategory1ByCategory3SysNo(c3SysNo);
        }
    }
}
