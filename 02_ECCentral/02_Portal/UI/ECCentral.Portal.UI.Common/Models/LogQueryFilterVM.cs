using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Common.Models
{
    public class LogQueryFilterVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        public LogQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
        }
        private string m_TicketSysNo;
        public string TicketSysNo
        {
            get { return m_TicketSysNo; }
            set { this.SetValue("TicketSysNo", ref m_TicketSysNo, value); }
        }

        public DateTime? m_StartDate;
        public DateTime? StartDate
        {
            get { return m_StartDate; }
            set { this.SetValue("StartDate", ref m_StartDate, value); }
        }

        public DateTime? m_EndDate;
        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        public bool m_CancelOutStore;
        public bool CancelOutStore
        {
            get { return m_CancelOutStore; }
            set { this.SetValue("CancelOutStore", ref m_CancelOutStore, value); }
        }

        public bool m_ISSOLog;
        public bool ISSOLog
        {
            get { return m_ISSOLog; }
            set { this.SetValue("ISSOLog", ref m_ISSOLog, value); }
        }
    }
}
