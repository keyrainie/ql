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
        private ShipTypeAppService shipTypeAppService = ObjectFactory<ShipTypeAppService>.Instance;

        /// <summary>
        /// 获取配送方式查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeInfo/QueryShipTypeList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]        
        public virtual QueryResult QueryShipTypeList(ShipTypeQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShipTypeQueryDA>.Instance.QueryShipTypeList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 新增配送方式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeInfo/Create", Method = "POST")]
        public virtual void CreateShipTypeList(ShippingType entity)
        {
            shipTypeAppService.CreateShiType(entity);
        }
        /// <summary>
        /// 加载配送方式
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeInfo/Load/{sysNo}", Method = "GET")]
        public virtual ShippingType LoadShipType(string sysNo)
        {
             int _sysNo =int.Parse(sysNo); 
             return  shipTypeAppService.LoadShiType(_sysNo);
        }
        /// <summary>
        /// 更新配送方式
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeInfo/Update", Method = "PUT")]
        public virtual void UpdateShipType(ShippingType entity)
        {
            shipTypeAppService.UpdateShiType(entity);
        }

    }
}
