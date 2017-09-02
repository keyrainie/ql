using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PostIncomeQueryResultVM : ModelBase
    {
        private List<PostIncomeVM> m_ResultList;
        public List<PostIncomeVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        public int TotalCount
        {
            get;
            set;
        }
    }

    public class PostIncomeVM : ModelBase
    {
        private bool m_IsChecked;
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        private int? m_SysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                base.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private string m_SOSysNo;
        /// <summary>
        /// 订单系统编号
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
        /// 实收金额，用于UI展示
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d+(\.\d{0,})?$")]
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

        /// <summary>
        /// 用于计算的实收金额
        /// </summary>
        public decimal? IncomeAmtForCalc
        {
            get
            {
                decimal incomeAmt;
                return decimal.TryParse(IncomeAmt, out incomeAmt) ? incomeAmt : new Nullable<decimal>();
            }
            set
            {
                IncomeAmt = value.ToString();
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

        private DateTime? m_CreateDate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate
        {
            get
            {
                return m_CreateDate;
            }
            set
            {
                base.SetValue("CreateDate", ref m_CreateDate, value);
            }
        }

        private DateTime? m_ModifyDate;
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? ModifyDate
        {
            get
            {
                return m_ModifyDate;
            }
            set
            {
                base.SetValue("ModifyDate", ref m_ModifyDate, value);
            }
        }

        private DateTime? m_IncomeDate;
        /// <summary>
        /// 收款日期
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? IncomeDate
        {
            get
            {
                return m_IncomeDate;
            }
            set
            {
                base.SetValue("IncomeDate", ref m_IncomeDate, value);
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
        [Validate(ValidateType.Required)]
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

        private string m_BankNo;
        /// <summary>
        /// 银行流水号
        /// </summary>
        public string BankNo
        {
            get
            {
                return m_BankNo;
            }
            set
            {
                base.SetValue("BankNo", ref m_BankNo, value);
            }
        }

        private string m_Notes;
        /// <summary>
        /// 备注
        /// </summary>
        public string Notes
        {
            get
            {
                return m_Notes;
            }
            set
            {
                base.SetValue("Notes", ref m_Notes, value);
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

        private string m_ConfirmUser;
        /// <summary>
        /// 确认人
        /// </summary>
        public string ConfirmUser
        {
            get
            {
                return m_ConfirmUser;
            }
            set
            {
                base.SetValue("ConfirmUser", ref m_ConfirmUser, value);
            }
        }

        private DateTime? m_ConfirmDate;
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? ConfirmDate
        {
            get
            {
                return m_ConfirmDate;
            }
            set
            {
                base.SetValue("ConfirmDate", ref m_ConfirmDate, value);
            }
        }

        private string m_HandleUser;
        /// <summary>
        /// 处理人
        /// </summary>
        public string HandleUser
        {
            get
            {
                return m_HandleUser;
            }
            set
            {
                base.SetValue("HandleUser", ref m_HandleUser, value);
            }
        }

        private bool? m_IsHandled;
        /// <summary>
        /// 是否已处理，UI扩展字段
        /// </summary>
        public bool? IsHandled
        {
            get
            {
                if (!m_IsHandled.HasValue)
                {
                    m_IsHandled = (HandleStatus == PostIncomeHandleStatus.Handled);
                }
                return m_IsHandled;
            }
            set
            {
                base.SetValue("IsHandled", ref m_IsHandled, value);
                HandleStatus = m_IsHandled.Value ? PostIncomeHandleStatus.Handled : PostIncomeHandleStatus.WaitingHandle;
            }
        }

        private PostIncomeHandleStatus? m_HandleStatus;
        /// <summary>
        /// 处理情况
        /// </summary>
        public PostIncomeHandleStatus? HandleStatus
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

        private PostIncomeStatus? m_ConfirmStatus;
        /// <summary>
        /// 确认状态
        /// </summary>
        public PostIncomeStatus? ConfirmStatus
        {
            get
            {
                return m_ConfirmStatus;
            }
            set
            {
                base.SetValue("ConfirmStatus", ref m_ConfirmStatus, value);
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

        private DateTime? m_OutTime;
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? OutTime
        {
            get
            {
                return m_OutTime;
            }
            set
            {
                base.SetValue("OutTime", ref m_OutTime, value);
            }
        }

        private DateTime? m_OrderTime;
        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime? OrderTime
        {
            get
            {
                return m_OrderTime;
            }
            set
            {
                base.SetValue("OrderTime", ref m_OrderTime, value);
            }
        }

        private int? m_OrderSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? OrderSysNo
        {
            get
            {
                return m_OrderSysNo;
            }
            set
            {
                base.SetValue("OrderSysNo", ref m_OrderSysNo, value);
            }
        }

        private string m_CSNotes;
        /// <summary>
        /// 客服备注
        /// </summary>
        public string CSNotes
        {
            get
            {
                return m_CSNotes;
            }
            set
            {
                base.SetValue("CSNotes", ref m_CSNotes, value);
            }
        }

        private string m_ConfirmedSOSysNoList;
        /// <summary>
        /// CS确认的订单号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^(\\d{1,9}\\.){0,49}(\\d{1,9}){1}(\\.)?$")]
        public string ConfirmedSOSysNoList
        {
            get
            {
                return m_ConfirmedSOSysNoList.EmptyIfNull();
            }
            set
            {
                base.SetValue("ConfirmedSOSysNoList", ref m_ConfirmedSOSysNoList, value);
            }
        }
    }
}