using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ICategoryKeywordsDA
    {
        /// <summary>
        /// 好像是产品关键字队列
        /// </summary>
        /// <param name="item"></param>
        void InsertProductKeywordsQueue(ProductKeywordsQueue item);

        #region 分类关键字（CategoryKeywords）
        /// <summary>
        /// 是否存在该类别下的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckCategoryKeywordsC3SysNo(CategoryKeywords item);

        /// <summary>
        /// 获取关键字对应的属性列表
        /// </summary>
        /// <param name="sysNoString"></param>
        /// <returns></returns>
        List<string> GetKeywordsProperty(string sysNoString);

        /// <summary>
        ///设置分类关键字的三级类别
        /// </summary>
        /// <param name="item"></param>
        //void SetC3OfCategoryKeywords(CategoryKeywords item);

        /// <summary>
        /// 加载分类关键字信息，包括通用关键字和属性关键字列表
        /// </summary>
        /// <returns></returns>
        //CategoryKeywords LoadCategoryKeywords(int sysNo);
        #endregion

        #region 三级类通用关键字（CommonKeyWords）
        /// <summary>
        ///设置通用关键字
        /// </summary>
        /// <param name="item"></param>
        void AddCommonKeyWords(CategoryKeywords item);

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        void UpdateCommonKeyWords(CategoryKeywords item);

        /// <summary>
        /// 加载三级类通用关键字
        /// </summary>
        /// <returns></returns>
        //CategoryKeywords LoadCommonKeyWords(int sysNo);
        #endregion

        #region 三级类属性关键字（PropertyKeywords）
        /// <summary>
        ///设定三级类属性关键字  设置属性关键字，送商品类别属性中选择
        /// </summary>
        /// <param name="item"></param>
        void AddPropertyKeywords(CategoryKeywords item);

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        void UpdatePropertyKeywords(CategoryKeywords item);

        /// <summary>
        /// 加载三级类通用关键字
        /// </summary>
        /// <returns></returns>
        //List<CategoryKeywords> LoadPropertyKeywords(int sysNo);
        
        List<PropertyInfo> GetPropertyInfo(string companyCode, int productSysNo, int c3SysNo);
        #endregion

    }
}
