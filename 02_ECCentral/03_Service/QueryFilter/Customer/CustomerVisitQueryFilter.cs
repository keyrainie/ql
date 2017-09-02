using System;
using System.Collections.Generic;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerVisitQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? SeachType
        {
            get;
            set;
        }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 顾客等级
        /// </summary>
        public int? CustomerRank
        {
            get;
            set;
        }
        /// <summary>
        /// 较小累积消费
        /// </summary>
        public decimal? FromTotalAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 较大累积消费
        /// </summary>
        public decimal? ToTotalAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 较小最后下单时间
        /// </summary>
        public DateTime? FromLastSODate
        {
            get;
            set;
        }

        /// <summary>
        /// 较大最后下单时间
        /// </summary>
        public DateTime? ToLastSODate
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是Vip
        /// </summary>
        public bool? IsVip
        {
            get;
            set;
        }
        /// <summary>
        /// 回访处理状态
        /// </summary>
        public int? DealStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 回访电话结果
        /// </summary>
        public List<int> CallResult
        {
            get;
            set;
        }

        /// <summary>
        /// 购买意愿
        /// </summary>
        public int? ConsumeDesire
        {
            get;
            set;
        }
        /// <summary>
        /// 是否被激活
        /// </summary>
        public int? IsActivated
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有维护
        /// </summary>
        public bool? IsMaintain
        {
            get;
            set;
        }

        /// <summary>
        /// 维护者编号
        /// </summary>
        public int? LastEditorSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 较早回访时间
        /// </summary>
        public DateTime? FromVisitDate
        {
            get;
            set;
        }
        /// <summary>
        /// 较近回访时间
        /// </summary>
        public DateTime? ToVisitDate
        {
            get;
            set;
        }
        /// <summary>
        /// 较早下单时间
        /// </summary>
        public DateTime? FromOrderDate
        {
            get;
            set;
        }
        /// <summary>
        /// 较近下单时间
        /// </summary>
        public DateTime? ToOrderDate
        {
            get;
            set;
        }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? ShipType { get; set; }
        public int ShipTypeCondition { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public DateTime? SpiderOrderDateFrom { get; set; }
        public DateTime? SpiderOrderDateTo { get; set; }
        public string CustomerSysNos { get; set; }
        public bool IsSpiderSearch { get; set; }
    }
}
