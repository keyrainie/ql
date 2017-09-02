using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.RMA
{
    public class RMARegisterInfo : EntityBase
    {
        /// <summary>
        /// 单件编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public RMARequestType? RequestType { get; set; }

        /// <summary>
        /// 单件状态
        /// </summary>
        public RMARequestStatus? Status { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RMARefundStatus? RefundStatus { get; set; }

        /// <summary>
        /// 单件发还状态
        /// </summary>
        public RMARevertStatus? RevertStatus { get; set; }

        /// <summary>
        /// 商品类型    
        /// </summary>
        public SOItemType? SoItemType { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// 归属于
        /// </summary>
        public RMAOwnBy? OwnBy { get; set; }

        public RMALocation? Location { get; set; }

        /// <summary>
        /// 接收仓库编号
        /// </summary>
        public string LocationWarehouse { get; set; }

        public bool? IsWithin7Days { get; set; }
    }
}
