using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.ThirdPart.Interface
{
    /// <summary>
    /// 神州积分联盟，退积分，退预付卡
    /// </summary>
    public interface ISZPointAlliance
    {
        int RefundPrepayCard(decimal refundAmount, int soSysNo, string tNumber, string refundKey);
    }
}
