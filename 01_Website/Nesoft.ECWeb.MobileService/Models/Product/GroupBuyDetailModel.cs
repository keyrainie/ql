using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class GroupBuyDetailModel
    {
        /// <summary>
        /// 团购活动编号
        /// </summary>
        public int GroupBuyingSysNo { get; set; }

        /// <summary>
        /// 团购活动标题
        /// </summary>
        public string GroupBuyingTitle { get; set; }

        /// <summary>
        /// 团购活动图片
        /// </summary>
        public string GroupBuyingPicUrl { get; set; }

        /// <summary>
        /// 当前参团人数
        /// </summary>
        public int CurrentSellCount { get; set; }

        /// <summary>
        /// 活动状态
        /// </summary>
        public string SellStatusStr { get; set; }

        /// <summary>
        /// 好多折
        /// </summary>
        public string DiscountStr { get; set; }

        /// <summary>
        /// 活动剩余时间(单位秒)
        /// </summary>
        public long LeftSeconds { get; set; }

        /// <summary>
        /// 节省金额
        /// </summary>
        public decimal SaveMoney { get; set; }

        /// <summary>
        /// 团购活动简短描述
        /// </summary>
        public string GroupBuyingDesc { get; set; }
        /// <summary>
        /// 团购活动长描述
        /// </summary>
        public string GroupBuyingDescLong { get; set; }

        /// <summary>
        /// 团购规则描述
        /// </summary>
        public string GroupBuyingRules { get; set; }

        /// <summary>
        /// 图文描述
        /// </summary>
        public string ProductPhotoDesc { get; set; }
    }
}