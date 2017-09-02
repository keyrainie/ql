using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class LotteryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 中奖ID
        /// </summary>
        public string LotteryLabel { get; set; }

        /// <summary>
        /// 抽奖活动名称
        /// </summary>
        public string LotteryName { get; set; }

        /// <summary>
        /// 是否中奖
        /// </summary>
        public NYNStatus? IsLucky { get; set; }

        /// <summary>
        /// 团购开始日期范围从
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 团购开始日期范围到
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? EndDateTo { get; set; }
    }
}
