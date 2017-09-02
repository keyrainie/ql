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
        private SaleDiscountRuleAppService _appServiceSaleDiscountRule = ObjectFactory<SaleDiscountRuleAppService>.Instance;
        /// <summary>
        /// 分页查询限购规则
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SaleDiscountRule/Query", Method = "POST")]
        public QueryResult QuerySaleDiscountRule(SaleDiscountRuleQueryFilter filter)
        {
            int totalCount = 0;
            var dt = ObjectFactory<ISaleDiscountRuleQueryDA>.Instance.Query(filter, out totalCount);
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dt;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        [WebGet(UriTemplate = "/SaleDiscountRule/{sysNo}")]
        public SaleDiscountRule LoadSaleDiscountRule(string sysNo)
        {
            int no;
            if (!int.TryParse(sysNo, out no)) return null;
            return _appServiceSaleDiscountRule.Load(no);
        }

        [WebInvoke(UriTemplate = "/SaleDiscountRule/Create", Method = "POST")]
        public void InsertSaleDiscountRule(SaleDiscountRule data)
        {
            _appServiceSaleDiscountRule.Insert(data);
        }

        [WebInvoke(UriTemplate = "/SaleDiscountRule/Update", Method = "PUT")]
        public void UpdateSaleDiscountRule(SaleDiscountRule data)
        {
            _appServiceSaleDiscountRule.Update(data);
        }
    }
}
