using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface IDeliveryDA
    {
        /// <summary>
        ///  修改配送状态(info.Status,info.Note)，根据配送的订单编号(info.SOSysNo)、配送原因(info.Reason)和配送状态(oldStatus)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="oldStatus">配送状态</param>
        void UpdateLastOKStatus(DeliveryInfo info, DeliveryStatus oldStatus);

        /// <summary>
        /// 取得配送信息
        /// </summary>
        /// <param name="type">配送类型</param>
        /// <param name="orderSysNo">单据编号</param>
        /// <param name="status">配送状态</param>
        /// <returns></returns>
        DeliveryInfo GetDeliveryInfo(DeliveryType type, int orderSysNo, DeliveryStatus status);
    }
}
