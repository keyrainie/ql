using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    /// <summary>
    /// 处理Seller发过来的SSB消息 - AppService
    /// </summary>
    [VersionExport(typeof(InventorySellerMessageAppService))]
    public class InventorySellerMessageAppService
    {
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            switch (reqMsg.ActionType)
            {
                //库存调整
                case "AdjustInventory":
                    ObjectFactory<InventorySellerMessageProcessor>.Instance.AdjustSellerInventory(reqMsg.Message);
                    break;
                //构建 单据创建 或更新 调整批次库存表的SSB消息，批次功能已去除，故此消息不处理
                case "ProductBatchUpdate":
                    break;
                //从WMS过来的盘点消息创建损益单
                //todo:盘点消息创建损益单 这一块暂时从IPP直接移植过来，直接调用SP,后面还是需要遵从ECC规范来做
                case "WMSCheck":
                    break;
                //目前已无此类型的消息
                case "UpdateBatchInfo":
                    break;
                default:
                    break;
            }
        }
    }
}
