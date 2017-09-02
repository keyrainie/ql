using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess
{

    [VersionExport(typeof(ILeaveWordsDA))]
    public class LeaveWordsDA : ILeaveWordsDA
    {
        #region 客户留言
        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="sysNo"></param>
        public LeaveWordsItem LoadLeaveWord(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LeaveWords_LoadLeaveWords");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable<CommentProcessStatus>("Status");
            return DataMapper.GetEntity<LeaveWordsItem>(dt.Rows[0]);
        }

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void UpdateLeaveWord(LeaveWordsItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LeaveWords_UpdateLeaveWords");
            dc.SetParameterValue<LeaveWordsItem>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 发送邮件后更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void SendMailThenUpdateLeaveWord(LeaveWordsItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LeaveWords_SendMailThenUpdateReplyContent");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载客户留言处理的用户列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public List<UserInfo> GetLeaveWordProcessUser(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LeaveWords_GetLeaveWordUser");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<UserInfo>();
        }


        #endregion
    }
}
