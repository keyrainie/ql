using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PostIncomeConfirmInfoVM : ModelBase
    {
        private string m_SysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNo
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

        private string m_PostIncomeSysNo;
        /// <summary>
        /// 电汇邮局收款单编号
        /// </summary>
        public string PostIncomeSysNo
        {
            get
            {
                return m_PostIncomeSysNo;
            }
            set
            {
                base.SetValue("PostIncomeSysNo", ref m_PostIncomeSysNo, value);
            }
        }

        private string m_ConfirmedSoSysNo;
        /// <summary>
        /// CS确认的订单编号
        /// </summary>
        public string ConfirmedSoSysNo
        {
            get
            {
                return m_ConfirmedSoSysNo;
            }
            set
            {
                base.SetValue("ConfirmedSoSysNo", ref m_ConfirmedSoSysNo, value);
            }
        }

        private PostIncomeConfirmStatus? m_Status;
        /// <summary>
        /// 状态
        /// </summary>
        public PostIncomeConfirmStatus? Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                base.SetValue("Status", ref m_Status, value);
            }
        }

        public List<KeyValuePair<PostIncomeConfirmStatus?, string>> ConfirmStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PostIncomeConfirmStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }
    }
}