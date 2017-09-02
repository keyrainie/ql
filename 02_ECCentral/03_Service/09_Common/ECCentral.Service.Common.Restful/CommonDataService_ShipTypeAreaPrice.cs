using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Common.AppService;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful
{
   public partial class CommonDataService
    {
        /// <summary>
        /// 获取配送方式-地区-价格查询列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaPriceInfo/QueryShipTypeAreaPriceList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual QueryResult QueryShipTypeAreaPriceList(ShipTypeAreaPriceQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShipTypeAreaPriceQueryDA>.Instance.QueryShipTypeAreaPriceList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 新增配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaPriceInfo/Create", Method = "POST")]
        public virtual ShipTypeAreaPriceInfo CreateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
           return  ObjectFactory<ShipTypeAreaPriceAppService>.Instance.CreateShipTypeAreaPrice(entity);
        }
        /// <summary>
        /// 删除配送方式-地区-价格
        /// </summary>
        /// <param name="sysnoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaPriceInfo/Void", Method = "DELETE")]
        public virtual void VoidShipTypeAreaPrice(List<int> sysnoList)
        {
            ObjectFactory<ShipTypeAreaPriceAppService>.Instance.VoidShipTypeAreaPrice(sysnoList);
        }
        /// <summary>
        /// 更新配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaPriceInfo/Update", Method = "PUT")]
        public  void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            ObjectFactory<ShipTypeAreaPriceAppService>.Instance.UpdateShipTypeAreaPrice(entity);
        }
    }
}
