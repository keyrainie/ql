using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ControlPanelUserQueryFilterVM : ModelBase
    {
        public List<KeyValuePair<ControlPanelUserStatus?, string>> ListStatus { get; set; }

        public ControlPanelUserQueryFilterVM()
        {
            this.ListStatus = EnumConverter.GetKeyValuePairs<ControlPanelUserStatus>(EnumConverter.EnumAppendItemType.All);
            
            this.PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }

        private  string m_LoginName;
        public string LoginName
        {
            get { return m_LoginName; }
            set { this.SetValue("LoginName", ref m_LoginName, value); }
        }

        private string m_DisplayName;
        public string DisplayName
        {
            get { return m_DisplayName; }
            set { this.SetValue("DisplayName", ref m_DisplayName, value); }
        }

        private string m_DepartmentCode;
        public string DepartmentCode
        {
            get { return m_DepartmentCode; }
            set { this.SetValue("DepartmentCode", ref m_DepartmentCode, value); }
        }

        private ControlPanelUserStatus? m_Status;
        public ControlPanelUserStatus? Status
        {
            get { return m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }
    }
}
