using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        /// <summary>
        /// 代收结算单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryAdventProductsList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAdventProductsList(AdventProductsQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IBatchManagementQueryDA>.Instance.QueryAdventProductsList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }
        [WebInvoke(UriTemplate = "/Inventory/UpdateProductRingDayInfo", Method = "PUT")]
        public ProductRingDayInfo UpdateProductRingDayInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<BatchManagementAppService>.Instance.UpdateProductRingInfo(entity);
            return entity;
        }

        [WebInvoke(UriTemplate = "/Inventory/AddProductRingDayInfo", Method = "PUT")]
        public ProductRingDayInfo AddProductRingDayInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<BatchManagementAppService>.Instance.AddProductRingInfo(entity);
            return entity;
        }
    }
}
