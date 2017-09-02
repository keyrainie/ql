using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Collections.Generic;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;
using WMSServiceInterface.ServiceInterface;

namespace ECCentral.Service.EventConsumer
{
    public class WMSHoldMessageProcessor : IConsumer<WMSHoldMessage>
    {
        public void HandleEvent(WMSHoldMessage eventMessage)
        {
            int result = WCFAdapter<IWCFSO>.GetProxy().HoldSOWithHoldTypeWHNO(eventMessage.SOSysNo, (int)eventMessage.ActionType, eventMessage.WarehouseSysNoList.ToArray(), eventMessage.UserSysNo.ToString(), eventMessage.Reason);
            switch (result)
            {
                case -1: throw new ThirdPartBizException("仓库服务异常");
                case -3: throw new ThirdPartBizException("该订单在仓库已经发生出库行为");
                case -4: throw new ThirdPartBizException("该订单在仓库已经出库");
                case -5: throw new ThirdPartBizException("该订单在仓库已经作废");
                case -6: throw new ThirdPartBizException("该订单在仓库已经被扫描");
            }
        }



        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}