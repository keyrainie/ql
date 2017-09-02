using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private BuyLimitRuleAppService _appServiceBuyLimitRule = ObjectFactory<BuyLimitRuleAppService>.Instance;
        /// <summary>
        /// 分页查询限购规则
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/BuyLimitRule/Query", Method = "POST")]
        public QueryResult QueryBuyLimitRule(BuyLimitRuleQueryFilter filter)
        {
            int totalCount = 0;
            var dt = ObjectFactory<IBuyLimitRuleQueryDA>.Instance.Query(filter, out totalCount);
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dt;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        [WebGet(UriTemplate = "/BuyLimitRule/{sysNo}")]
        public BuyLimitRule LoadBuyLimitRule(string sysNo)
        {
            int no;
            if (!int.TryParse(sysNo, out no)) return null;
            return _appServiceBuyLimitRule.Load(no);
        }

        [WebInvoke(UriTemplate = "/BuyLimitRule/Create", Method = "POST")]
        public void InsertBuyLimitRule(BuyLimitRule data)
        {
            _appServiceBuyLimitRule.Insert(data);
        }

        [WebInvoke(UriTemplate = "/BuyLimitRule/Update", Method = "PUT")]
        public void UpdateBuyLimitRule(BuyLimitRule data)
        {
            _appServiceBuyLimitRule.Update(data);
        }
    }
}
