using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.Keywords;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess.Keywords
{
    [VersionExport(typeof(IAdvancedKeywordsDA))]
    public class AdvancedKeywordsDA : IAdvancedKeywordsDA
    {
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public int AddAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AdvancedKeywords_CreateAdvancedKeywordsInfo");
            dc.SetParameterValue<AdvancedKeywordsInfo>(item);
            dc.ExecuteNonQuery();
            return (int)dc.GetParameterValue("@SysNo");
        }

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="item"></param>
        public void EditAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AdvancedKeywords_ModifyAdvancedKeywordsInfo");
            dc.SetParameterValue<AdvancedKeywordsInfo>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 是否存在当前关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSameAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AdvancedKeywords_HasTheSameKeywords");
            dc.SetParameterValue<AdvancedKeywordsInfo>(item);
            return dc.ExecuteScalar()==null;
        }


        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public AdvancedKeywordsInfo LoadAdvancedKeywordsInfo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AdvancedKeywords_GetAdvancedKeywords");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            return DataMapper.GetEntity<AdvancedKeywordsInfo>(dt.Rows[0]);
        }

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <param name="item"></param>
        public void CreateAdvancedKeywordsLog(AdvancedKeywordsLog item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AdvancedKeywords_CreateAdvancedKeywordsLog");
            dc.SetParameterValue<AdvancedKeywordsLog>(item);
            dc.ExecuteNonQuery();
        }
    }
}
