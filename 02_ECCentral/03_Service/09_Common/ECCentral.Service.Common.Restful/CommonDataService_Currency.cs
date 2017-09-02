using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Common.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/Currency/QueryCurrencyList", Method = "GET")]
        public List<CurrencyInfo> QueryCurrencyList()
        {
            return ObjectFactory<ICurrencyDA>.Instance.QueryCurrencyList();
        }

        [WebInvoke(UriTemplate = "/Currency/CreateCurrency", Method = "POST")]
        public CurrencyInfo CreateCurrency(CurrencyInfo entity)
        {
            return ObjectFactory<ICurrencyDA>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/Currency/UpdateCurrency", Method = "PUT")]
        public CurrencyInfo UpdateCurrency(CurrencyInfo entity)
        {
            return ObjectFactory<ICurrencyDA>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/Currency/LoadCurrency/{sysNo}", Method = "GET")]
        public CurrencyInfo LoadCurrency(string sysNo)
        {
            int num = int.Parse(sysNo);
            return ObjectFactory<ICurrencyDA>.Instance.Load(num);
        }

    }
}
