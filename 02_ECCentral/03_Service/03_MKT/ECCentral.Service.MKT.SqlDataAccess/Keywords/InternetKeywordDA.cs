using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
namespace ECCentral.Service.MKT.SqlDataAccess.Keywords
{
    [VersionExport(typeof(IInternetKeywordDA))]
    public class InternetKeywordDA : IInternetKeywordDA
    {
        /// <summary>
        /// 插入外网搜索关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InternetKeywordInfo InsertKeyword(InternetKeywordInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertKeyword");
            cmd.SetParameterValue("@keyword", item.Searchkeyword);
            cmd.SetParameterValue("@status", item.Status);
            cmd.SetParameterValue("@userName", item.OperateUser.UserDisplayName);
            cmd.ExecuteNonQuery();
            item.SysNo = (int)cmd.GetParameterValue("@sysNo");
            return item;
        }

        /// <summary>
        /// 修改外网搜索关键字状态
        /// </summary>
        /// <param name="item"></param>
        public InternetKeywordInfo UpdateKeywordStatus(InternetKeywordInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateKeywordStatus");

            cmd.SetParameterValue("@SysNo", item.SysNo);
            cmd.SetParameterValue("@Status", item.Status);
            cmd.ExecuteNonQuery();
            return item;
        }

        /// <summary>
        /// 根据keyword获取InternetKeywordInfo实体
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public InternetKeywordInfo GetInternetKeywordInfoByKeyword(string keyword)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetInternetKeywordInfoByKeyword");
            cmd.SetParameterValue("@keyword", keyword);
            var result = cmd.ExecuteEntity<InternetKeywordInfo>();
            return result;
        }

        /// <summary>
        /// 根据sysNo获取InternetKeywordInfo实体
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public InternetKeywordInfo GetInternetKeywordInfoBySysNo(string sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetInternetKeywordInfoBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            var result = cmd.ExecuteEntity<InternetKeywordInfo>();
            return result;
        }
    }
}
