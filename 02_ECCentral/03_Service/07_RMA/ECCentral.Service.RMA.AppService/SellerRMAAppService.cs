using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(SellerRMAAppService))]
    public class SellerRMAAppService
    {
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            switch (reqMsg.ActionType)
            {
                case "RMAReject":
                    ProcessRMARejectMessage(reqMsg.Message);
                    break;
                default:
                    break;
            }
        }

        private void ProcessRMARejectMessage(string message)
        {
            var requestProcessor = ObjectFactory<SellerPortalRequestProcessor>.Instance;
            var refundProcessor = ObjectFactory<SellerPortalRefundProcessor>.Instance;

            RMARejectProInfo rejectInfo = SerializationUtility.XmlDeserialize<RMARejectProInfo>(message);
            if (rejectInfo != null)
            {
                foreach (var rejectOrder in rejectInfo.Node.RMARejectRequestRoot.Body.RMARejectMsg.OrdersList)
                {
                    try
                    {
                        requestProcessor.CreateRequest4AutoRMA(rejectOrder.SONumber, rejectInfo.Node.RMARejectRequestRoot.Body.RMARejectMsg.InUser);
                    }
                    catch { }
                }
            }
        }
    }
}
