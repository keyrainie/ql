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
        [WebInvoke(UriTemplate = "/ControlPanelSociety/QuerySociety", Method = "POST")]
        public QueryResult QuerySociety(ControlPanelSocietyQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IControlPanelSocietyQueryDA>.Instance.QuerySociety(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/GetControlPanelSocietyByLoginName", Method = "POST")]
        public ControlPanelSociety GetControlPanelSocietyByLoginName(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetControlPanelSocietyByLoginName(loginName);
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/CreateSociety", Method = "POST")]
        public ControlPanelSociety CreateSociety(ControlPanelSociety request)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.Create(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/UpdateSociety", Method = "PUT")]
        public ControlPanelSociety UpdateSociety(ControlPanelSociety request)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.Update(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/GetSociety/{sysNo}", Method = "GET")]
        public ControlPanelSociety GetSocietyBySysNo(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetSocietyBySysNo(_sysNo);
        }
        [WebInvoke(UriTemplate = "/ControlPanelSociety/GetIPPSocietySysNo/{loginName}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public int GetIPPSocietySysNo(string loginName)
        {
            //return ObjectFactory<CommonDataAppService>.Instance.GetSocietySysNo(loginName);
            return 0;
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/LoginCount", Method = "POST")]
        public int GetCPSocietysLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetCPSocietysLoginCount(request);
        }

        [WebInvoke(UriTemplate = "/ControlPanelSociety/LoginSociety", Method = "POST")]
        public ControlPanelSociety GetCPSocietysLoginSociety(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetCPSocietysLoginSociety(loginName);
        }


        [WebInvoke(UriTemplate = "/ControlPanelSociety/GetSocietyProvince_ComBox", Method = "POST")]
        public List<ComBoxData> GetSocietyProvince_ComBox(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetSocietyProvince_ComBox(loginName);
        }
        [WebInvoke(UriTemplate = "/ControlPanelSociety/GetSocietyCommissionType_ComBox", Method = "POST")]
        public List<ComBoxData> GetSocietyCommissionType_ComBox(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyAppService>.Instance.GetSocietyCommissionType_ComBox(loginName);
        }
    }
}
