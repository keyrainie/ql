using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurgeTool/GetPurgeToolByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetPurgeToolByQuery(PurgeToolQueryFilter query)
        {
            int totalCount;
            var dt = ObjectFactory<IPurgeToolDA>.Instance.GetPurgeToolByQuery(query, out totalCount);


            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurgeTool/CreatePurgeTool", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void CreatePurgeTool(List<PurgeToolInfo> list)
        {
            ObjectFactory<PurgeToolAppService>.Instance.CreatePurgeTool(list);
        }
    }
}
