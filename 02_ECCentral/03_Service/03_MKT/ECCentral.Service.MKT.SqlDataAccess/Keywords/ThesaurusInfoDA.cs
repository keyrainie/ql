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
namespace ECCentral.Service.MKT.SqlDataAccess.Keywords
{
    [VersionExport(typeof(IThesaurusInfoDA))]
    public class ThesaurusInfoDA : IThesaurusInfoDA
    {

        #region 同义词（ThesaurusInfo）
        /// <summary>
        ///添加同义词
        /// </summary>
        /// <param name="item"></param>
        public void AddThesaurusWords(ThesaurusInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ThesaurusKeywords_CreateThesaurus");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查是否存在相同的同义词
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckThesaurusWordsForUpdate(ThesaurusInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ThesaurusKeywords_CheckThesaurusWordsForUpdate");
            cmd.SetParameterValue<ThesaurusInfo>(item);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 检查是否存在相同的同义词
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckThesaurusWordsForInsert(ThesaurusInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ThesaurusKeywords_CheckThesaurusWordsForInsert");
            cmd.SetParameterValue<ThesaurusInfo>(item);
            return cmd.ExecuteScalar() != null;
        }
        /// <summary>
        ///更新同义词
        /// </summary>
        /// <param name="item"></param>
        public void UpdateThesaurusInfo(ThesaurusInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ThesaurusKeywords_UpdateThesaurusWords");
            dc.SetParameterValue<ThesaurusInfo>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <returns></returns>
        public ThesaurusInfo LoadThesaurusWords(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertCustomerRight");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            ThesaurusInfo keywords = new ThesaurusInfo();
            keywords = DataMapper.GetEntity<ThesaurusInfo>(dt.Rows[0]);
            keywords.ThesaurusWords = new LanguageContent("zh-CN", dt.Rows[0]["Keywords"].ToString().Trim());
            return keywords;
        }
        #endregion
    }
}
