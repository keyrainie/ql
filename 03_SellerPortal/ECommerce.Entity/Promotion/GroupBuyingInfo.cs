using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums.Promotion;

namespace ECommerce.Entity.Promotion
{
    public class GroupBuyingInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string GroupBuyingTitle { get; set; }
        /// <summary>
        /// 团购简述
        /// </summary>
        public string GroupBuyingDesc { get; set; }
        /// <summary>
        /// 团购详细描述
        /// </summary>
        public string GroupBuyingDescLong { get; set; }

        /// <summary>
        /// 团购规则描述
        /// </summary>
        public string GroupBuyingRules { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 团购活动状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 时间条件：起始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 时间条件：结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 团购商品促销图,大图,需要多语言
        /// </summary>
        public string GroupBuyingPicUrl { get; set; }

        /// <summary>
        /// 团购商品促销图,中图,需要多语言
        /// </summary>
        public string GroupBuyingMiddlePicUrl { get; set; }

        /// <summary>
        /// 团购商品促销图,小图,需要多语言
        /// </summary>
        public string GroupBuyingSmallPicUrl { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 每个团购订单最大可购买该商品数量
        /// </summary>
        public int MaxPerOrder { get; set; }
        /// <summary>
        /// 限购次数(每个Customer ID)
        /// </summary>
        public int LimitOrderCount { get; set; }

        /// <summary>
        /// 团购商品可销售数量
        /// </summary>
        public int AvailableSaleQty { get; set; }

        /// <summary>
        /// 商品团购价格
        /// </summary>
        public decimal GroupBuyPrice { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品卖价
        /// </summary>
        public decimal CurrentPrice { get; set; }
    }
}
