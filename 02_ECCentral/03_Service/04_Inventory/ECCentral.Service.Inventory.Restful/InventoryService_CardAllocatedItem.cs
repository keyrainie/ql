using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        [WebInvoke(UriTemplate = "/InventoryStock/QueryAllocatedItemInventoryStock", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAllocatedItemInventoryStock(InventoryQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryInventoryStockList(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }

        [WebInvoke(UriTemplate = "/InventoryStock/QueryAllocatedItemOrders", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAllocatedItemOrders(InventoryAllocatedCardQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IInventoryAllocatedItemsQueryDA>.Instance.QueryAllocatedItemOrdersRelated(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }
    }
}
