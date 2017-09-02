using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.RMA;

namespace ECCentral.QueryFilter.RMA
{
    /// <summary>
    /// 送修未返还查询条件
    /// </summary>
    public class OutBoundNotReturnQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 送修时间范围开始
        /// </summary>
        public DateTime? OutTimeFrom { get; set; }

        /// <summary>
        /// 送修时间范围结束
        /// </summary>
        public DateTime? OutTimeTo { get; set; }
        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int? VendorSysNo { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 产品管理员系统编号
        /// </summary>
        public int? PMUserSysNo { get; set; }
        /// <summary>
        /// 产品三级类别
        /// </summary>
        public int? C3SysNo { get; set; }
        /// <summary>
        /// 产品二级类别
        /// </summary>
        public int? C2SysNo { get; set; }
        /// <summary>
        /// 产品一级类别
        /// </summary>
        public int? C1SysNo { get; set; }
        /// <summary>
        /// 送修超过天数
        /// </summary>
        public int? SendDays { get; set; }
        /// <summary>
        /// 催讨信息
        /// </summary>
        public bool? HasResponse { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public RMARefundStatus? RefundStatus { get; set; }
        /// <summary>
        /// 发还状态
        /// </summary>
        public RMARevertStatus? RevertStatus { get; set; }

        public string CompanyCode { get; set; }


        public string ChannelID { get; set; }

    }

    /// <summary>
    /// RMA库存查询条件
    /// </summary>
    public class RMAInventoryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? RMASysNo { get; set; }

        public RMAOwnBy? RMAOwnBy { get; set; }

        public int? ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string LocationWarehouse { get; set; }

        public string OwnByWarehouse { get; set; }

        public RMAInventorySearchType? SearchType { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }
    }
}
