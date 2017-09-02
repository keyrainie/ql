using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICategoryDA
    {

        CategoryInfo AddCategory(CategoryInfo entity);

        void UpdateCategory(CategoryInfo entity);

        List<CategoryInfo> GetAllCategory();

        List<CategoryInfo> GetCategory1List();

        CategoryInfo GetCategoryBySysNo(int sysNo);

        CategoryInfo GetCategory1BySysNo(int c1ysNo);

        CategoryInfo GetCategory2BySysNo(int c2ysNo);

        CategoryInfo GetCategory3BySysNo(int c3SysNo);

        /// <summary>
        /// 根据C3SysNo得到C1CategoryInfo
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        CategoryInfo GetCategory1ByCategory3SysNo(int c3SysNo);


        CategoryInfo GetCategoryByCategoryName(string categoryName);

        bool IsExistsCategoryByCategoryName(string categoryName, int sysNo);

        bool IsExistsCategoryByCategoryName(string categoryName);

        string GetCategoryC2AndC3NameBySysNo(int? c2SysNo, int? c3SysNo);

        bool IsExistsCategoryByCategoryID(string categoryID);

        bool IsExistsCategoryByCategoryID(string categoryID, int sysNo);

        bool IsCategoryInUsing(int categorySysNo);

        bool IsExistsCategoryBySysNo(int sysNo);

        /// <summary>
        /// 根据SysNo检查类别2是否存在
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool IsExistsCategory2BySysNo(int sysNo);

        /// <summary>
        /// 根据CategoryType创建不同的类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        void CeateCategoryByType(CategoryType type,CategoryInfo entity);

        /// <summary>
        /// 根据CategoryType更改不同的类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        void UpdateCategoryByType(CategoryType type, CategoryInfo entity);

        /// <summary>
        /// 检查不同类别是否有相同的
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsIsExistCategoryByType(CategoryType type, CategoryInfo entity);
        /// <summary>
        /// 同步表
        /// </summary>
        /// <returns></returns>
        int CreateCategorySequence();

        /// <summary>
        /// 根据query得到不同类别的信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        DataTable GetCategoryListByType(CategoryQueryFilter query, out int totalCount);
        


    }
}
