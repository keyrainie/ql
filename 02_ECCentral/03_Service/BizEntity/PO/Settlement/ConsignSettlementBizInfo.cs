using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 用于查询代销结算单时，根据不同权限获取PMList
    /// </summary>
    public class ConsignSettlementBizInfo
    {
        public PMQueryType QueryType{get;set;} 

        public string CurrentUserName{get;set;}

        public string CompanyCode { get; set; }
    }
}
