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
    public class LogQueryResultVM : ModelBase
    {
        private int? m_SysNo;
        public int? SysNo
        {
            get { return m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private int? m_TicketSysNo;
        public int? TicketSysNo
        {
            get { return m_TicketSysNo; }
            set { this.SetValue("TicketSysNo", ref m_TicketSysNo, value); }
        }

        public string m_OPtTime;
        public string OPtTime
        {
            get { return m_OPtTime; }
            set { this.SetValue("OPtTime", ref m_OPtTime, value); }
        }

        private int? m_TicketType;
        public int? TicketType
        {
            get { return m_TicketType; }
            set { this.SetValue("TicketType", ref m_TicketType, value); }
        }

        public string m_OptIp;
        public string OptIp
        {
            get { return m_OptIp; }
            set { this.SetValue("OptIp", ref m_OptIp, value); }
        }

        public string m_Note;
        public string Note
        {
            get { return m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        public string m_Outtime;
        public string Outtime
        {
            get { return m_Outtime; }
            set { this.SetValue("Outtime", ref m_Outtime, value); }
        }

        private int? m_Status;
        public int? Status
        {
            get { return m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private int? m_OptType;
        public int? OptType
        {
            get { return m_OptType; }
            set { this.SetValue("OptType", ref m_OptType, value); }
        }

        public string m_Username;
        public string Username
        {
            get { return m_Username; }
            set { this.SetValue("Username", ref m_Username, value); }
        }

        /// <summary>
        /// get the ticket type description
        /// </summary>
        public string LogType
        {
            get
            {
                if (TicketType.HasValue)
                {
                    return EnumConverter.GetDescription(TicketType, typeof(ECCentral.BizEntity.Common.BizLogType));
                }
                else return string.Empty;
            }
        }

        public string SOOperationType
        {
            get
            {
                if (OptType.HasValue&&(OptType.ToString().StartsWith("6006") == true || OptType.ToString().StartsWith("4090") == true))
                    return EnumConverter.GetDescription(OptType, typeof(ECCentral.BizEntity.Common.BizLogType));
                else
                    return OptType == 0 ? "创建" : "更新";
            }
        }

        /// <summary>
        /// get so status desciption
        /// </summary>
        public string SoStatus
        {
            get
            {
                if (Status.HasValue)
                {
                    return EnumConverter.GetDescription(Status, typeof(ECCentral.BizEntity.Common.LogSOStatus));
                }
                else return string.Empty;
            }
        }


        public string WhoOperation
        {
            get { return Username + " " + OPtTime; }
        }

    }
}
