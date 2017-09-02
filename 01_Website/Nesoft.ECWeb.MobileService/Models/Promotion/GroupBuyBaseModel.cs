using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class GroupBuyBaseModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

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
        /// 价格信息
        /// </summary>
        public SalesInfoModel Price { get; set; }
    }
}