using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
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
    [VersionExport(typeof(VirtualRequestAppService))]
    public class VirtualRequestAppService
    {
        VirtualRequestProcessor VirtualRequestProcessor = ObjectFactory<VirtualRequestProcessor>.Instance;
        public virtual void ApproveVirtualRequest(VirtualRequestInfo info)
        {
            VirtualRequestProcessor.ApproveRequest(info);
        }
        public virtual void RejectVirtualRequest(VirtualRequestInfo info)
        {
            VirtualRequestProcessor.RejectRequest(info);
        }
        public virtual void ApplyRequestBatch(bool canOperateItemOfLessThanPrice, bool canOperateItemOfSecondHand, List<VirtualRequestInfo> requestList)
        {
            VirtualRequestProcessor.ApplyRequestBatch(canOperateItemOfLessThanPrice, canOperateItemOfSecondHand, requestList);
        }
        public virtual string BatchApproveVirtualRequest(List<VirtualRequestInfo> infoList)
        {
            List<BatchActionItem<VirtualRequestInfo>> items = infoList.Select(x => new BatchActionItem<VirtualRequestInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            var resutl = BatchActionManager.DoBatchAction<VirtualRequestInfo, BizException>(items, (p) =>
            {
                VirtualRequestProcessor.ApproveRequest(p);
            });

            return resutl.PromptMessage;
        }
    }
}
