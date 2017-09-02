using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IDefaultKeywordsDA))]
    public class DefaultKeywordsDA : IDefaultKeywordsDA
    {
        #region 默认关键字（DefaultKeywordsInfo）

        /// <summary>
        /// 检查是否存在该默认关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckDuplicate(DefaultKeywordsInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Keyword_CheckDuplicateDefaultKeywordsInfo");
            cmd.SetParameterValue<DefaultKeywordsInfo>(item);
            return cmd.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public void AddDefaultKeywords(DefaultKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_InsertDefaultKeywords");
            dc.SetParameterValue<DefaultKeywordsInfo>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑默认关键字
        /// </summary>
        /// <param name="item"></param>
        public void EditDefaultKeywords(DefaultKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_UpdateDefaultKeywords");
            dc.SetParameterValue<DefaultKeywordsInfo>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public DefaultKeywordsInfo LoadDefaultKeywordsInfo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Keyword_GetDefaultKeyword");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            DefaultKeywordsInfo keywords = new DefaultKeywordsInfo();
            return DataMapper.GetEntity<DefaultKeywordsInfo>(dt.Rows[0]);
        }
        #endregion
    }
}
