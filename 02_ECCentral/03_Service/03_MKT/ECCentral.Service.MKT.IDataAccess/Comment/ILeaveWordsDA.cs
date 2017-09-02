using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ILeaveWordsDA
    {
        #region 客户留言
        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="sysNo"></param>
        LeaveWordsItem LoadLeaveWord(int sysNo);

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        void UpdateLeaveWord(LeaveWordsItem item);

        /// <summary>
        /// 发送邮件后更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        void SendMailThenUpdateLeaveWord(LeaveWordsItem item);

        /// <summary>
        /// 加载客户留言处理的用户列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        List<UserInfo> GetLeaveWordProcessUser(string companyCode, string channelID);
        #endregion
    }
}
