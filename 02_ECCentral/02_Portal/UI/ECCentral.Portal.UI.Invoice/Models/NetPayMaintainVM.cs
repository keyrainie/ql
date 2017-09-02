using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// ά��NetPayʱ��ViewModel
    /// </summary>
    public class NetPayMaintainVM : ModelBase
    {
        public NetPayMaintainVM()
        {
            PayInfo = new PayInfoVM();
            RefundInfo = new RefundInfoVM();
            IsForceCheck = false;
        }

        private int? m_SysNo;
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                this.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private bool? m_IsForceCheck;
        /// <summary>
        /// �Ƿ�ǿ�ƺ���
        /// </summary>
        public bool? IsForceCheck
        {
            get
            {
                return m_IsForceCheck;
            }
            set
            {
                base.SetValue("IsForceCheck", ref m_IsForceCheck, value);
            }
        }

        private NetPaySource? m_Source;
        /// <summary>
        /// ��Դ
        /// </summary>
        public NetPaySource? Source
        {
            get
            {
                return m_Source;
            }
            set
            {
                base.SetValue("Source", ref m_Source, value);
            }
        }

        private NetPayStatus? m_Status;
        public NetPayStatus? Status
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

        private PayInfoVM m_PayInfo;
        /// <summary>
        /// ֧����Ϣ
        /// </summary>
        public PayInfoVM PayInfo
        {
            get
            {
                return m_PayInfo;
            }
            set
            {
                base.SetValue("PayInfo", ref m_PayInfo, value);
            }
        }

        private RefundInfoVM m_RefundInfo;
        /// <summary>
        /// �˿���Ϣ
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
    }
}