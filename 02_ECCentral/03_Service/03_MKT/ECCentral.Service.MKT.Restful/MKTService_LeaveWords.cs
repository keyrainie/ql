using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private LeaveWordsAppService leaveWordsAppService = ObjectFactory<LeaveWordsAppService>.Instance;

        #region 留言管理

        /// <summary>
        /// 留言管理查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryLeaveWord", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryLeaveWord(LeaveWordQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryLeaveWord(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadLeaveWord", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual LeaveWordsItem LoadLeaveWord(int sysNo)
        {
            return leaveWordsAppService.LoadLeaveWord(sysNo);
        }

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateLeaveWord", Method = "PUT")]
        public virtual void UpdateLeaveWord(LeaveWordsItem item)
        {
            leaveWordsAppService.UpdateLeaveWord(item);
        }

        /// <summary>
        /// 加载客户留言处理的用户列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/GetLeaveWordProcessUser", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual List<UserInfo> GetLeaveWordProcessUser(string companyCode)
        {
            string channelID = "0";
            return leaveWordsAppService.GetLeaveWordProcessUser(companyCode, channelID);
        }

        /// <summary>
        /// 给客户发送邮件
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/SendCustomerEmailForLeaveWord", Method = "POST")]
        public virtual void SendCustomerEmailForLeaveWord(LeaveWordsItem item)
        {
            leaveWordsAppService.SendCustomerEmailForLeaveWord(item);
        }

        #endregion
    }
}
