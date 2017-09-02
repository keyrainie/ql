using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 退款审核查询条件ViewModel
    /// </summary>
    public class AuditRefundQueryVM : ModelBase
    {
        public AuditRefundQueryVM()
        {
            OperationType = OperationSignType.LessThanOrEqual;
            CreateTimeFrom = DateTime.Now.Date;
            CreateTimeTo = CreateTimeFrom.Value.AddDays(1);
        }

        private String m_Id;
        /// <summary>
        /// 单据编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public String Id
        {
            get
            {
                return this.m_Id;
            }
            set
            {
                this.SetValue("Id", ref m_Id, value);
            }
        }

        private RefundStatus? m_AuditStatus;
        /// <summary>
        /// 审核状态
        /// </summary>
        public RefundStatus? AuditStatus
        {
            get
            {
                return this.m_AuditStatus;
            }
            set
            {
                this.SetValue("AuditStatus", ref m_AuditStatus, value);
            }
        }

        private DateTime? m_CreateTimeFrom;
        /// <summary>
        /// 创建时间（从）
        /// </summary>
        public DateTime? CreateTimeFrom
        {
            get
            {
                return this.m_CreateTimeFrom;
            }
            set
            {
                this.SetValue("CreateTimeFrom", ref m_CreateTimeFrom, value);
            }
        }

        private DateTime? m_CreateTimeTo;
        /// <summary>
        /// 创建时间（到）
        /// </summary>
        public DateTime? CreateTimeTo
        {
            get
            {
                return this.m_CreateTimeTo;
            }
            set
            {
                this.SetValue("CreateTimeTo", ref m_CreateTimeTo, value);
            }
        }

        private DateTime? m_AuditTimeFrom;
        /// <summary>
        /// 审核时间（从）
        /// </summary>
        public DateTime? AuditTimeFrom
        {
            get
            {
                return this.m_AuditTimeFrom;
            }
            set
            {
                this.SetValue("AuditTimeFrom", ref m_AuditTimeFrom, value);
            }
        }

        private DateTime? m_AuditTimeTo;
        /// <summary>
        /// 审核时间（到）
        /// </summary>
        public DateTime? AuditTimeTo
        {
            get
            {
                return this.m_AuditTimeTo;
            }
            set
            {
                this.SetValue("AuditTimeTo", ref m_AuditTimeTo, value);
            }
        }

        private String m_RMANumber;
        /// <summary>
        /// RMA退款单号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public String RMANumber
        {
            get
            {
                return this.m_RMANumber;
            }
            set
            {
                this.SetValue("RMANumber", ref m_RMANumber, value);
            }
        }

        private RMARefundStatus? m_RMAStatus;
        /// <summary>
        /// RMA退款单状态
        /// </summary>
        public RMARefundStatus? RMAStatus
        {
            get
            {
                return this.m_RMAStatus;
            }
            set
            {
                this.SetValue("RMAStatus", ref m_RMAStatus, value);
            }
        }

        private String m_OrderNumber;
        /// <summary>
        /// 订单号，多个订单号之间用逗号隔开
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9][0-9]{0,9}(\.[1-9][0-9]{0,9}){0,10}$")]
        public String OrderNumber
        {
            get
            {
                return this.m_OrderNumber;
            }
            set
            {
                this.SetValue("OrderNumber", ref m_OrderNumber, value);
            }
        }

        private RefundOrderType? m_OrderType;
        /// <summary>
        /// 单据类型
        /// </summary>
        public RefundOrderType? OrderType
        {
            get
            {
                return this.m_OrderType;
            }
            set
            {
                this.SetValue("OrderType", ref m_OrderType, value);
            }
        }

        private String m_RMAReasonCode;
        /// <summary>
        /// 退款原因SysNo
        /// </summary>
        public String RMAReasonCode
        {
            get
            {
                return this.m_RMAReasonCode;
            }
            set
            {
                this.SetValue("RMAReasonCode", ref m_RMAReasonCode, value);
            }
        }

        private RefundPayType? m_RefundPayType;
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get
            {
                return this.m_RefundPayType;
            }
            set
            {
                this.SetValue("RefundPayType", ref m_RefundPayType, value);
            }
        }

        private Boolean? m_ShipRejected;
        /// <summary>
        /// 是否物流拒收
        /// </summary>
        public Boolean? ShipRejected
        {
            get
            {
                return this.m_ShipRejected;
            }
            set
            {
                this.SetValue("ShipRejected", ref m_ShipRejected, value);
            }
        }

        private Boolean m_CashRelated;
        /// <summary>
        /// 是否涉及现金
        /// </summary>
        public Boolean CashRelated
        {
            get
            {
                return this.m_CashRelated;
            }
            set
            {
                this.SetValue("CashRelated", ref m_CashRelated, value);
            }
        }

        private SOIncomeStatus? m_RefundStatus;
        /// <summary>
        /// 退款状态（收款状态吧？）
        /// </summary>
        public SOIncomeStatus? RefundStatus
        {
            get
            {
                return this.m_RefundStatus;
            }
            set
            {
                this.SetValue("RefundStatus", ref m_RefundStatus, value);
            }
        }

        private String m_CustomerID;
        /// <summary>
        /// 客户ID
        /// </summary>
        [Validate(ValidateType.MaxLength, 100)]
        public String CustomerID
        {
            get
            {
                return this.m_CustomerID;
            }
            set
            {
                this.SetValue("CustomerID", ref m_CustomerID, value);
            }
        }

        private OperationSignType m_OperationType;
        /// <summary>
        /// 退款金额操作符号
        /// </summary>
        public OperationSignType OperationType
        {
            get
            {
                return this.m_OperationType;
            }
            set
            {
                this.SetValue("OperationType", ref m_OperationType, value);
            }
        }

        private string m_RefundAmount;
        /// <summary>
        /// 退款金额
        /// </summary>
        [Validate(ValidateType.Regex, @"^\d{1,9}(\.\d{1,2})?$")]
        public string RefundAmount
        {
            get
            {
                return this.m_RefundAmount;
            }
            set
            {
                this.SetValue("RefundAmount", ref m_RefundAmount, value);
            }
        }

        private String m_PayTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public String PayTypeSysNo
        {
            get
            {
                return this.m_PayTypeSysNo;
            }
            set
            {
                this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private WLTRefundStatus? m_WLTRefundStatus;

        /// <summary>
        /// 退款结果
        /// </summary>
        public WLTRefundStatus? WLTRefundStatus
        {
            get { return m_WLTRefundStatus; }
            set
            {
                this.SetValue("WLTRefundStatus", ref m_WLTRefundStatus, value);
            }
        }

        private Boolean? m_IsVAT;
        /// <summary>
        /// 是否需要开增票
        /// </summary>
        public Boolean? IsVAT
        {
            get
            {
                return this.m_IsVAT;
            }
            set
            {
                this.SetValue("IsVAT", ref m_IsVAT, value);
            }
        }

        private Int32? m_StockSysNo;
        /// <summary>
        /// 分仓系统编号
        /// </summary>
        public Int32? StockSysNo
        {
            get
            {
                return this.m_StockSysNo;
            }
            set
            {
                this.SetValue("StockSysNo", ref m_StockSysNo, value);
            }
        }

        public String CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        #region 扩展属性

        /// <summary>
        /// 审核状态列表
        /// </summary>
        public List<KeyValuePair<RefundStatus?, string>> AuditStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<RefundStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 单据类型列表,除去SO
        /// </summary>
        public List<KeyValuePair<RefundOrderType?, string>> OrderTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<RefundOrderType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 退款类型列表
        /// </summary>
        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList
        {
            get
            {
                var refundPayTypeList = EnumConverter.GetKeyValuePairs<RefundPayType>(EnumConverter.EnumAppendItemType.All);
                refundPayTypeList.RemoveAll(x => x.Key == ECCentral.BizEntity.Invoice.RefundPayType.TransferPointRefund);
                return refundPayTypeList;
            }
        }

        /// <summary>
        /// RMA退款单状态列表
        /// </summary>
        public List<KeyValuePair<RMARefundStatus?, string>> RMARefundStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<RMARefundStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 退款状态列表
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> RefundStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 退款结果列表
        /// </summary>
        public List<KeyValuePair<WLTRefundStatus?, string>> WLTRefundStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<WLTRefundStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 是、否状态列表
        /// </summary>
        public List<KeyValuePair<bool?, string>> YNList
        {
            get
            {
                return BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 操作符号列表
        /// </summary>
        public List<KeyValuePair<OperationSignType?, string>> OperationSignTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<OperationSignType>(EnumConverter.EnumAppendItemType.None);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<WebChannelVM> WebChannelList
        {
            get
            {
                var webchannleList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
                webchannleList.Insert(0, new WebChannelVM()
                {
                    ChannelName = ResCommonEnum.Enum_All
                });
                return webchannleList;
            }
        }

        #endregion 扩展属性

        #region 延迟加载的属性

        private List<StockInfo> m_StockList;
        /// <summary>
        /// 分仓列表
        /// </summary>
        public List<StockInfo> StockList
        {
            get
            {
                return m_StockList;
            }
            set
            {
                base.SetValue("StockList", ref m_StockList, value);
            }
        }

        private List<CodeNamePair> m_RefundReasonList;
        /// <summary>
        /// 退款原因列表
        /// </summary>
        public List<CodeNamePair> RefundReasonList
        {
            get
            {
                return m_RefundReasonList;
            }
            set
            {
                base.SetValue("RefundReasonList", ref m_RefundReasonList, value);
            }
        }

        #endregion 延迟加载的属性
    }
}