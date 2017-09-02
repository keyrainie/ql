using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PayItemQueryVM : ModelBase
    {
        public PayItemQueryVM()
        {
            IsFilterAbandonItem = true;
        }

        private String m_WebChannelID;
        public String WebChannelID
        {
            get
            {
                return this.m_WebChannelID;
            }
            set
            {
                this.SetValue("WebChannelID", ref m_WebChannelID, value);
            }
        }

        private String m_OrderID;
        /// <summary>
        /// 单据编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^\s*[1-9][0-9]{0,8}(\s*\.\s*[1-9][0-9]{0,8}\s*)*$", ErrorMessageResourceName = "Msg_ValidateNoList", ErrorMessageResourceType = typeof(ResPayItemQuery))]
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

        private PayableOrderType? m_OrderType;
        /// <summary>
        /// 单据类型
        /// </summary>
        public PayableOrderType? OrderType
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

        private String m_Note;
        /// <summary>
        /// 备注
        /// </summary>
        [Validate(ValidateType.MaxLength, 200)]
        public String Note
        {
            get
            {
                return this.m_Note;
            }
            set
            {
                this.SetValue("Note", ref m_Note, value);
            }
        }

        private DateTime? m_InvoiceEditDateFrom;
        /// <summary>
        /// 发票状态修改时间: 从
        /// </summary>
        public DateTime? InvoiceEditDateFrom
        {
            get
            {
                return this.m_InvoiceEditDateFrom;
            }
            set
            {
                this.SetValue("InvoiceEditDateFrom", ref m_InvoiceEditDateFrom, value);
            }
        }

        private DateTime? m_InvoiceEditDateTo;
        /// <summary>
        /// 发票状态修改时间: 到
        /// </summary>
        public DateTime? InvoiceEditDateTo
        {
            get
            {
                return this.m_InvoiceEditDateTo;
            }
            set
            {
                this.SetValue("InvoiceEditDateTo", ref m_InvoiceEditDateTo, value);
            }
        }

        private Boolean m_IsFilterAbandonItem;
        /// <summary>
        /// 是否过滤Abandon状态的付款单
        /// </summary>
        public Boolean IsFilterAbandonItem
        {
            get
            {
                return this.m_IsFilterAbandonItem;
            }
            set
            {
                this.SetValue("IsFilterAbandonItem", ref m_IsFilterAbandonItem, value);
            }
        }

        private PayableInvoiceStatus? m_InvoiceStatus;
        /// <summary>
        /// 发票状态
        /// </summary>
        public PayableInvoiceStatus? InvoiceStatus
        {
            get
            {
                return this.m_InvoiceStatus;
            }
            set
            {
                this.SetValue("InvoiceStatus", ref m_InvoiceStatus, value);
            }
        }

        private string m_VendorSysNo;
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VendorSysNo
        {
            get
            {
                return this.m_VendorSysNo;
            }
            set
            {
                this.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }

        private Int32? m_UserID;
        /// <summary>
        /// PMID
        /// </summary>
        public Int32? UserID
        {
            get
            {
                return this.m_UserID;
            }
            set
            {
                this.SetValue("UserID", ref m_UserID, value);
            }
        }

        private PayItemStatus? m_Status;
        /// <summary>
        /// 付款状态
        /// </summary>
        public PayItemStatus? Status
        {
            get
            {
                return this.m_Status;
            }
            set
            {
                this.SetValue("Status", ref m_Status, value);
            }
        }

        private PayItemStyle? m_PayStyle;
        /// <summary>
        /// 付款类型
        /// </summary>
        public PayItemStyle? PayStyle
        {
            get
            {
                return this.m_PayStyle;
            }
            set
            {
                this.SetValue("PayStyle", ref m_PayStyle, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        /// <summary>
        /// 创建时间: 从
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
        /// 创建时间: 到
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

        private Boolean m_IsFilterPOETP;
        /// <summary>
        /// 是否到期的未付款PO明细
        /// </summary>
        public Boolean IsFilterPOETP
        {
            get
            {
                return this.m_IsFilterPOETP;
            }
            set
            {
                this.SetValue("IsFilterPOETP", ref m_IsFilterPOETP, value);
            }
        }

        private DateTime? m_ETPFrom;
        /// <summary>
        /// 到期付款日： 从
        /// </summary>
        public DateTime? ETPFrom
        {
            get
            {
                return this.m_ETPFrom;
            }
            set
            {
                this.SetValue("ETPFrom", ref m_ETPFrom, value);
            }
        }

        private DateTime? m_ETPTo;
        /// <summary>
        /// 到期付款日： 到
        /// </summary>
        public DateTime? ETPTo
        {
            get
            {
                return this.m_ETPTo;
            }
            set
            {
                this.SetValue("ETPTo", ref m_ETPTo, value);
            }
        }

        private DateTime? m_EstimatePayDateFrom;
        /// <summary>
        /// 估计付款时间: 从
        /// </summary>
        public DateTime? EstimatePayDateFrom
        {
            get
            {
                return this.m_EstimatePayDateFrom;
            }
            set
            {
                this.SetValue("EstimatePayDateFrom", ref m_EstimatePayDateFrom, value);
            }
        }

        private DateTime? m_EstimatePayDateTo;
        /// <summary>
        /// 估计付款时间: 到
        /// </summary>
        public DateTime? EstimatePayDateTo
        {
            get
            {
                return this.m_EstimatePayDateTo;
            }
            set
            {
                this.SetValue("EstimatePayDateTo", ref m_EstimatePayDateTo, value);
            }
        }

        private DateTime? m_PayDateFrom;
        /// <summary>
        /// 付款时间: 从
        /// </summary>
        public DateTime? PayDateFrom
        {
            get
            {
                return this.m_PayDateFrom;
            }
            set
            {
                this.SetValue("PayDateFrom", ref m_PayDateFrom, value);
            }
        }

        private DateTime? m_PayDateTo;
        /// <summary>
        /// 付款时间: 到
        /// </summary>
        public DateTime? PayDateTo
        {
            get
            {
                return this.m_PayDateTo;
            }
            set
            {
                this.SetValue("PayDateTo", ref m_PayDateTo, value);
            }
        }

        private Int32? m_StockSysNo;
        /// <summary>
        /// 仓库系统编号
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

        private String m_ReferenceID;
        /// <summary>
        /// 凭证号
        /// </summary>
        [Validate(ValidateType.MaxLength, 20)]
        public String ReferenceID
        {
            get
            {
                return this.m_ReferenceID;
            }
            set
            {
                this.SetValue("ReferenceID", ref m_ReferenceID, value);
            }
        }

        private Boolean m_NotInStock;
        /// <summary>
        /// 未入库（PO）
        /// </summary>
        public Boolean NotInStock
        {
            get
            {
                return this.m_NotInStock;
            }
            set
            {
                this.SetValue("NotInStock", ref m_NotInStock, value);
            }
        }

        private ECCentral.BizEntity.PO.PaySettleCompany? paySettleCompany;
        public ECCentral.BizEntity.PO.PaySettleCompany? PaySettleCompany
        {
            get
            {
                return paySettleCompany;
            }
            set
            {
                SetValue("PaySettleCompany", ref paySettleCompany, value);
            }
        }

        #region 扩展属性

        /// <summary>
        /// 单据类型列表
        /// </summary>
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList
        {
            get
            {
                var orderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>();
                return orderTypeList;
            }
        }

        /// <summary>
        /// 付款状态
        /// </summary>
        public List<KeyValuePair<PayItemStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PayItemStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 付款类型列表
        /// </summary>
        public List<KeyValuePair<PayItemStyle?, string>> StyleList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PayItemStyle>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 发票状态列表
        /// </summary>
        public List<KeyValuePair<PayableInvoiceStatus?, string>> InvoiceStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PayableInvoiceStatus>(EnumConverter.EnumAppendItemType.All);
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

        public List<KeyValuePair<ECCentral.BizEntity.PO.PaySettleCompany?, string>> PaySettleCompanyList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public String CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
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

        #endregion 延迟加载的属性
    }
}
