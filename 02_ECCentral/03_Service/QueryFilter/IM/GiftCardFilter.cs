using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class GiftCardFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ECCentral.BizEntity.IM.GiftCardStatus? Status { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        ///到期时间开始于
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// 到期时间结束于
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardCode { get; set; }

        public int? ChannelID { get; set; }

        /// <summary>
        /// 礼品卡类型
        /// </summary>
        public ECCentral.BizEntity.IM.GiftCardType? CardType { get; set; }

        //public int? Type { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 抵扣订单号
        /// </summary>
        public int? ActionSysNo { get; set; }

        //public int? ReferenceSOSysNo { get; set; }
        public int? CustomerSysNo { get; set; }

        public string CustomerID { get; set; }
    }
}
