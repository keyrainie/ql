using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    /// <summary>
    /// 商品促销信息
    /// </summary>
    public class PromoInfoModel
    {
        /// <summary>
        /// 团购活动编号
        /// </summary>
        public int GroupBuyingSysNo { get; set; }

        /// <summary>
        /// 限时抢购活动编号
        /// </summary>
        public int CountDownSysNo { get; set; }

        /// <summary>
        /// 限时抢购剩余时间(单位：秒)
        /// </summary>
        public int CountDownLeftSecond { get; set; }
        /// <summary>
        /// 套餐信息
        /// </summary>
        public List<ComboInfoModel> ComboInfo { get; set; }

        /// <summary>
        /// 赠品列表
        /// </summary>
        public List<GiftItemModel> GiftInfo { get; set; }

    }
}