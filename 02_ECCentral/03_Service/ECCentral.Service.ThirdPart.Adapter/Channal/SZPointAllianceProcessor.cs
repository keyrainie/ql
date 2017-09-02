using ECCentral.BizEntity;
using ECCentral.Service.EventConsumer;
using ECCentral.Service.EventMessage.SZPointAlliance;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.ThirdPart.Adapter
{
    [VersionExport(typeof(IThirdPartBizInteract))]
    public class SZPointAllianceProcessor : IThirdPartBizInteract
    {
        public int RefundPrepayCard(decimal refundAmount, int soSysNo, string tNumber, string refundKey)
        {
            SZPointAllianceRequestMessage request = new SZPointAllianceRequestMessage()
            {
                RefundAmount = refundAmount,
                RefundDescription = string.Empty,
                RefundKey = refundKey,
                RefundType = PointAllianceRefundType.PrepaidCard,
                SOSysNo = soSysNo,
                TNumber = tNumber
            };
            int result = -1;

            try
            {
                SZPointAllianceResponseMessage response = Refund(request);
                result = response.Result;
                if (result == 2)
                {
                    throw new BizException(response.Message);
                }
            }
            catch (Exception e)
            {
                throw new BizException(e.Message);
            }
            return result;
        }

        private SZPointAllianceResponseMessage Refund(SZPointAllianceRequestMessage req)
        {
            SZPointAllianceConsumer consumer = new SZPointAllianceConsumer();
            consumer.HandleEvent(req);
            return consumer.SZResponse;
        }
    }
}
