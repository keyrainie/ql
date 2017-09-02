using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(LeaveWordsAppService))]
    public class LeaveWordsAppService
    {
        #region 客户留言
        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="sysNo"></param>
        public LeaveWordsItem LoadLeaveWord(int sysNo)
        {
            return ObjectFactory<LeaveWordsProcessor>.Instance.LoadLeaveWord(sysNo);
        }

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void UpdateLeaveWord(LeaveWordsItem item)
        {
            ObjectFactory<LeaveWordsProcessor>.Instance.UpdateLeaveWord(item);
        }

        /// <summary>
        /// 加载客户留言处理的用户列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public List<UserInfo> GetLeaveWordProcessUser(string companyCode, string channelID)
        {
            return ObjectFactory<LeaveWordsProcessor>.Instance.GetLeaveWordProcessUser(companyCode, channelID);
        }

        /// <summary>
        /// 给客户发送邮件
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void SendCustomerEmailForLeaveWord(LeaveWordsItem item)
        {
            ObjectFactory<LeaveWordsProcessor>.Instance.SendCustomerEmailForLeaveWord(item);
        }
        #endregion
    }
}
