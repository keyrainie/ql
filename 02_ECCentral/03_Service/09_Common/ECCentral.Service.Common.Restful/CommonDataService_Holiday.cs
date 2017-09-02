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
        [WebInvoke(UriTemplate = "/Holiday/QueryHoliday", Method = "POST")]
        public QueryResult QueryHoliday(HolidayQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IHolidayQueryDA>.Instance.QueryHoliday(request, out totalCount);
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
        [WebInvoke(UriTemplate = "/Holiday/Create", Method = "POST")]
        public void CreateHoliday(Holiday request)
        {
            ObjectFactory<HolidayAppService>.Instance.Create(request);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Holiday/Delete", Method = "DELETE")]
        public void DeleteHoliday(List<int> sysNos)
        {
            ObjectFactory<HolidayAppService>.Instance.DeleteBatch(sysNos);
        }

    }
}
