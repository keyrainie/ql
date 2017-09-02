using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Common.AppService;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        /// <summary>
        /// 删除配送方式-产品
        /// </summary>
        /// <param name="sysnoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeProductInfo/Void", Method = "DELETE")]
        public virtual void VoidShipTypeProduct(List<int?> sysnoList)
        {
            //int id = int.Parse(bannerLocationSysNo);
            ObjectFactory<ShipTypeProductAppService>.Instance.VoidShipTypeProduct(sysnoList);
        }
        /// <summary>
        /// 新增配送方式-产品
        /// </summary>
        /// <param name="ShipTypeProductInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeProductInfo/Create", Method = "POST")]
        public virtual void CreateShipTypeProduct(ShipTypeProductInfo ShipTypeProductInfo)
        {
            //int id = int.Parse(bannerLocationSysNo);
            ObjectFactory<ShipTypeProductAppService>.Instance.CreateShipTypeProduct(ShipTypeProductInfo);
        }
        /// <summary>
        /// 获取配送方式-产品查询列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeProductInfo/QueryShipTypeProductList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual QueryResult QueryShipTypeProductList(ShipTypeProductQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShipTypeProductQueryDA>.Instance.QueryShipTypeProductList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
    }
}
