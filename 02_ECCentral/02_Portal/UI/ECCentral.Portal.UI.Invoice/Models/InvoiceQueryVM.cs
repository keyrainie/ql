using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 分公司收款单查询条件VM
    /// </summary>
    public class InvoiceQueryVM : ModelBase
    {
        public InvoiceQueryVM()
        {
            IsByCustomer = false;
            IsSalesOrder = true;
        }

        private String m_OrderID;
        /// <summary>
        /// 单据ID
        /// </summary>
        [Validate(ValidateType.Regex, @"^(R3)?[1-9][0-9]{0,9}(\.(R3)?[1-9][0-9]{0,9}){0,10}$")]
        public String OrderID
        {
            get
            {
                return this.m_OrderID;
            }
            set
            {
                this.SetValue("OrderID", ref m_OrderID, value);
            }
        }

        private String m_CustomerSysNo;
        /// <summary>
        /// 客户系统编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9][0-9]{0,9}(\.[1-9][0-9]{0,9}){0,10}$")]
        public String CustomerSysNo
        {
            get
            {
                return this.m_CustomerSysNo;
            }
            set
            {
                this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);
            }
        }

        private SOIncomeOrderType? m_OrderType;
        /// <summary>
        /// 收款单单据类型
        /// </summary>
        public SOIncomeOrderType? OrderType
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

        private SOIncomeOrderStyle? m_IncomeType;
        /// <summary>
        /// 收款类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeType
        {
            get
            {
                return this.m_IncomeType;
            }
            set
            {
                this.SetValue("IncomeType", ref m_IncomeType, value);
            }
        }

        private SOIncomeStatus? m_IncomeStatus;
        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get
            {
                return this.m_IncomeStatus;
            }
            set
            {
                this.SetValue("IncomeStatus", ref m_IncomeStatus, value);
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

        private String m_ShipTypeSysNo;
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public String ShipTypeSysNo
        {
            get
            {
                return this.m_ShipTypeSysNo;
            }
            set
            {
                this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);
            }
        }

        private String m_IncomeConfirmer;
        /// <summary>
        /// 收款单审核人
        /// </summary>
        public String IncomeConfirmer
        {
            get
            {
                return this.m_IncomeConfirmer;
            }
            set
            {
                this.SetValue("IncomeConfirmer", ref m_IncomeConfirmer, value);
            }
        }

        private String m_OrderSysNo;
        /// <summary>
        /// 单据系统编号
        /// </summary>
        public String OrderSysNo
        {
            get
            {
                return this.m_OrderSysNo;
            }
            set
            {
                this.SetValue("OrderSysNo", ref m_OrderSysNo, value);
            }
        }

        private Boolean m_IsCash;
        /// <summary>
        /// 涉及现金的RO
        /// </summary>
        public Boolean IsCash
        {
            get
            {
                return this.m_IsCash;
            }
            set
            {
                this.SetValue("IsCash", ref m_IsCash, value);
            }
        }

        private Boolean m_IsException;
        /// <summary>
        /// 统计异常数据
        /// </summary>
        public Boolean IsException
        {
            get
            {
                return this.m_IsException;
            }
            set
            {
                this.SetValue("IsException", ref m_IsException, value);
            }
        }

        private String m_StockID;
        /// <summary>
        /// 仓库编号，多个仓库编号之间用逗号隔开
        /// </summary>
        public String StockID
        {
            get
            {
                return this.m_StockID;
            }
            set
            {
                this.SetValue("StockID", ref m_StockID, value);
            }
        }

        private Boolean? m_IsByCustomer;
        /// <summary>
        /// 根据单据的CustomerSysNo查询所有
        /// </summary>
        public Boolean? IsByCustomer
        {
            get
            {
                return this.m_IsByCustomer;
            }
            set
            {
                this.SetValue("IsByCustomer", ref m_IsByCustomer, value);
            }
        }

        private Boolean m_IsRelated;
        /// <summary>
        /// 是否关联子母单
        /// </summary>
        public Boolean IsRelated
        {
            get
            {
                return this.m_IsRelated;
            }
            set
            {
                this.SetValue("IsRelated", ref m_IsRelated, value);
            }
        }

        private Boolean m_IsSalesOrder;
        /// <summary>
        /// 是否排除测试和作废的
        /// </summary>
        public Boolean IsSalesOrder
        {
            get
            {
                return this.m_IsSalesOrder;
            }
            set
            {
                this.SetValue("IsSalesOrder", ref m_IsSalesOrder, value);
            }
        }

        private Boolean m_IsGiftCard;
        /// <summary>
        /// 是否排除礼品卡
        /// </summary>
        public Boolean IsGiftCard
        {
            get
            {
                return this.m_IsGiftCard;
            }
            set
            {
                this.SetValue("IsGiftCard", ref m_IsGiftCard, value);
            }
        }

        private DateTime? m_PayedDateFrom;
        /// <summary>
        /// 网关收款时间起
        /// </summary>
        public DateTime? PayedDateFrom
        {
            get
            {
                return this.m_PayedDateFrom;
            }
            set
            {
                this.SetValue("PayedDateFrom", ref m_PayedDateFrom, value);
            }
        }

        private DateTime? m_PayedDateTo;
        /// <summary>
        /// 网关收款时间止
        /// </summary>
        public DateTime? PayedDateTo
        {
            get
            {
                return this.m_PayedDateTo;
            }
            set
            {
                this.SetValue("PayedDateTo", ref m_PayedDateTo, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        /// <summary>
        /// 创建时间起
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get
            {
                return this.m_CreateDateFrom;
            }
            set
            {
                this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private DateTime? m_CreateDateTo;
        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? CreateDateTo
        {
            get
            {
                return this.m_CreateDateTo;
            }
            set
            {
                this.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private DateTime? m_ConfirmDateFrom;
        /// <summary>
        /// 确认时间起
        /// </summary>
        public DateTime? ConfirmDateFrom
        {
            get
            {
                return this.m_ConfirmDateFrom;
            }
            set
            {
                this.SetValue("ConfirmDateFrom", ref m_ConfirmDateFrom, value);
            }
        }

        private DateTime? m_ConfirmDateTo;
        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? ConfirmDateTo
        {
            get
            {
                return this.m_ConfirmDateTo;
            }
            set
            {
                this.SetValue("ConfirmDateTo", ref m_ConfirmDateTo, value);
            }
        }

        private DateTime? m_SOOutDateFrom;
        /// <summary>
        /// 出库时间起
        /// </summary>
        public DateTime? SOOutDateFrom
        {
            get
            {
                return this.m_SOOutDateFrom;
            }
            set
            {
                this.SetValue("SOOutDateFrom", ref m_SOOutDateFrom, value);
            }
        }

        private DateTime? m_SOOutDateTo;
        /// <summary>
        /// 出库时间止
        /// </summary>
        public DateTime? SOOutDateTo
        {
            get
            {
                return this.m_SOOutDateTo;
            }
            set
            {
                this.SetValue("SOOutDateTo", ref m_SOOutDateTo, value);
            }
        }

        private DateTime? m_RORefundDateFrom;
        /// <summary>
        /// RO退款时间起
        /// </summary>
        public DateTime? RORefundDateFrom
        {
            get
            {
                return this.m_RORefundDateFrom;
            }
            set
            {
                this.SetValue("RORefundDateFrom", ref m_RORefundDateFrom, value);
            }
        }

        private DateTime? m_RORefundDateTo;
        /// <summary>
        /// RO退款时间止
        /// </summary>
        public DateTime? RORefundDateTo
        {
            get
            {
                return this.m_RORefundDateTo;
            }
            set
            {
                this.SetValue("RORefundDateTo", ref m_RORefundDateTo, value);
            }
        }

        #region 扩展字段

        public String CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        /// <summary>
        /// 单据类型列表
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderType?, string>> OrderTypeList
        {
            get
            {
                var orderTypeList = EnumConverter.GetKeyValuePairs<SOIncomeOrderType>(EnumConverter.EnumAppendItemType.All);
                orderTypeList.RemoveAll(x => x.Key == SOIncomeOrderType.OverPayment || x.Key == SOIncomeOrderType.AO
                    || x.Key == SOIncomeOrderType.Group|| x.Key == SOIncomeOrderType.GroupRefund);
                return orderTypeList;
            }
        }

        /// <summary>
        /// 收款单状态列表
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> IncomeStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 收款类型列表
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderStyle?, string>> IncomeTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeOrderStyle>(EnumConverter.EnumAppendItemType.All);
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

        #endregion 扩展字段

        #region 延迟加载的字段

        private List<UserInfo> m_ConfirmerList;
        /// <summary>
        /// 审核人列表
        /// </summary>
        public List<UserInfo> ConfirmerList
        {
            get
            {
                return m_ConfirmerList;
            }
            set
            {
                base.SetValue("ConfirmerList", ref m_ConfirmerList, value);
            }
        }

        #endregion 延迟加载的字段
    }
}