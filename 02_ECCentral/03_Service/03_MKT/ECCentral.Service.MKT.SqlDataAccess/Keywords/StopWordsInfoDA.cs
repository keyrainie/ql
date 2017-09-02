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
    [VersionExport(typeof(IStopWordsInfoDA))]
    public class StopWordsInfoDA : IStopWordsInfoDA
    {

        #region 阻止词（StopWordsInfo）
        /// <summary>
        /// 检查是否存在相同的阻止词
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckStopWords(StopWordsInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("StopWord_CheckStopword");
            cmd.SetParameterValue<StopWordsInfo>(item);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        ///根据编号列表批量更新阻止词
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStopWords(StopWordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("StopWord_UpdateStopWord");
            dc.SetParameterValue<StopWordsInfo>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        ///添加阻止词
        /// </summary>
        /// <param name="item"></param>
        public void AddStopWords(StopWordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("StopWord_InsertStopWord");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public StopWordsInfo LoadStopWords(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("StopWord_GetStopword");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            StopWordsInfo keywords = new StopWordsInfo();
            keywords = DataMapper.GetEntity<StopWordsInfo>(dt.Rows[0]);
            return keywords;
        }
        #endregion
    
    }
}
