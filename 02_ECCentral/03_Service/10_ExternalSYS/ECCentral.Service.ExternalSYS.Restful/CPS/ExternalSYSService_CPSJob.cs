using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.Service.ExternalSYS.AppService.CPS;
using ECCentral.Service.Utility;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 获取待结算用户列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsJob/GetSettledUserSysNoList", Method = "GET")]
        public List<int> GetSettledUserSysNoList()
        {
            return ObjectFactory<CPSJOBAppService>.Instance.GetSettledUserSysNoList();
        }

        /// <summary>
        /// 处理用户佣金结算信息
        /// </summary>
        /// <param name="userSysNo"></param>
        [WebInvoke(UriTemplate = "/CpsJob/ProcessUserSettledCommissionInfo", Method = "PUT")]
        public void ProcessUserSettledCommissionInfo(int userSysNo)
        {
            ObjectFactory<CPSJOBAppService>.Instance.ProcessUserSettledCommissionInfo(userSysNo);
        }

        /// <summary>
        /// 获取拥有待处理的佣金结算单的用户列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsJob/GetPendingCommissionSettlementUserList", Method = "GET")]
        public List<int> GetPendingCommissionSettlement()
        {
            return ObjectFactory<CPSJOBAppService>.Instance.GetPendingCommissionSettlement();
        }

        /// <summary>
        /// 自动处理用户佣金结算单、并提交兑现单
        /// </summary>
        /// <param name="userSysNo"></param>
        [WebInvoke(UriTemplate = "/CpsJob/ProcessUserAutoSettledCommissionInfo", Method = "PUT")]
        public void ProcessUserAutoSettledCommissionInfo(int userSysNo)
        {
            ObjectFactory<CPSJOBAppService>.Instance.ProcessUserAutoSettledCommissionInfo(userSysNo);
        }
    }
}
