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
using System.Data;
using ECCentral.Service.Common.Restful.ResponseMsg;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/AreaDelivery/QueryAreaDelivery", Method = "POST")]
        public QueryResult QueryAreaDelivery(AreaDeliveryQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAreaDeliveryQueryDA>.Instance.QueryAreaDelivery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/AreaDelivery/GetWHArea", Method = "GET")]
        public List<AreaDelidayResponse> QueryWHArea()
        {
            DataTable dataTable = ObjectFactory<IAreaDeliveryQueryDA>.Instance.QueryWHArea();

            List<AreaDelidayResponse> tmpList = new List<AreaDelidayResponse>();

            foreach (DataRow dr in dataTable.Rows)
                tmpList.Add(new AreaDelidayResponse() { WHArea = dr[0].ToInteger(), City = dr[1].ToString() });

            return tmpList;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AreaDelivery/Create", Method = "POST")]
        public AreaDeliveryInfo CreateAreaDelivery(AreaDeliveryInfo request)
        {
            return ObjectFactory<AreaDeliveryAppService>.Instance.Create(request);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AreaDelivery/Update", Method = "PUT")]
        public AreaDeliveryInfo UpdateAreaDelivery(AreaDeliveryInfo request)
        {
            return ObjectFactory<AreaDeliveryAppService>.Instance.Update(request);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/AreaDelivery/Delete", Method = "DELETE")]
        public void DeleteAreaDelivery(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            ObjectFactory<AreaDeliveryAppService>.Instance.Delete(_sysNo);
        }

        /// <summary>
        /// 根据系统id加载记录
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AreaDelivery/Load/{sysNo}", Method = "GET")]
        public AreaDeliveryInfo GetAreaDeliveryInfoByID(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            return ObjectFactory<AreaDeliveryAppService>.Instance.GetAreaDeliveryInfoByID(_sysNo);
        }
    }
}
