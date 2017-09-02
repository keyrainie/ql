using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Common;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.AppService;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/ControlPanelUser/QueryUser", Method = "POST")]
        public QueryResult QueryUser(ControlPanelUserQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IControlPanelUserQueryDA>.Instance.QueryUser(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/GetControlPanelUserByLoginName", Method = "POST")]
        public ControlPanelUser GetControlPanelUserByLoginName(string loginName)
        {
            return ObjectFactory<ControlPanelUserAppService>.Instance.GetControlPanelUserByLoginName(loginName);
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/CreateUser", Method = "POST")]
        public ControlPanelUser CreateUser(ControlPanelUser request)
        {
            return ObjectFactory<ControlPanelUserAppService>.Instance.Create(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/UpdateUser", Method = "PUT")]
        public ControlPanelUser UpdateUser(ControlPanelUser request)
        {
            return ObjectFactory<ControlPanelUserAppService>.Instance.Update(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/GetUser/{sysNo}", Method = "GET")]
        public ControlPanelUser GetUserBySysNo(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            return ObjectFactory<ControlPanelUserAppService>.Instance.GetUserBySysNo(_sysNo);
        }
        [WebInvoke(UriTemplate = "/ControlPanelUser/GetIPPUserSysNo/{loginName}", Method = "GET",ResponseFormat=WebMessageFormat.Json)]
        public int GetIPPUserSysNo(string loginName)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserSysNo(loginName);           
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/LoginCount", Method = "POST")]
        public int GetCPUsersLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<ControlPanelUserAppService>.Instance.GetCPUsersLoginCount(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelUser/LoginUser", Method = "POST")]
        public ControlPanelUser GetCPUsersLoginUser(string loginName)
        {
            return ObjectFactory<ControlPanelUserAppService>.Instance.GetCPUsersLoginUser(loginName);
        }
    }
}
