using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.AppService;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        [WebInvoke(UriTemplate = "/InventoryStock/QueryCardItemInventoryStock", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCardItemInventoryStock(InventoryQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            //如果是查询总库存，则调用QueryInventory的service,否则调用QueryInventoryStock:
            if (queryFilter.IsShowTotalInventory.HasValue && queryFilter.IsShowTotalInventory.Value == true)
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductInventoryTotal(queryFilter, out getTotalCount);

            }
            else
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductInventoryByStock(queryFilter, out getTotalCount);
            }
            result.TotalCount = getTotalCount;
            return result;
        }

        [WebInvoke(UriTemplate = "/InventoryStock/QueryCardItemOrders", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCardItemOrders(InventoryItemCardQueryFilter queryFilter)
        {
            //获取RMAInventoryOnlineDate:
            queryFilter.RMAInventoryOnlineDate = ObjectFactory<InventoryItemCardAppService>.Instance.GetRMAInventoryOnlineDateForItemCardQuery("RMAInventoryOnlineDate", queryFilter.CompanyCode);

            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IInventoryCardItemsQueryDA>.Instance.QueryCardItemOrdersRelated(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }
    }
}
