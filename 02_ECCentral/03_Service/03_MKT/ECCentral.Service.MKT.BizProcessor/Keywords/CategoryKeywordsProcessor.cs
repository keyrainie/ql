using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;


namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(CategoryKeywordsProcessor))]
    public class CategoryKeywordsProcessor
    {

        private ICategoryKeywordsDA keywordDA = ObjectFactory<ICategoryKeywordsDA>.Instance;

        #region 分类关键字（CategoryKeywords）
        /// <summary>
        /// 是否存在该类别下的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CheckCategoryKeywordsC3SysNo(CategoryKeywords item)
        {
            return keywordDA.CheckCategoryKeywordsC3SysNo(item);
        }

        /// <summary>
        ///设置分类关键字的三级类别
        /// </summary>
        /// <param name="item"></param>
        //public virtual void SetC3OfCategoryKeywords(CategoryKeywords item)
        //{
        //    keywordDA.SetC3OfCategoryKeywords(item);
        //}

        ///// <summary>
        ///// 加载分类关键字信息，包括通用关键字和属性关键字列表
        ///// </summary>
        ///// <returns></returns>
        //public virtual CategoryKeywords LoadCategoryKeywords(int sysNo)
        //{
        //    return keywordDA.LoadCategoryKeywords(sysNo);
        //}
        #endregion

        #region 三级类通用关键字（CommonKeyWords）
        /// <summary>
        ///设置通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddCommonKeyWords(CategoryKeywords item)
        {
            keywordDA.AddCommonKeyWords(item);
            ProductKeywordsQueue queue = new ProductKeywordsQueue();
            queue.C3SysNo = item.Category3SysNo.Value;
            queue.CompanyCode = item.CompanyCode;
            queue.ProductSysNo = 0;
            keywordDA.InsertProductKeywordsQueue(queue);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateCommonKeyWords(CategoryKeywords item)
        {
            keywordDA.UpdateCommonKeyWords(item);
            ProductKeywordsQueue queue = new ProductKeywordsQueue();
            queue.C3SysNo = item.Category3SysNo.Value;
            queue.CompanyCode = item.CompanyCode;
            queue.ProductSysNo = 0;
            keywordDA.InsertProductKeywordsQueue(queue);
        }

        /// <summary>
        /// 加载三级类通用关键字
        /// </summary>
        /// <returns></returns>
        //public virtual CommonKeyWords LoadCommonKeyWords(int sysNo)
        //{
        //    return keywordDA.LoadCommonKeyWords(sysNo);
        //}
        #endregion

        #region 三级类属性关键字（PropertyKeywords）
        /// <summary>
        /// 获取三级类别下的属性列表
        /// </summary>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.IM.CategoryProperty> GetPropertyByCategory(int sysNo)
        {
            //调用 IM的接口获取属性列表
            //IList<ECCentral.BizEntity.IM.CategoryProperty> list = ExternalDomainBroker.GetCategoryPropertyByCategorySysNo(sysNo);//ObjectFactory<ECCentral.Service.IBizInteract.IIMBizInteract>.Instance
            ECCentral.BizEntity.IM.CategorySetting list = ExternalDomainBroker.GetCategorySetting(sysNo);
            return list.CategoryProperties.ToList();
        }


        /// <summary>
        ///设定三级类属性关键字  设置属性关键字，送商品类别属性中选择
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddPropertyKeywords(CategoryKeywords item)
        {
            keywordDA.AddPropertyKeywords(item);

            ProductKeywordsQueue queue = new ProductKeywordsQueue();
            queue.C3SysNo = item.Category3SysNo.Value;
            queue.CompanyCode = item.CompanyCode;
            queue.ProductSysNo = 0;
            keywordDA.InsertProductKeywordsQueue(queue);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePropertyKeywords(CategoryKeywords item)
        {
            keywordDA.UpdatePropertyKeywords(item);

            ProductKeywordsQueue queue = new ProductKeywordsQueue();
            queue.C3SysNo = item.Category3SysNo.Value;
            queue.CompanyCode = item.CompanyCode;
            queue.ProductSysNo = 0;
            keywordDA.InsertProductKeywordsQueue(queue);
        }

        /// <summary>
        /// 加载三级类通用关键字
        /// </summary>
        /// <returns></returns>
        //public virtual List<CategoryKeywords> LoadPropertyKeywords(int sysNo)
        //{
        //    return keywordDA.LoadPropertyKeywords(sysNo);
        //}
        #endregion
    }
}
