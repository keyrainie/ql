using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Enums.Promotion;


namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 限时销售
    /// </summary>
    public class CountdownInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 促销计划标题
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CountdownStatus Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 活动商品编码
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 活动商品ID
        /// </summary>
        public string ProductID { get; set; }


        /// <summary>
        /// 拿出来做活动的产品总数量
        /// </summary>
        public int CountDownQty { get; set; }

        /// <summary>
        /// 限量发售的时候，如果限制的数量卖完了，是否需要结束这个促销
        /// </summary>
        public string IsEndIfNoQty { get; set; }

        /// <summary>
        /// 促销时商品的卖价
        /// </summary>
        public decimal? CountDownCurrentPrice { get; set; }


    }
}
