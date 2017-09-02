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
    public class TrackingInfoQueryVM : ModelBase
    {
        private String m_OrderSysNo;
        /// <summary>
        /// 单据ID
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9][0-9]{0,9}(\.[1-9][0-9]{0,9}){0,10}$")]
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

        private SOIncomeOrderType? m_OrderType;
        /// <summary>
        /// 单据类型
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

        private SOIncomeOrderStyle? m_IncomeStyle;
        /// <summary>
        /// 收款类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeStyle
        {
            get
            {
                return this.m_IncomeStyle;
            }
            set
            {
                this.SetValue("IncomeStyle", ref m_IncomeStyle, value);
            }
        }

        private DateTime? m_OutFromDate;
        /// <summary>
        /// 出库日期（从）
        /// </summary>
        public DateTime? OutFromDate
        {
            get
            {
                return this.m_OutFromDate;
            }
            set
            {
                this.SetValue("OutFromDate", ref m_OutFromDate, value);
            }
        }

        private DateTime? m_OutToDate;
        /// <summary>
        /// 出库日期（到）
        /// </summary>
        public DateTime? OutToDate
        {
            get
            {
                return this.m_OutToDate;
            }
            set
            {
                this.SetValue("OutToDate", ref m_OutToDate, value);
            }
        }

        private Int32? m_PayTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public Int32? PayTypeSysNo
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

        private Int32? m_ShipTypeSysNo;
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public Int32? ShipTypeSysNo
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

        private Int32? m_LossType;
        /// <summary>
        /// 损失类型
        /// </summary>
        public Int32? LossType
        {
            get
            {
                return this.m_LossType;
            }
            set
            {
                this.SetValue("LossType", ref m_LossType, value);
            }
        }

        private String m_ResponsibleUserName;
        /// <summary>
        /// 收款单责任人姓名
        /// </summary>
        public String ResponsibleUserName
        {
            get
            {
                return this.m_ResponsibleUserName;
            }
            set
            {
                this.SetValue("ResponsibleUserName", ref m_ResponsibleUserName, value);
            }
        }

        private TrackingInfoStyle? m_InType;
        /// <summary>
        /// 单据添加方式
        /// </summary>
        public TrackingInfoStyle? InType
        {
            get
            {
                return this.m_InType;
            }
            set
            {
                this.SetValue("InType", ref m_InType, value);
            }
        }

        private String m_ChannelID;
        public String ChannelID
        {
            get
            {
                return this.m_ChannelID;
            }
            set
            {
                this.SetValue("ChannelID", ref m_ChannelID, value);
            }
        }

        private bool m_HasStatusFollow;
        /// <summary>
        /// 进度：业务跟进
        /// </summary>
        public bool HasStatusFollow
        {
            get
            {
                return m_HasStatusFollow;
            }
            set
            {
                base.SetValue("HasStatusFollow", ref m_HasStatusFollow, value);
            }
        }

        private bool m_HasStatusSubmit;
        /// <summary>
        /// 进度：提交报损
        /// </summary>
        public bool HasStatusSubmit
        {
            get
            {
                return m_HasStatusSubmit;
            }
            set
            {
                base.SetValue("HasStatusSubmit", ref m_HasStatusSubmit, value);
            }
        }

        private bool m_HasStatusConfirm;
        /// <summary>
        /// 进度：核销完毕
        /// </summary>
        public bool HasStatusConfirm
        {
            get
            {
                return m_HasStatusConfirm;
            }
            set
            {
                base.SetValue("HasStatusConfirm", ref m_HasStatusConfirm, value);
            }
        }

        #region 扩展属性

        /// <summary>
        /// 单据类型列表
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderType?, string>> OrderTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeOrderType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 收款类型列表
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderStyle?, string>> IncomeStyleList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeOrderStyle>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 单据添加方式列表
        /// </summary>
        public List<KeyValuePair<TrackingInfoStyle?, string>> InTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<TrackingInfoStyle>(EnumConverter.EnumAppendItemType.All);
            }
        }

        //单据处理进度列表
        public List<KeyValuePair<TrackingInfoStatus?, string>> TrackingStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<TrackingInfoStatus>(EnumConverter.EnumAppendItemType.None);
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

        public String CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        #endregion 扩展属性

        #region 延迟加载的属性

        private List<CodeNamePair> m_ResponsibleUserList;
        /// <summary>
        /// 收款单责任人列表
        /// </summary>
        public List<CodeNamePair> ResponsibleUserList
        {
            get
            {
                return m_ResponsibleUserList;
            }
            set
            {
                base.SetValue("ResponsibleUserList", ref m_ResponsibleUserList, value);
            }
        }

        private List<CodeNamePair> m_LossTypeList;
        /// <summary>
        /// 损失类型列表
        /// </summary>
        public List<CodeNamePair> LossTypeList
        {
            get
            {
                return m_LossTypeList;
            }
            set
            {
                base.SetValue("LossTypeList", ref m_LossTypeList, value);
            }
        }

        #endregion 延迟加载的属性
    }
}