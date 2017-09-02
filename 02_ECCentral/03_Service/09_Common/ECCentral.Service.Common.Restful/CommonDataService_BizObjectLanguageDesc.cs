using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.AppService;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        //[WebInvoke(UriTemplate = "/BizObject/Get/{bizObjectType}/{bizObjectSysNo}", Method = "GET")]

        [WebInvoke(UriTemplate = "/BizObject/Get/{bizObjectType}/{bizObjectSysNo}/{bizObjectId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<BizObjectLanguageDesc> GetBizObjectLanguageDescList(string bizObjectType, string bizObjectSysNo, string bizObjectId)
        {
            int temp = 0;
            if (!string.IsNullOrEmpty(bizObjectSysNo) && int.TryParse(bizObjectSysNo, out temp))
            {
                return ObjectFactory<BizObjecLanguageDescService>.Instance.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, bizObjectId);
            }
            return ObjectFactory<BizObjecLanguageDescService>.Instance.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, bizObjectId);
            
        }

        [WebInvoke(UriTemplate = "/BizObject/GetBySysNo/{bizObjectType}/{bizObjectSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<BizObjectLanguageDesc> GetBizObjectLanguageDescListByObjectSysNo(string bizObjectType, string bizObjectSysNo)
        {
            int temp = 0;
            if (!string.IsNullOrEmpty(bizObjectSysNo) && int.TryParse(bizObjectSysNo, out temp))
            {
                return ObjectFactory<BizObjecLanguageDescService>.Instance.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, string.Empty);
            }
            return ObjectFactory<BizObjecLanguageDescService>.Instance.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, string.Empty);

        }



        [WebInvoke(UriTemplate = "/BizObject/Create", Method = "POST")]
        public bool InsertBizObjectLanguageDesc(BizObjectLanguageDesc request)
        {
            return ObjectFactory<BizObjecLanguageDescService>.Instance.InsertBizObjectLanguageDesc(request);
        }




        [WebInvoke(UriTemplate = "/BizObject/Update", Method = "PUT")]
        public bool UpdateBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            return ObjectFactory<BizObjecLanguageDescService>.Instance.UpdateBizObjectLanguageDesc(entity);
        }
    }
}
