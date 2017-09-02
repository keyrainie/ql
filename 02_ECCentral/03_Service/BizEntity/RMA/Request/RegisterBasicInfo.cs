using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Customer;

namespace ECCentral.BizEntity.RMA
{                                                                                                               /// <summary>
    /// 单件基本信息
    /// </summary>
    public class RegisterBasicInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        public CustomerInfo Customer { get; set; }

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
        /// 机身编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 客户描述
        /// </summary>
        public string CustomerDesc { get; set; }

        /// <summary>
        /// 是否7天内处理完成的
        /// </summary>
        public bool? IsWithin7Days { get; set; }

        /// <summary>
        /// 归属于
        /// </summary>
        public RMAOwnBy? OwnBy { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public RMALocation? Location { get; set; }

        /// <summary>
        /// 下一个处理者
        /// </summary>
        public RMANextHandler? NextHandler { get; set; }

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
        /// 商品所属仓库
        /// </summary>
        public string LocationWarehouse { get; set; }

        /// <summary>
        /// 归属仓库--属于NE后才会写入数据
        /// </summary>
        public string OwnByWarehouse { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 退货成本--和销售成本要一致
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// RMA原因
        /// </summary>
        public int? RMAReason { get; set; }

        /// <summary>
        /// RMA原因描述
        /// </summary>
        public string RMAReasonDesc { get; set; }

        /// <summary>
        /// 送修单写入的数据
        /// 送/无= 0
        /// 送/有= 1
        /// 送/返= 2
        /// 送/未返= 3
        /// </summary>
        public int? OutBoundWithInvoice { get; set; }

        /// <summary>
        /// SO单出货仓库
        /// </summary>
        public string ShippedWarehouse { get; set; }

        /// <summary>
        /// 更新返还信息时间
        /// </summary>
        public DateTime? UpdateNoResponseTime { get; set; }

        /// <summary>
        /// RMA产生的责任部门
        /// </summary>
        public int? FailureType { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public SOProductType? SOItemType { get; set; }

        #region 相关状态信息
        /// <summary>
        /// 送修状态
        /// </summary>
        public RMAOutBoundStatus? OutBoundStatus { get; set; }

        /// <summary>
        /// 入库状态
        /// </summary>
        public RMAReturnStatus? ReturnStatus { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RMARefundStatus? RefundStatus { get; set; }

        /// <summary>
        /// 单件状态
        /// </summary>
        public RMARequestStatus? Status { get; set; }

        /// <summary>
        /// 结案人系统编号
        /// </summary>
        public int? CloseUserSysNo { get; set; }

        /// <summary>
        /// 结案人名称
        /// </summary>
        public string CloseUserName { get; set; }

        /// <summary>
        /// 结案时间
        /// </summary>
        public DateTime? CloseTime { get; set; }

        #endregion

        public ERPReturnType ERPStatus { get; set; }

        public ProductInventoryType InventoryType { get; set; }
    }
}
