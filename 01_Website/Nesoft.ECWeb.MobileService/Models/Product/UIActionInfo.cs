using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    /// <summary>
    /// 商品详情操作信息
    /// </summary>
    public class UIActionInfo
    {
        /// <summary>
        /// 操作类型,cart--加入购物车,notify--到货通知,over--已售罄,countdown--马上抢,groupbuy--我要团
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 操作类型描述
        /// </summary>
        public string ActionText { get; set; }

        public static UIActionInfo BuildActionDone()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.over.ToString();
            actionInfo.ActionText = "已结束";

            return actionInfo;
        }

        public static UIActionInfo BuildActionOver()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.over.ToString();
            actionInfo.ActionText = "已售罄";

            return actionInfo;
        }

        public static UIActionInfo BuildActionNotify()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.notify.ToString();
            actionInfo.ActionText = "到货通知";

            return actionInfo;
        }

        public static UIActionInfo BuildActionCart()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.cart.ToString();
            actionInfo.ActionText = "加入购物车";

            return actionInfo;
        }

        public static UIActionInfo BuildActionCountDown()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.countdown.ToString();
            actionInfo.ActionText = "马上抢";

            return actionInfo;
        }

        public static UIActionInfo BuildActionGroupBuy()
        {
            UIActionInfo actionInfo = new UIActionInfo();

            actionInfo.ActionType = UIActionType.groupbuy.ToString();
            actionInfo.ActionText = "我要参团";

            return actionInfo;
        }
    }
}