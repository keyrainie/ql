using System;
using System.Collections.Generic;
using System.ServiceModel.Web;

using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        private readonly AccessoryAppService _accessoryService = ObjectFactory<AccessoryAppService>.Instance;

        /// <summary>
        /// 查询配件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Accessory/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccessory(AccessoryQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAccessoryQueryDA>.Instance.QueryAccessory(request, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取配件
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Accessory/Load/{sysNo}", Method = "GET")]
        public AccessoryInfo GetAccessory(string sysNo)
        {
            int accessorySysNo;
            if (Int32.TryParse(sysNo, out accessorySysNo))
            {
                return _accessoryService.GetAccessory(accessorySysNo);
            }
            throw new ArgumentException("GetAccessory Arg Error");
        }

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Accessory/CreateAccessory", Method = "POST")]
        public AccessoryInfo CreateAccessory(AccessoryInfo accessoryInfo)
        {
            var entity = _accessoryService.CreateAccessory(accessoryInfo);
            return entity;
        }

        /// <summary>
        /// 修改配件
        /// </summary>
        /// <param name="accessoryInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Accessory/UpdateAccessory", Method = "PUT")]
        public AccessoryInfo UpdateAccessory(AccessoryInfo accessoryInfo)
        {
            var entity = _accessoryService.UpdateAccessory(accessoryInfo);
            return entity;
        }

        /// <summary>
        /// 修获取配件
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Accessory/GetAllAccessory", Method = "GET")]
        public IList<AccessoryInfo> GetAllAccessory()
        {
            var entity = _accessoryService.GetAllAccessory();
            return entity;
        }
    }
}
