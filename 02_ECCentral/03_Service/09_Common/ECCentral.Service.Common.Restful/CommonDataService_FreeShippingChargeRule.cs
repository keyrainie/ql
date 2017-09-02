using System.ServiceModel.Web;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryRules(FreeShippingChargeRuleQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IFreeShippingChargeRuleQueryDA>.Instance.Query(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/{id}", Method = "GET")]
        public FreeShippingChargeRuleInfo LoadRuleBySysNo(string id)
        {
            int sysno;
            if (int.TryParse(id, out sysno))
            {
                return ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Load(sysno);
            }
            return null;
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Create", Method = "POST")]
        public FreeShippingChargeRuleInfo CreateRule(FreeShippingChargeRuleInfo entity)
        {
            return ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Update", Method = "PUT")]
        public void UpdateRule(FreeShippingChargeRuleInfo entity)
        {
            ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Valid", Method = "PUT")]
        public void ValidRule(string id)
        {
            int sysno;
            if (int.TryParse(id, out sysno))
            {
                ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Valid(sysno);
            }
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Invalid", Method = "PUT")]
        public void InvalidRule(string id)
        {
            int sysno;
            if (int.TryParse(id, out sysno))
            {
                ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Invalid(sysno);
            }
        }

        [WebInvoke(UriTemplate = "/FreeShippingChargeRule/Delete", Method = "DELETE")]
        public void DeleteRule(string id)
        {
            int sysno;
            if (int.TryParse(id, out sysno))
            {
                ObjectFactory<FreeShippingChargeRuleAppService>.Instance.Delete(sysno);
            }
        }
    }
}