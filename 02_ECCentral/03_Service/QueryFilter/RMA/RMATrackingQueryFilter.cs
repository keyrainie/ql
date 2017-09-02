using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.RMA;

namespace ECCentral.QueryFilter.RMA
{
    public class RMATrackingQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 单件号
        /// </summary>
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 下部操作
        /// </summary>
        public RMANextHandler? NextHandler { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public InternalMemoStatus? Status { get; set; }
        /// <summary>
        /// 创建时间范围开始
        /// </summary>
        public DateTime? CreateTimeFrom { get; set; }
        /// <summary>
        /// 创建时间范围结束
        /// </summary>
        public DateTime? CreateTimeTo { get; set; }
        /// <summary>
        /// 关闭时间范围开始
        /// </summary>
        public DateTime? CloseTimeFrom { get; set; }
        /// <summary>
        /// 关闭时间范围结束
        /// </summary>
        public DateTime? CloseTimeTo { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 处理者
        /// </summary>
        public int? UpdateUserSysNo { get; set; }

        public string CompanyCode { get; set; }

    }
}
