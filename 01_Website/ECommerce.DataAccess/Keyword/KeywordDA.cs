using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Keyword
{
    public class KeywordDA
    {
        public static List<SEOInfo> GetAllSEOInfoList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Keyword_GetAllSEOInfoList");
            return cmd.ExecuteEntityList<SEOInfo>();
        }

        /// <summary>
        /// 获取热门关键信息
        /// </summary>
        public static List<HotSearchKeyword> GetHotSearchKeyword(int pageType, int pageID)
        {

            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetHotSearchKeyword");
            dataCommand.SetParameterValue("@PageType", pageType);
            dataCommand.SetParameterValue("@PageID", pageID);
            List<HotSearchKeyword> entitys = dataCommand.ExecuteEntityList<HotSearchKeyword>();
            return entitys;
        }

        public static string GetDefaultSearchKeyword(int pageType, int pageID, string languageCode, string companyCode, string storeCompanyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("GetDefaultSearchKeyword");
            cmd.SetParameterValue("@PageType", pageType);
            cmd.SetParameterValue("@PageID", pageID);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@StoreCompanyCode", storeCompanyCode);

            var dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["DefaultKeyword"].ToString();
            }
            return "";
        }
    }
}
