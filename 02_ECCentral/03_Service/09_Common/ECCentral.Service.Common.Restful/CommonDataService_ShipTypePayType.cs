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
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypePayType/QueryShipTypePayType", Method = "POST")]
        public QueryResult QueryShipTypePayType(ShipTypePayTypeQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShipTypePayTypeQueryDA>.Instance.QueryShipTypePayType(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypePayType/Create", Method = "POST")]
        public ShipTypePayTypeInfo CreateShipTypePayType(ShipTypePayTypeInfo request)
        {
            return ObjectFactory<ShipTypePayTypeAppService>.Instance.Create(request);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypePayType/Delete", Method = "DELETE")]
        public void DeleteShipTypePayType(List<int> sysNos)
        {
            ObjectFactory<ShipTypePayTypeAppService>.Instance.Delete(sysNos);
        }

    }
}
