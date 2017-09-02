using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        /// <summary>
        /// 处理从SellerPortal发过来的SSB消息
        /// </summary>
        /// <param name="reqMsg"></param>
        [WebInvoke(UriTemplate = "/Inventory/ProcessSellerPortalMessage", Method = "PUT")]
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            ObjectFactory<InventorySellerMessageAppService>.Instance.ProcessSellerPortalMessage(reqMsg);
        }
    }
}
