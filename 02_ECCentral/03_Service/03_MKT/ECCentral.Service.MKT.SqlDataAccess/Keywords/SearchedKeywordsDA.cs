using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess.Keywords
{
    [VersionExport(typeof(ISearchedKeywordsDA))]
    public class SearchedKeywordsDA : ISearchedKeywordsDA
    {
        #region 自动匹配关键字（SearchedKeyword）

        /// <summary>
        /// 检查是否存在相同的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSearchKeywords(SearchedKeywords item)
        {
             DataCommand cmd ;
            if (item.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("Keyword_CheckSearchKeywordByUpdate");
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("Keyword_CheckSearchKeywordByKeywords");
            }
        
            cmd.SetParameterValue<SearchedKeywords>(item);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 添加自动匹配的搜索关键字    IPP系统中默认CreateUserType=0。
        /// </summary>
        /// <param name="item"></param>
        public void AddSearchedKeywords(SearchedKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertSearchKeyword");
            dc.SetParameterValue<SearchedKeywords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        public void EditSearchedKeywords(SearchedKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateSearchKeyword");
            dc.SetParameterValue<SearchedKeywords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新自动匹配关键字屏蔽状态 
        /// </summary>
        /// <param name="items"></param>
        public void ChangeSearchedKeywordsStatus(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateStatusSearchKeyword");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="items"></param>
        public void DeleteSearchedKeywords(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_DeleteSearchKeyword");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SearchedKeywords LoadSearchedKeywords(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetSearchKeyword");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            return DataMapper.GetEntity<SearchedKeywords>(dt.Rows[0]);
        }

        /// <summary>
        /// 加载编辑用户
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> LoadEditUsers(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetEditUserSearchKeyword");
            dc.SetParameterValue("@CompanyCode", companyCode);
            using (IDataReader reader = dc.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<UserInfo, List<UserInfo>>(reader);
            }
        }
        #endregion

    }
}
