using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum GiftCardType
    {
        /// <summary>
        /// 电子卡
        /// </summary>
        ElectronicCard = 1,

        /// <summary>
        /// 实物卡
        /// </summary>
        PhysicalCard = 2,

        /// <summary>
        /// 礼品卷
        /// </summary>
        GiftVolume = 3,

        /// <summary>
        /// 建行换礼
        /// </summary>
        GiftHExchange = 4
    }

    public enum InternalType
    {
        Normal = 1,

        CS_Card = 2,

        RMA_Card = 3
    }

    public enum GiftCardStatus
    {

    }

}
