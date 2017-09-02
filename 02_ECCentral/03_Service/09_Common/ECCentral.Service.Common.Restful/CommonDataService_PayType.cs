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
        [WebInvoke(UriTemplate = "/PayType/QueryPayType", Method = "POST")]
        public QueryResult QueryPayType(PayTypeQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IPayTypeQueryDA>.Instance.QueryPayType(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建支付方式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PayType/Create", Method = "POST")]
        public PayType CreatePayType(PayType request)
        {
            return ObjectFactory<PayTypeAppService>.Instance.Create(request);
        }

        /// <summary>
        /// 更新支付方式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PayType/Update", Method = "PUT")]
        public PayType UpdatePayType(PayType request)
        {
            return ObjectFactory<PayTypeAppService>.Instance.Update(request);
        }

        /// <summary>
        /// 加载支付方式
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PayType/Load/{sysNo}", Method = "GET")]
        public PayType LoadPayType(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            return ObjectFactory<PayTypeAppService>.Instance.LoadPayType(_sysNo);
        }
    }
}
