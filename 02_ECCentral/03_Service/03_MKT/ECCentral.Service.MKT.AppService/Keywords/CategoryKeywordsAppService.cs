using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{

    [VersionExport(typeof(CategoryKeywordsAppService))]
    public class CategoryKeywordsAppService
    {
        #region 分类关键字（CategoryKeywords）
        /// <summary>
        /// 是否存在该类别下的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CheckCategoryKeywordsC3SysNo(CategoryKeywords item)
        {
            return ObjectFactory<CategoryKeywordsProcessor>.Instance.CheckCategoryKeywordsC3SysNo(item);
        }

        /// <summary>
        ///设置分类关键字的三级类别
        /// </summary>
        /// <param name="item"></param>
        //public virtual void SetC3OfCategoryKeywords(CategoryKeywords item)
        //{
        //    ObjectFactory<CategoryKeywordsProcessor>.Instance.SetC3OfCategoryKeywords(item);
        //}

        ///// <summary>
        ///// 加载分类关键字信息，包括通用关键字和属性关键字列表
        ///// </summary>
        ///// <returns></returns>
        //public virtual CategoryKeywords LoadCategoryKeywords(int sysNo)
        //{
        //    return ObjectFactory<CategoryKeywordsProcessor>.Instance.LoadCategoryKeywords(sysNo);
        //}

        #endregion

        #region 三级类通用关键字（CommonKeyWords）
        /// <summary>
        ///设置通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddCommonKeyWords(CategoryKeywords item)
        {
            ObjectFactory<CategoryKeywordsProcessor>.Instance.AddCommonKeyWords(item);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateCommonKeyWords(CategoryKeywords item)
        {
            ObjectFactory<CategoryKeywordsProcessor>.Instance.UpdateCommonKeyWords(item);
        }

        #endregion

        #region 三级类属性关键字（PropertyKeywords）

        /// <summary>
        /// 获取三级类别下的属性列表
        /// </summary>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.IM.CategoryProperty> GetPropertyByCategory(int sysNo)
        {
            return ObjectFactory<CategoryKeywordsProcessor>.Instance.GetPropertyByCategory(sysNo);
        }

        /// <summary>
        ///设定三级类属性关键字  设置属性关键字，送商品类别属性中选择
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddPropertyKeywords(CategoryKeywords item)
        {
            ObjectFactory<CategoryKeywordsProcessor>.Instance.AddPropertyKeywords(item);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePropertyKeywords(CategoryKeywords item)
        {
            ObjectFactory<CategoryKeywordsProcessor>.Instance.UpdatePropertyKeywords(item);
        }

        #endregion

    }
}
