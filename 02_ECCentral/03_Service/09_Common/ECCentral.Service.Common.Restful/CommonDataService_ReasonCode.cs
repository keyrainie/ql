using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.AppService;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/ReasonCode/InsertReasonCode", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public ReasonCodeEntity InsertReasonCode(ReasonCodeEntity entity)
        {
            return ObjectFactory<ReasonCodeService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/ReasonCode/UpdateReasonCode", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateReasonCode(ReasonCodeEntity query)
        {
            ObjectFactory<ReasonCodeService>.Instance.Update(query);
        }

        [WebInvoke(UriTemplate = "/ReasonCode/UpdateReasonStatusList", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateReasonStatusList(List<ReasonCodeEntity> list)
        {
            ObjectFactory<ReasonCodeService>.Instance.UpdateStatusList(list);
        }

        [WebInvoke(UriTemplate = "/ReasonCode/GetReasonCodeBySysNo/{sysNO}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public ReasonCodeEntity GetReasonCodeBySysNo(string sysNo)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetReasonCodeBySysNo(int.Parse(sysNo));
        }

        [WebInvoke(UriTemplate = "/ReasonCode/GetReasonCodePath/{sysNO}/{companyCode}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public string GetReasonCodePath(string sysNo, string companyCode)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetReasonCodePath(int.Parse(sysNo),companyCode);
        }

        [WebInvoke(UriTemplate = "/ReasonCode/GetReasonCodeByNodeLevel/{nodeLevel}", Method = "GET")]
        public List<ReasonCodeEntity> GetReasonCodeByNodeLevel(string nodeLevel)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetReasonCodeByNodeLevel(int.Parse(nodeLevel), "8601");
        }

        [WebInvoke(UriTemplate = "/ReasonCode/GetChildrenReasonCode/{SysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<ReasonCodeEntity> GetChildrenReasonCode(string SysNo)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetChildrenReasonCode(int.Parse(SysNo));
        }

        [WebInvoke(UriTemplate = "/ReasonCode/GetReasonCodePathList", Method = "POST")]
        public List<ReasonCodeEntity> GetReasonCodePathList(List<ReasonCodeEntity> list)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetReasonCodePathList(list);
        }
    }
}
