using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/SysConfig/Query", Method = "POST")]
        public virtual QueryResult SysConfigQuery(SysConfigQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<ISysConfigQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/SysConfig/Update", Method = "PUT")]
        public virtual void SysConfigUpdate(List<SysConfigReq> msg)
        {
            msg.ForEach(item =>
            {
                ObjectFactory<SysConfigAppService>.Instance.Update(item.Key, item.Value, item.ChannlID);
            });
        }
    }
}
