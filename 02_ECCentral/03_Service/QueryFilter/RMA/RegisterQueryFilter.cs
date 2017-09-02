using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.RMA;
using System.Runtime.Serialization;
using ECCentral.BizEntity;


namespace ECCentral.QueryFilter.RMA
{
    /// <summary>
    /// 单件查询条件
    /// </summary>
    [Serializable]
    [DataContract]
    public class RegisterQueryFilter
    {
        [DataMember]
        public PagingInfo PagingInfo { get; set; }

        #region 基本查询条件
        /// <summary>
        /// 单件编号
        /// </summary>
        [DataMember]
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 申请单编号
        /// </summary>
        [DataMember]
        public string RequestID { get; set; }

        /// <summary>
        /// 销售单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        [DataMember]
        public RMARequestType? RequestType { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 客户账号
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 销售方
        /// </summary>
        [DataMember]
        public SellersType? SellersType { get; set; }
        /// <summary>
        /// 是否已打印标签
        /// </summary>
        [DataMember]
        public bool? IsLabelPrinted { get; set; }
        /// <summary>
        /// 商家处理结果
        /// </summary>
        [DataMember]
        public string SellerOperationInfo { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string ChannelID { get; set; }

        #region 状态
        /// <summary>
        /// 未接收
        /// </summary>
        [DataMember]
        public bool? IsUnReceive { get; set; }
        /// <summary>
        /// 未检测
        /// </summary>
        [DataMember]
        public bool? IsUnCheck { get; set; }
        /// <summary>
        /// 未送修
        /// </summary>
        [DataMember]
        public bool? IsUnOutbound { get; set; }
        /// <summary>
        /// 未返还
        /// </summary>
        [DataMember]
        public bool? IsUnResponse { get; set; }
        /// <summary>
        /// 未退款
        /// </summary>
        [DataMember]
        public bool? IsUnRefund { get; set; }
        /// <summary>
        /// 未退货
        /// </summary>
        [DataMember]
        public bool? IsUnReturn { get; set; }
        /// <summary>
        /// 未发货
        /// </summary>
        [DataMember]
        public bool? IsUnRevert { get; set; }

        #endregion
        #endregion
        #region 高级查询条件
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? CreateTimeFrom { get; set; }
        [DataMember]
        public DateTime? CreateTimeTo { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        [DataMember]
        public DateTime? RecvTimeFrom { get; set; }
        [DataMember]
        public DateTime? RecvTimeTo { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        [DataMember]
        public DateTime? CheckTimeFrom { get; set; }
        [DataMember]
        public DateTime? CheckTimeTo { get; set; }
        /// <summary>
        /// 送修时间
        /// </summary>
        [DataMember]
        public DateTime? OutboundTimeFrom { get; set; }
        [DataMember]
        public DateTime? OutboundTimeTo { get; set; }
        /// <summary>
        /// 返还时间
        /// </summary>
        [DataMember]
        public DateTime? ResponseTimeFrom { get; set; }
        [DataMember]
        public DateTime? ResponseTimeTo { get; set; }
        /// <summary>
        /// 退款时间
        /// </summary>
        [DataMember]
        public DateTime? RefundTimeFrom { get; set; }
        [DataMember]
        public DateTime? RefundTimeTo { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        [DataMember]
        public DateTime? ReturnTimeFrom { get; set; }
        [DataMember]
        public DateTime? ReturnTimeTo { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMember]
        public DateTime? RevertTimeFrom { get; set; }
        [DataMember]
        public DateTime? RevertTimeTo { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        [DataMember]
        public RMARevertStatus? RevertStatus { get; set; }
        /// <summary>
        /// 新品状态
        /// </summary>
        [DataMember]
        public RMANewProductStatus? NewProductStatus { get; set; }

        /// <summary>
        /// 送修状态
        /// </summary>
        [DataMember]
        public RMAOutBoundStatus? OutBoundStatus { get; set; }
        /// <summary>
        /// 入库状态
        /// </summary>
        [DataMember]
        public RMAReturnStatus? ReturnStatus { get; set; }
        /// <summary>
        /// 单件状态
        /// </summary>
        [DataMember]
        public RMARequestStatus? RequestStatus { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        [DataMember]
        public RMARefundStatus? RefundStatus { get; set; }
        /// <summary>
        /// 产品管理员
        /// </summary>
        [DataMember]
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 下一步操作
        /// </summary>
        [DataMember]
        public RMANextHandler? NextHandler { get; set; }
        /// <summary>
        /// 是否VIP
        /// </summary>
        [DataMember]
        public bool? IsVIP { get; set; }
        /// <summary>
        /// 是否7天内处理完成
        /// </summary>
        [DataMember]
        public bool? IsWithin7Days { get; set; }
        /// <summary>
        /// 申请RMA原因
        /// </summary>
        [DataMember]
        public int? RMAReason { get; set; }

        /// <summary>
        /// 是否建议退款
        /// </summary>
        [DataMember]
        public bool? IsRecommendRefund { get; set; }

        /// <summary>
        /// 是否为复核单件
        /// </summary>
        [DataMember]
        public bool? IsRepeatRegister { get; set; }

        /// <summary>
        /// 产品三级类别编号
        /// </summary>
        [DataMember]
        public int? Category3SysNo { get; set; }

        [DataMember]
        public int? Category2SysNo { get; set; }

        [DataMember]
        public int? Category1SysNo { get; set; }

        #endregion

        /// <summary>
        /// 比较符号
        /// </summary>
        [DataMember]
        public CompareSymbolType? CompareSymbol
        {
            get;
            set;
        }
        /// <summary>
        /// 复核单件数量
        /// </summary>
        [DataMember]
        public int? ProductCount
        {
            get;
            set;
        }
        [DataMember]
        public string NextHandlerList
        {
            get;
            set;
        }
    }
}
