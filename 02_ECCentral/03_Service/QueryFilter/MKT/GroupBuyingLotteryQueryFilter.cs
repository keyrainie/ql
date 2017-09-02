using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.MKT
{
    public class GroupBuyingLotteryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }

        //public string ChannelID { get; set; }

        /// <summary>
        /// 团购系统编号
        /// </summary>
        public int? GroupBuyingSysNo { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public CustomerRank? RankType { get; set; }

        /// <summary>
        /// 团购开始日期范围从
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 团购开始日期范围到
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        public DateTime? EndDateFrom { get; set; }

        public DateTime? EndDateTo { get; set; }
    }
}
