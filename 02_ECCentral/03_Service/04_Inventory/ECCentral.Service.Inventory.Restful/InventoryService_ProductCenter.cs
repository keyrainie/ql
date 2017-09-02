using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        /// <summary>
        /// 备货中心 - 采购
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/CreateBasketItemsForPrepare", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public int CreateBasketItemsForPrepare(List<ProductCenterItemInfo> list)
        {
            return ObjectFactory<ProductCenterAppService>.Instance.CreateBasketItemsForPrepare(list);
        }

    }
}
