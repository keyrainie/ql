using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Category;
using ECommerce.DataAccess.Product;
using ECommerce.DataAccess.Recommend;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.Product;

namespace ECommerce.Facade.Product
{
    public class CategoryFacade
    {
        public static Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

        #region 商品类别
        /// <summary>
        /// (已缓存)所有的前台商品类别
        /// </summary>
        /// <returns></returns>
        public static List<CategoryInfo> QueryCategoryInfos()
        {
            string cacheKey = CommonFacade.GenerateKey("QueryCategoryInfos");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<CategoryInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<CategoryInfo> result = CategoryDA.QueryCategories();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return result;
        }

        private static List<BrandInfoExt> GetBrands(int c1SysNo, List<CategoryInfo> cates, List<BrandInfoExt> brands)
        {
            var result = new List<BrandInfoExt>();
            var c3SysNoes = new List<int>();
            var c1 = cates.Where(f => f.CategoryID == c1SysNo).First();
            if (c1 != null)
            {
                c1.SubCategories.ForEach(p =>
                {
                    p.SubCategories.ForEach(q =>
                    {
                        c3SysNoes.Add(q.CategoryID);
                    });
                });
            }

            result.AddRange(brands.Where(p => c3SysNoes.Any(q => q == p.ECSysNo && !string.IsNullOrWhiteSpace(p.ADImage))));

            return result;
        }

        /// <summary>
        /// 获得所有的品牌
        /// 如果一级分类下没有任何品牌则此一级分类不显示
        /// 如果一级分类下的所有品牌都没有LOGO图片则不显示
        /// </summary>
        /// <returns></returns>
        public static List<CategoryBrand> QueryCategoryBrandInfos()
        {
            var categories = QueryCategoryInfos();
            var cates = QueryCategoryInfosForHomePage();
            var brands = ProductDA.GetAllBrands();

            var result = new List<CategoryBrand>();
            categories.ForEach(p =>
            {
                if (p.CategoryType == CategoryType.TabStore)
                {
                    var item = new CategoryBrand();
                    item.Category = p;
                    item.BrandInfos = GetBrands(p.CategoryID, cates, brands);
                    if (item.BrandInfos.Count > 0)
                    {
                        result.Add(item);
                    }
                }
            });
            return result;
        }

        public static List<CategoryInfo> QueryCategoryInfosForHomePage()
        {
            string cacheKey = CommonFacade.GenerateKey("QueryCategoryInfosForHomePage");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<CategoryInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<CategoryInfo> categoryList = QueryCategoryInfos();

            List<CategoryInfo> queryList = ECommerce.Utility.SerializationUtility.DeepClone<List<CategoryInfo>>(categoryList);

            List<CategoryInfo> category1_List = queryList.FindAll(x => x.CategoryType == CategoryType.TabStore);
            category1_List.OrderBy(x => x.Priority);

            List<CategoryInfo> category2_List = queryList.FindAll(x => x.CategoryType == CategoryType.Category);
            List<CategoryInfo> category3_List = queryList.FindAll(x => x.CategoryType == CategoryType.SubCategory);

            foreach (CategoryInfo entity in category2_List)
            {
                entity.SubCategories = category3_List.FindAll(x => x.ParentSysNo == entity.CurrentSysNo);
                entity.SubCategories.OrderBy(x => x.Priority);
            }

            foreach (CategoryInfo entity in category1_List)
            {
                entity.SubCategories = category2_List.FindAll(x => x.ParentSysNo == entity.CurrentSysNo);
                entity.SubCategories.OrderBy(x => x.Priority);
            }

            HttpRuntime.Cache.Insert(cacheKey, category1_List, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return category1_List;
        }

        public static List<CategoryInfo> QueryCategoriesForC1(string c1SysNo)
        {
            var result = from a in QueryCategoryInfosForHomePage()
                         where a.CategoryID == Convert.ToInt32(c1SysNo)
                         select a.SubCategories;
            return result.First();
        }
        public static List<CategoryInfo> QueryCategoriesForC2(int c2SysNo)
        {
            var c1SysNo = (from a in QueryCategoryInfos()
                           where a.CategoryType == CategoryType.Category && a.CurrentSysNo == c2SysNo
                           select a.ParentSysNo).First();
            var allC2 = from a in QueryCategoryInfosForHomePage()
                        where a.CurrentSysNo == c1SysNo
                        select a.SubCategories;

            var result = from a in allC2.First()
                         where a.CurrentSysNo == c2SysNo
                         select a.SubCategories;
            return result.First();
        }

        public static CategoryInfo QuerySingleC2(int c3CategoryID)
        {
            var allCategories = QueryCategoryInfos();
            //var c2SysNo = (from r in allCategories
            //               where r.CategoryType == CategoryType.SubCategory && r.CategoryID == c3CategoryID
            //               select r.ParentSysNo).First();
            //var c1SysNo = (from r in allCategories
            //               where r.CategoryType == CategoryType.Category && r.CurrentSysNo == c2SysNo
            //               select r.ParentSysNo).First();

            CategoryInfo subCategoryInfo = allCategories.Find(f => f.CategoryType == CategoryType.SubCategory && f.CategoryID == c3CategoryID);
            var c2SysNo = subCategoryInfo == null ? 0 : subCategoryInfo.ParentSysNo;

            CategoryInfo categoryInfo = allCategories.Find(f => f.CategoryType == CategoryType.Category && f.CurrentSysNo == c2SysNo);
            var c1SysNo = categoryInfo == null ? 0 : categoryInfo.ParentSysNo;

            var allCategoriesTree = QueryCategoryInfosForHomePage();
            //var c1 = (from r in allCategoriesTree
            //          where r.CurrentSysNo == c1SysNo
            //          select r).First();
            //var c2 = (from r in c1.SubCategories
            //          where r.CurrentSysNo == c2SysNo
            //          select r).First();
            CategoryInfo c1 = allCategoriesTree.Find(f => f.CurrentSysNo == c1SysNo);

            CategoryInfo c2 = null;
            if (c1 != null)
            {
                c2 = c1.SubCategories.Find(f => f.CurrentSysNo == c2SysNo);
            }

            return c2;
        }

        public static CategoryInfo GetSingleC2(int c2CategoryID)
        {
            var allCategories = QueryCategoryInfos();

            CategoryInfo subCategoryInfo = allCategories.Find(f => f.CategoryType == CategoryType.Category && f.CategoryID == c2CategoryID);
            var c2SysNo = subCategoryInfo == null ? 0 : subCategoryInfo.CurrentSysNo;

            CategoryInfo categoryInfo = allCategories.Find(f => f.CategoryType == CategoryType.Category && f.CurrentSysNo == c2SysNo);
            var c1SysNo = categoryInfo == null ? 0 : categoryInfo.ParentSysNo;

            var allCategoriesTree = QueryCategoryInfosForHomePage();

            CategoryInfo c1 = allCategoriesTree.Find(f => f.CurrentSysNo == c1SysNo);

            CategoryInfo c2 = null;
            if (c1 != null)
            {
                c2 = c1.SubCategories.Find(f => f.CurrentSysNo == c2SysNo);
            }

            return c2;
        }
        #endregion
    }
}
