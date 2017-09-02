using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public enum UIActionType
    {
        /// <summary>
        /// 加入购物车
        /// </summary>
        cart,
        /// <summary>
        /// 到货通知
        /// </summary>
        notify,
        /// <summary>
        /// 已售罄
        /// </summary>
        over,
        /// <summary>
        /// 马上抢
        /// </summary>
        countdown,
        /// <summary>
        /// 我要团
        /// </summary>
        groupbuy
    }
}