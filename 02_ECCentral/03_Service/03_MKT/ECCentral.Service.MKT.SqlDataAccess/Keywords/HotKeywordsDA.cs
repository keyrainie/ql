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
    [VersionExport(typeof(IHotKeywordsDA))]
    public class HotKeywordsDA : IHotKeywordsDA
    {
        #region 热门关键字（HotSearchKeyWord）

        /// <summary>
        /// 预览关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<string> GetHotKeywordsListByPageType(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetHotKeywordsListByPageType");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return list;
        }

        /// <summary>
        /// 获取首页关键字
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public string GetHotKeywordsForHomepage(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetHotKeywordsForHomepage");
            dc.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = dc.ExecuteDataTable();
            if (dt == null || dt.Rows.Count < 1)
                return string.Empty;
            return dt.Rows[0][0].ToString().Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<HotSearchKeyWords> GetHotKeywordsEditByKeyword(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetHotKeywordsEditByKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            List<HotSearchKeyWords> list = new List<HotSearchKeyWords>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<HotSearchKeyWords>(dr));
            }
            return list;
        }

        /// <summary>
        /// 适用底层类别，扩展生效判断
        /// 根据PageID，查询相同的C3前台类别SysNO
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<HotSearchKeyWords> GetOtherHotSearchKeywordECCategoryList(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetOtherHotSearchKeywordEC_CategoryList");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            return dc.ExecuteEntityList<HotSearchKeyWords>();
        }

        /// <summary>
        /// 根据关键字获取类似的关键字列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<string> GetSearchedKeywordsInfoByKeyword(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetSearchedKeywordsInfoByKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return list;
        }

        /// <summary>
        /// 根据关键字获取类似的关键字列表，IsOnlineShow不为1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<string> GetHotKeywordsInfoByKeywordbyTrue(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_GetHotKeywordsInfoByKeywordbyTrue");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return list;
        }
        /// <summary>
        /// 根据关键字获取类似的关键字列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<string> GetHotKeywordsInfoByKeyword(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_GetHotKeywordsInfoByKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return list;
        }

        /// <summary>
        /// 获取热门关键字的最大长度,根据已经存在的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetHotKeywordsMaxLenthBySysNo(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_CheckKeywordMaxLenthBySysNo");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            StringBuilder maxKeywords = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    maxKeywords.Append(dr[0].ToString());
                }
            }
            maxKeywords.Append(item.Keywords.Content);
            if (maxKeywords.ToString().Length > 41)
                return false;
            return true;
        }

        /// <summary>
        /// 扩展生效功能
        /// 操作同类后的记录，无则修改，有则增加
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void OperExpandECCategoryHotSearchKeyword(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_OperExpandEC_CategoryHotSearchKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 判断在同后台类别的前台类别下，是否有相同名称的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IsSameNameHotSearchKeyword(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_IsSameNameHotSearchKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            return dc.ExecuteScalar<int>();
        }

        /// <summary>
        /// 获取热门关键字的最大长度
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetHotKeywordsMaxLenth(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_GetHotKeywordsMaxLenth");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            DataTable dt = dc.ExecuteDataTable();
            return int.Parse(dt.Rows[0][0].ToString());
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<UserInfo> GetHotKeywordsEditUserList(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetEditHotKeywordsUserList");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<UserInfo>();
        }

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetHotKeywordsInvalid(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_InvalidHotKeywordsStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            // dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加热门搜索关键字-不显示状态
        /// </summary>
        /// <param name="item"></param>
        public void InsertHotKeywordsInfoHidder(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertHotKeywordsInfoHidder");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public void AddHotSearchKeywords(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertHotKeywordsInfo");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 添加热门搜索关键字for SearchKeyword
        /// </summary>
        /// <param name="item"></param>
        public void AddHotSearchKeywordsForSearch(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertSearchKeywordForHotKeyword");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public void EditHotSearchKeywords(HotSearchKeyWords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateHotKeywordsInfo");
            dc.SetParameterValue<HotSearchKeyWords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新热门搜索关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        public void ChangeHotSearchedKeywordsStatus(List<int> items)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载热门搜索关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public HotSearchKeyWords LoadHotSearchKeywords(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertCustomerRight");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            HotSearchKeyWords keywords = new HotSearchKeyWords();
            keywords = DataMapper.GetEntity<HotSearchKeyWords>(dt.Rows[0]);
            keywords.Keywords = new LanguageContent("zh-CN", dt.Rows[0]["Keywords"].ToString().Trim());
            return keywords;
        }

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetHotKeywordsAvailable(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Keywords_AvailableHotKeywordsStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            // dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }
        #endregion
    }
}
