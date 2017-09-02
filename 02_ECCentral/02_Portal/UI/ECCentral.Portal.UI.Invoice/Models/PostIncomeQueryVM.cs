using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PostIncomeQueryVM : ModelBase
    {
        private string m_SOSysNo;
        /// <summary>
        /// 订单号
        /// </summary>
        [Validate(ValidateType.Regex, "^(\\d{0,9})$")]
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

        private string m_IncomeAmt;
        /// <summary>
        /// 实收金额
        /// </summary>
        public string IncomeAmt
        {
            get
            {
                return m_IncomeAmt;
            }
            set
            {
                base.SetValue("IncomeAmt", ref m_IncomeAmt, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        /// <summary>
        /// 创建时间（从）
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get
            {
                return m_CreateDateFrom;
            }
            set
            {
                base.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private DateTime? m_CreateDateTo;
        /// <summary>
        /// 创建时间（到）
        /// </summary>
        public DateTime? CreateDateTo
        {
            get
            {
                return m_CreateDateTo;
            }
            set
            {
                base.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private DateTime? m_IncomeDateFrom;
        /// <summary>
        /// 收款时间（从）
        /// </summary>
        public DateTime? IncomeDateFrom
        {
            get
            {
                return m_IncomeDateFrom;
            }
            set
            {
                base.SetValue("IncomeDateFrom", ref m_IncomeDateFrom, value);
            }
        }

        private DateTime? m_IncomeDateTo;
        /// <summary>
        /// 收款时间（到）
        /// </summary>
        public DateTime? IncomeDateTo
        {
            get
            {
                return m_IncomeDateTo;
            }
            set
            {
                base.SetValue("IncomeDateTo", ref m_IncomeDateTo, value);
            }
        }

        private PostIncomeHandleStatusUI? m_HandleStatus;
        /// <summary>
        /// 处理情况
        /// </summary>
        public PostIncomeHandleStatusUI? HandleStatus
        {
            get
            {
                return m_HandleStatus;
            }
            set
            {
                base.SetValue("HandleStatus", ref m_HandleStatus, value);
            }
        }

        private string m_PayBank;
        /// <summary>
        /// 付款行
        /// </summary>
        public string PayBank
        {
            get
            {
                return m_PayBank;
            }
            set
            {
                base.SetValue("PayBank", ref m_PayBank, value);
            }
        }

        private string m_IncomeBank;
        /// <summary>
        /// 收款行
        /// </summary>
        public string IncomeBank
        {
            get
            {
                return m_IncomeBank;
            }
            set
            {
                base.SetValue("IncomeBank", ref m_IncomeBank, value);
            }
        }

        private string m_CreateUser;
        /// <summary>
        /// 制单人
        /// </summary>
        public string CreateUser
        {
            get
            {
                return m_CreateUser;
            }
            set
            {
                base.SetValue("CreateUser", ref m_CreateUser, value);
            }
        }

        private string m_PayUser;
        /// <summary>
        /// 付款人
        /// </summary>
        public string PayUser
        {
            get
            {
                return m_PayUser;
            }
            set
            {
                base.SetValue("PayUser", ref m_PayUser, value);
            }
        }

        private string m_AuditUser;
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser
        {
            get
            {
                return m_AuditUser;
            }
            set
            {
                base.SetValue("AuditUser", ref m_AuditUser, value);
            }
        }

        private string m_ConfirmedSOSysNoList;
        /// <summary>
        /// CS确认的订单号，多个订单号之间用逗号分隔
        /// </summary>
        [Validate(ValidateType.Regex, "^(\\d{1,9}\\.){0,49}(\\d{1,9}){1}$")]
        public string ConfirmedSOSysNoList
        {
            get
            {
                return m_ConfirmedSOSysNoList;
            }
            set
            {
                base.SetValue("ConfirmedSOSysNoList", ref m_ConfirmedSOSysNoList, value);
            }
        }

        private bool m_IsMatchSO;
        /// <summary>
        /// 是否匹配SO单
        /// </summary>
        public bool IsMatchSO
        {
            get
            {
                return m_IsMatchSO;
            }
            set
            {
                base.SetValue("IsMatchSO", ref m_IsMatchSO, value);
            }
        }

        /// <summary>
        /// 处理状态，UI端的处理状态跟服务端的不同，有组合属性
        /// </summary>
        public List<KeyValuePair<PostIncomeHandleStatusUI?, string>> HandleStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PostIncomeHandleStatusUI>(EnumConverter.EnumAppendItemType.All);
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

        public string CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }
    }
}