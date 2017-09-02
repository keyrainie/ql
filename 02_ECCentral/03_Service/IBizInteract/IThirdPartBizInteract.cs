using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.IBizInteract
{
    public interface IThirdPartBizInteract
    {
        /// <summary>
        /// 退预付卡
        /// </summary>
        /// <param name="refundAmount">退款金额</param>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="tNumber"></param>
        /// <param name="refundKey"></param>
        /// <returns></returns>
        int RefundPrepayCard(decimal refundAmount, int soSysNo, string tNumber, string refundKey);
    }
}
