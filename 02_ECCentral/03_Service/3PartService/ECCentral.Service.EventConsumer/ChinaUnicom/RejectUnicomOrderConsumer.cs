using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;

namespace ECCentral.Service.EventConsumer.ChinaUnicom
{
    public class RejectUnicomOrderConsumer : IConsumer<RejectUnicomOrderMessage>
    {
        public void HandleEvent(RejectUnicomOrderMessage eventMessage)
        {
            //service.AbandonOrder(orderInfo);
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
