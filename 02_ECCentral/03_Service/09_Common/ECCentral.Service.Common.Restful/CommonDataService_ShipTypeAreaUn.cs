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
        /// 获取配送方式-地区（非）查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaUnInfo/QueryShipTypeAreaUnList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual QueryResult QueryShipTypeAreaUnList(ShipTypeAreaUnQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShipTypeAreaUnQueryDA>.Instance.QueryShipTypeAreaUnList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 删除配送方式-地区（非）
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShipTypeAreaUnInfo/Void", Method = "DELETE")]
        public virtual void VoidShipTypeAreaUn(List<int> sysnoList)
        {
            ObjectFactory<ShipTypeAreaUnAppService>.Instance.VoidShipTypeAreaUn(sysnoList);
        }
        /// <summary>
        /// 新增配送方式-地区（非）
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate="/ShipTypeAreaUnInfo/Create",Method="POST")]
        public virtual ErroDetail CreateShipTypeAreaUn(ShipTypeAreaUnInfo entity)
        {
            return ObjectFactory<ShipTypeAreaUnAppService>.Instance.CreateShipTypAreaUn(entity);
        }
    }
}
