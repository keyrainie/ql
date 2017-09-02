using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;

using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 团购分页查询服务
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuyingLottery/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryGroupBuyingLottery(GroupBuyingLotteryQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IGroupBuyingLotteryQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = ds.Tables[0],
                TotalCount = totalCount
            };
        }

    }
}
