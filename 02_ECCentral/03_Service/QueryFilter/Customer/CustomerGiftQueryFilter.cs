using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客奖品信息查询条件
    /// </summary>
    public class CustomerGiftQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 奖品信息状态
        /// </summary>
        public CustomerGiftStatus? Status { get; set; }

        /// <summary>
        /// 奖品数量
        /// </summary>
        public int? ProductQty { get; set; }

        /// <summary>
        /// 发布时间起始范围
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 发布时间终止范围
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }
    }
}
