using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SOIncomeRefundMaintainVM : ModelBase
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单号
        /// </summary>
        public int? RMANumber
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是RMA物流拒收
        /// </summary>
        public bool? ShipRejected
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundCashAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 转礼品卡金额
        /// </summary>
        public decimal? RefundGiftCard
        {
            get;
            set;
        }

        /// <summary>
        /// 转积分
        /// </summary>
        public int RefundPoint
        {
            get;
            set;
        }

        private RefundInfoVM m_RefundInfo;
        /// <summary>
        /// 退款信息
        /// </summary>
        public RefundInfoVM RefundInfo
        {
            get
            {
                return m_RefundInfo;
            }
            set
            {
                base.SetValue("RefundInfo", ref m_RefundInfo, value);
            }
        }

        private string m_RefundReasonSysNo;
        /// <summary>
        /// 退款原因系统编号
        /// </summary>
        [Validate(ValidateType.Regex)]
        public string RefundReasonSysNo
        {
            get
            {
                return m_RefundReasonSysNo;
            }
            set
            {
                base.SetValue("RefundReasonSysNo", ref m_RefundReasonSysNo, value);
            }
        }

        private string m_FinNote;
        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinNote
        {
            get
            {
                return m_FinNote;
            }
            set
            {
                base.SetValue("FinNote", ref m_FinNote, value);
            }
        }

        private string m_FinAppendNote;
        /// <summary>
        /// 追加财务备注
        /// </summary>
        public string FinAppendNote
        {
            get
            {
                return m_FinAppendNote;
            }
            set
            {
                base.SetValue("FinAppendNote", ref m_FinAppendNote, value);
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
                return m_OrderType;
            }
            set
            {
                base.SetValue("OrderType", ref m_OrderType, value);
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
                return m_AuditStatus;
            }
            set
            {
                base.SetValue("AuditStatus", ref m_AuditStatus, value);
            }
        }

        #region 扩展属性

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
        /// 审核状态列表
        /// </summary>
        public List<KeyValuePair<RefundStatus?, string>> AuditStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<RefundStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        #endregion 扩展属性

        #region 延迟加载的属性

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