using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOComplaintReplyInfoVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32 m_ComplainSysNo;
        public Int32 ComplainSysNo
        {
            get { return this.m_ComplainSysNo; }
            set { this.SetValue("ComplainSysNo", ref m_ComplainSysNo, value); }
        }

        private SOComplainStatus m_Status;
        public SOComplainStatus Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private SOComplainReplyType m_ReplyType;
        public SOComplainReplyType ReplyType
        {
            get { return this.m_ReplyType; }
            set { this.SetValue("ReplyType", ref m_ReplyType, value); }
        }

        private String m_ReplyContent;
        public String ReplyContent
        {
            get { return this.m_ReplyContent; }
            set { this.SetValue("ReplyContent", ref m_ReplyContent, value); }
        }

        private Int32? m_ReplyUserSysNo;
        public Int32? ReplyUserSysNo
        {
            get { return this.m_ReplyUserSysNo; }
            set { this.SetValue("ReplyUserSysNo", ref m_ReplyUserSysNo, value); }
        }

        private String m_ReplyUserName;
        public String ReplyUserName
        {
            get { return this.m_ReplyUserName; }
            set { this.SetValue("ReplyUserName", ref m_ReplyUserName, value); }
        }

        private DateTime? m_ReplyTime;
        public DateTime? ReplyTime
        {
            get { return this.m_ReplyTime; }
            set { this.SetValue("ReplyTime", ref m_ReplyTime, value); }
        }

    }
}
