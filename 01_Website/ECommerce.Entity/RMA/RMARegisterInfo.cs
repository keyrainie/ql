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
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 申请编号
        /// </summary>
        public int? RequestSysNo { get; set; }
        
        /// <summary>
        /// 申请类型
        /// </summary>
        public int RequestType { get; set; }

        public string RequestTypeString { get; set; }

        /// <summary>
        /// 客户描述
        /// </summary>
        public string CustomerDesc { get; set; }

        /// <summary>
        /// RMA原因
        /// </summary>
        public int RMAReason { get; set; }

        public string RMAReasonString { get; set; }

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
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// RMA原因描述
        /// </summary>
        public string RMAReasonDesc { get; set; }

        /// <summary>
        /// 单件状态
        /// </summary>
        public RMARequestStatus? Status { get; set; }

        public string StatusString { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RMARefundStatus? RefundStatus { get; set; }

        /// <summary>
        /// 返修状态
        /// </summary>
        public RMARevertStatus? RevertStatus { get; set; }

        /// <summary>
        /// 归属于
        /// </summary>
        public RMAOwnBy? OwnBy { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public RMALocation? Location { get; set; }

        /// <summary>
        /// 是否7天内处理完成的
        /// </summary>
        public bool? IsWithin7Days { get; set; }

        /// <summary>
        /// 是否建议退款
        /// </summary>
        public bool? IsRecommendRefund { get; set; }

        /// <summary>
        /// 新品类型
        /// </summary>
        public RMANewProductStatus? NewProductStatus { get; set; }

        /// <summary>
        /// 下一个处理者
        /// </summary>
        public RMANextHandler? NextHandler { get; set; }

        /// <summary>
        /// 单件数量 
        /// </summary>
        public int? Quantity { get { return 1; } }

        /// <summary>
        /// 是否有发票
        /// </summary>
        public bool? IsHaveInvoice { get; set; }

        /// <summary>
        /// 附件是否完整
        /// </summary>
        public bool? IsFullAccessory { get; set; }

        /// <summary>
        /// 包裹是否完整
        /// </summary>
        public bool? IsFullPackage { get; set; }

        /// <summary>
        /// 预约返还时间
        /// </summary>
        public DateTime? OutTime { get; set; }

        /// <summary>
        /// 发货包裹号
        /// </summary>
        public string RevertPackageID { get; set; }
        /// <summary>
        /// 发货人姓名
        /// </summary>
        public string RevertUserName { get; set; }

        /// <summary>
        /// 送修返还时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }
        /// <summary>
        /// 送修返还结果类型
        /// </summary>
        public string VendorRepairResultType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime? CheckTime { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public SOProductType? SOItemType { get; set; }

        /// <summary>
        /// 销售仓库编号
        /// </summary>
        public int ShippedWarehouse { get; set; }
    }
}
