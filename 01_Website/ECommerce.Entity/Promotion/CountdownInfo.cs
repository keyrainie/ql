using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 限时销售：限时抢购，限时促销，秒杀
    /// </summary>
    public class CountdownInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 促销计划标题
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 活动商品编码
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// 促销时商品的卖价
        /// </summary>
        public decimal? CountDownCurrentPrice { get; set; }

        /// <summary>
        /// 促销时商品折扣
        /// </summary>
        public decimal? CountDownCashRebate { get; set; }

        /// <summary>
        /// 促销活动赠送积分数量
        /// </summary>
        public int? CountDownPoint { get; set; }

        /// <summary>
        /// 商品原价
        /// </summary>
        public decimal SnapShotCurrentPrice { get; set; }

        /// <summary>
        /// 商品原返现金额
        /// </summary>
        public decimal SnapShotCashRebate { get; set; }

        /// <summary>
        /// 商品原赠送积分
        /// </summary>
        public decimal SnapShotPoint { get; set; }

        /// <summary>
        /// 是否促销计划 1是, 0否-限时抢购
        /// </summary>
        public bool? IsPromotionSchedule { get; set; }

        /// <summary>
        /// 是否是秒杀字段，为DC就是秒杀，否则为Null
        /// </summary>
        public bool? IsSecondKill { get; set; }


        #region 产品数量相关
        /// <summary>
        /// 拿出来做活动的产品总数量
        /// </summary>
        public int? CountDownQty { get; set; }

        /// <summary>
        /// 是否限量发售
        /// </summary>
        public bool? IsLimitedQty { get; set; }

        /// <summary>
        /// 限量发售的时候，如果限制的数量卖完了，是否需要结束这个促销
        /// </summary>
        public bool? IsEndIfNoQty { get; set; }

        /// <summary>
        /// 是否预留库存
        /// </summary>
        public bool? IsReservedQty { get; set; }

        /// <summary>
        /// 每单限购数量
        /// </summary>
        public int? MaxPerOrder { get; set; }

        #endregion


        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

    }
}
