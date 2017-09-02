using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.Common;

using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(LeaveWordsProcessor))]
    public class LeaveWordsProcessor
    {
        private ILeaveWordsDA leaveWordsDA = ObjectFactory<ILeaveWordsDA>.Instance;
        #region 客户留言
        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual LeaveWordsItem LoadLeaveWord(int sysNo)
        {
            return leaveWordsDA.LoadLeaveWord(sysNo);
        }

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual void UpdateLeaveWord(LeaveWordsItem item)
        {
            leaveWordsDA.UpdateLeaveWord(item);
        }

        /// <summary>
        /// 加载客户留言处理的用户列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public virtual List<UserInfo> GetLeaveWordProcessUser(string companyCode, string channelID)
        {
            return leaveWordsDA.GetLeaveWordProcessUser(companyCode, channelID);
        }

        /// <summary>
        /// 给客户发送邮件，然后更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual void SendCustomerEmailForLeaveWord(LeaveWordsItem item)
        {
            leaveWordsDA.SendMailThenUpdateLeaveWord(item);

            KeyValueVariables replaceVariables = new KeyValueVariables();
            replaceVariables.AddKeyValue(@"CustomerID", item.CustomerName);
            replaceVariables.AddKeyValue(@"Content", item.ReplyContent);
            replaceVariables.AddKeyValue(@"InDate-Y", DateTime.Now.Year.ToString());
            replaceVariables.AddKeyValue(@"InDate-M", DateTime.Now.Month.ToString());
            replaceVariables.AddKeyValue(@"InDate-D", DateTime.Now.Day.ToString());
            replaceVariables.AddKeyValue(@"Year", DateTime.Now.Year.ToString());
            if (string.IsNullOrEmpty(item.CustomerEmail))
               // throw new BizException("邮件地址为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_EmailIsNotNull"));
            else
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(item.CustomerEmail, "MKT_ReplyCustomerLeaveWords", replaceVariables, false);
        }
        #endregion
    }
}
