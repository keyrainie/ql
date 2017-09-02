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
        /// 中奖结果分页查询服务
        /// </summary>
        [WebInvoke(UriTemplate = "/Lottery/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryLottery(LotteryQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<ILotteryQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = ds.Tables[0],
                TotalCount = totalCount
            };
        }

    }
}
