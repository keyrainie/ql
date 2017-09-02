using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        [WebInvoke(UriTemplate = "/ConsignAdjust/QueryConsignAdjustList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConsignAdjustList(ConsignAdjustQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IConsignAdjustDA>.Instance.Query(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/ConsignAdjust/CreateConsignAdjust", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public ConsignAdjustInfo CreateConsignAdjust(ConsignAdjustInfo request)
        {
            return ObjectFactory<ConsignAdjustAppService>.Instance.Create(request);
        }

        [WebInvoke(UriTemplate = "/ConsignAdjust/LoadConsignAdjust/{SysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public ConsignAdjustInfo LoadConsignAdjustInfo(string SysNo)
        {
            return ObjectFactory<IConsignAdjustDA>.Instance.LoadInfo(Convert.ToInt32(SysNo));
        }

        [WebInvoke(UriTemplate = "/ConsignAdjust/UpdateConsignAdjust", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public ConsignAdjustInfo UpdateConsignAdjustInfo(ConsignAdjustInfo request)
        {
            return ObjectFactory<IConsignAdjustDA>.Instance.Update(request);
        }

        /// <summary>
        /// 更改状态的操作集成，包含作废，审核等
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignAdjust/MaintainConsignAdjustStatus", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public ConsignAdjustInfo MaintainConsignAdjustInfoStatus(ConsignAdjustInfo request)
        {
            return ObjectFactory<ConsignAdjustAppService>.Instance.MaintainStatus(request);
        }

        [WebInvoke(UriTemplate = "/ConsignAdjust/DelConsignAdjust", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public ConsignAdjustInfo DelConsignAdjustInfo(int sysNo)
        {
          return  ObjectFactory<IConsignAdjustDA>.Instance.Delete(sysNo);
        }
    }
}
