using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ICategoryKeywordsDA))]
    public class CategoryKeywordsDA : ICategoryKeywordsDA
    {

        #region 分类关键字（CategoryKeywords）
        /// <summary>
        /// 是否存在该类别下的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckCategoryKeywordsC3SysNo(CategoryKeywords item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Keyword_CheckKeywordsC3SysNo");
            cmd.SetParameterValue<CategoryKeywords>(item);
            cmd.ExecuteNonQuery();
            if (((int)cmd.GetParameterValue("@SysNo")) == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取关键字对应的属性列表
        /// </summary>
        /// <param name="sysNoString"></param>
        /// <returns></returns>
        public List<string> GetKeywordsProperty(string sysNoString)
        {
            if (string.IsNullOrEmpty(sysNoString))
                return null;
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetPropertyDescription");
            dc.SetParameterValue("@SysNoList", sysNoString);
            DataTable dt = dc.ExecuteDataTable();
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return list;
        }

        #endregion


        #region 三级类通用关键字（CommonKeyWords）
        /// <summary>
        ///设置通用关键字
        /// </summary>
        /// <param name="item"></param>
        public void AddCommonKeyWords(CategoryKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_CreateCommonKeywords");
            dc.SetParameterValue<CategoryKeywords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public void UpdateCommonKeyWords(CategoryKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateCommonKeywords");
            dc.SetParameterValue<CategoryKeywords>(item);
            dc.ExecuteNonQuery();
        }

        #endregion


        /// <summary>
        /// 好像是产品关键字队列
        /// </summary>
        /// <param name="item"></param>
        public void InsertProductKeywordsQueue(ProductKeywordsQueue item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertProductKeywordsQueue");
            dc.SetParameterValue<ProductKeywordsQueue>(item);
            dc.ExecuteNonQuery();
        }

        #region 三级类属性关键字（PropertyKeywords）


        /// <summary>
        ///设定三级类属性关键字  设置属性关键字，送商品类别属性中选择
        /// </summary>
        /// <param name="item"></param>
        public void AddPropertyKeywords(CategoryKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_CreatePropertyKeywords");
            dc.SetParameterValue<CategoryKeywords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePropertyKeywords(CategoryKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdatePropertyKeywords");
            dc.SetParameterValue<CategoryKeywords>(item);
            dc.ExecuteNonQuery();
        }

        public List<PropertyInfo> GetPropertyInfo(string companyCode, int productSysNo, int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Keyword_GetPropertyInfo");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@productSysNo", productSysNo);
            return cmd.ExecuteEntityList<PropertyInfo>();
        }

        #endregion
    }
}
