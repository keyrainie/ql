using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Inventory;
using System.ServiceModel.Web;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.AppService;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        /// <summary>
        /// 查询移仓篮List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryShiftRequestItemBasketList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryShiftRequestItemBasketList(ShiftRequestItemBasketQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IShiftRequestItemBasketQueryDA>.Instance.QueryShiftRequestItemBasketList(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }


        /// <summary>
        /// 批量创建移仓篮
        /// </summary>
        /// <param name="basket"></param>
        [WebInvoke(UriTemplate = "/ShiftRequest/BatchCreateShiftBasket", Method = "PUT")]
        public virtual void BatchCreateShiftBasket(ShiftRequestItemBasket basket)
        {
            ObjectFactory<ShiftRequestItemBasketAppService>.Instance.BatchCreateShiftBasket(basket);
        }

        /// <summary>
        /// 删除移仓篮Item
        /// </summary>
        /// <param name="basket"></param>
        [WebInvoke(UriTemplate = "/ShiftRequest/DeleteShiftBasket", Method = "PUT")]
        public virtual int DeleteShiftBasket(ShiftRequestItemInfo basket)
        {
            return ObjectFactory<ShiftRequestItemBasketAppService>.Instance.DeleteShiftBasket(basket);
        }

        /// <summary>
        /// 批量删除移仓篮Item
        /// </summary>
        /// <param name="shiftItemList"></param>
        [WebInvoke(UriTemplate = "/ShiftRequest/BatchDeleteShiftBasket", Method = "PUT")]
        public virtual void BatchDeleteShiftBasket(List<ShiftRequestItemInfo> shiftItemList)
        {
            ObjectFactory<ShiftRequestItemBasketAppService>.Instance.BatchDeleteShiftBasket(shiftItemList);
        }


        /// <summary>
        ///  更新移仓篮Item
        /// </summary>
        /// <param name="basket"></param>
        [WebInvoke(UriTemplate = "/ShiftRequest/UpdateShiftBasket", Method = "PUT")]
        public virtual int UpdateShiftBasket(ShiftRequestItemInfo basket)
        {
            return ObjectFactory<ShiftRequestItemBasketAppService>.Instance.UpdateShiftBasket(basket);
        }

        /// <summary>
        /// 批量更新移仓篮Item
        /// </summary>
        /// <param name="shiftItemList"></param>
        [WebInvoke(UriTemplate = "/ShiftRequest/BatchUpdateShiftBasket", Method = "PUT")]
        public virtual void BatchUpdateShiftBasket(List<ShiftRequestItemInfo> shiftItemList)
        {
            ObjectFactory<ShiftRequestItemBasketAppService>.Instance.BatchUpdateShiftBasket(shiftItemList);
        }

        /// <summary>
        /// 批量创建移仓单
        /// </summary>
        /// <param name="shiftInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/BatchCreateShiftRequest", Method = "PUT")]
        public virtual string BatchCreateShiftRequest(List<ShiftRequestInfo> shiftInfo)
        {
            return ObjectFactory<ShiftRequestItemBasketAppService>.Instance.BatchCreateShiftRequest(shiftInfo);
        }
    }
}
