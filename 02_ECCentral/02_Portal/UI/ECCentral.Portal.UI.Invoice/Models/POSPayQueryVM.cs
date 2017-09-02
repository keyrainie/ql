using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class POSPayQueryVM : ModelBase
    {
        private string m_SOSysNo;
        /// <summary>
        /// 订单系统编号,多个订单号之间用.隔开
        /// </summary>
        [Validate(ValidateType.Regex, @"^([1-9][0-9]{0,8})?(\.[1-9][0-9]{0,8})*$", ErrorMessageResourceName = "Msg_ValidateNoList", ErrorMessageResourceType = typeof(ResPOSPayQuery))]
        public string SOSysNo
        {
            get
            {
                return m_SOSysNo;
            }
            set
            {
                base.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private string m_POSTerminalID;
        /// <summary>
        /// POS终端号
        /// </summary>
        public string POSTerminalID
        {
            get
            {
                return m_POSTerminalID;
            }
            set
            {
                base.SetValue("POSTerminalID", ref m_POSTerminalID, value);
            }
        }

        private DateTime? m_OutDateFrom;
        /// <summary>
        /// 出库时间（从）
        /// </summary>
        public DateTime? OutDateFrom
        {
            get
            {
                return m_OutDateFrom;
            }
            set
            {
                base.SetValue("OutDateFrom", ref m_OutDateFrom, value);
            }
        }

        private DateTime? m_OutDateTo;
        /// <summary>
        /// 出库时间（到）
        /// </summary>
        public DateTime? OutDateTo
        {
            get
            {
                return m_OutDateTo;
            }
            set
            {
                base.SetValue("OutDateTo", ref m_OutDateTo, value);
            }
        }

        private DateTime? m_PayedDateFrom;
        /// <summary>
        /// POS收款时间（从）
        /// </summary>
        public DateTime? PayedDateFrom
        {
            get
            {
                return m_PayedDateFrom;
            }
            set
            {
                base.SetValue("PayedDateFrom", ref m_PayedDateFrom, value);
            }
        }

        private DateTime? m_PayedDateTo;
        /// <summary>
        /// POS收款时间（到）
        /// </summary>
        public DateTime? PayedDateTo
        {
            get
            {
                return m_PayedDateTo;
            }
            set
            {
                base.SetValue("PayedDateTo", ref m_PayedDateTo, value);
            }
        }

        private SOIncomeStatus? m_SOIncomeStatus;
        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? SOIncomeStatus
        {
            get
            {
                return m_SOIncomeStatus;
            }
            set
            {
                base.SetValue("SOIncomeStatus", ref m_SOIncomeStatus, value);
            }
        }

        private POSPayType? m_POSPayType;
        /// <summary>
        /// POS支付方式
        /// </summary>
        public POSPayType? POSPayType
        {
            get
            {
                return m_POSPayType;
            }
            set
            {
                base.SetValue("POSPayType", ref m_POSPayType, value);
            }
        }

        private AutoConfirmStatus? m_AutoConfirmStatus;
        /// <summary>
        /// POS自动确认状态
        /// </summary>
        public AutoConfirmStatus? AutoConfirmStatus
        {
            get
            {
                return m_AutoConfirmStatus;
            }
            set
            {
                base.SetValue("AutoConfirmStatus", ref m_AutoConfirmStatus, value);
            }
        }

        private string m_CombineNumber;
        /// <summary>
        /// 合单号
        /// </summary>
        public string CombineNumber
        {
            get
            {
                return m_CombineNumber;
            }
            set
            {
                base.SetValue("CombineNumber", ref m_CombineNumber, value);
            }
        }

        /// <summary>
        /// 收款单状态列表
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> SOIncomeStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// POS支付方式列表
        /// </summary>
        public List<KeyValuePair<POSPayType?, string>> POSPayTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<POSPayType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 自动确认状态列表
        /// </summary>
        public List<KeyValuePair<AutoConfirmStatus?, string>> AutoConfirmStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<AutoConfirmStatus>(EnumConverter.EnumAppendItemType.All);
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
    }
}