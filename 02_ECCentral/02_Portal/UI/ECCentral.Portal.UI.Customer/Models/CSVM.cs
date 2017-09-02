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
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Common;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CSSetVM : ModelBase
    {
        public CSSetVM()
        {
            PageInfo = new QueryFilter.Common.PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        private CSRole? _role;

        public CSRole? Role
        {
            get { return _role; }
            set { SetValue("Role", ref _role, value); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetValue<string>("Name", ref _name, value); }
        }
        public bool IsGetUnderling
        {
            get;
            set;
        }

        public List<CSVM> csList;

        public List<KeyValuePair<CSRole?, string>> RoleList
        {
            get;
            set;
        }
    }

    public class CSMaintainVM : ModelBase
    {
        public CSMaintainVM()
        {
            csvm = new CSVM();
            RoleList = new List<KeyValuePair<CSRole?, string>>();
            CSCheckBoxList = new ObservableCollection<CSCheckBoxVM>();
            CSList = new List<CSVM>();
            AllCSList = new List<CSVM>();
        }
        public CSVM csvm { get; set; }

        public List<KeyValuePair<CSRole?, string>> RoleList
        {
            get;
            set;
        }
        private List<CSVM> _leaderList;
        public List<CSVM> LeaderList
        {
            get { return _leaderList; }
            set { base.SetValue("LeaderList", ref _leaderList, value); }
        }
        private List<CSVM> _managerList;
        public List<CSVM> ManagerList
        {
            get { return _managerList; }
            set { base.SetValue("ManagerList", ref _managerList, value); }
        }

        private List<CSVM> _CSList;
        public List<CSVM> CSList
        {
            get { return _CSList; }
            set { base.SetValue("CSList", ref _CSList, value); }
        }
        private List<CSVM> _AllCSList;
        public List<CSVM> AllCSList
        {
            get { return _AllCSList; }
            set { base.SetValue("AllCSList", ref _AllCSList, value); }
        }
        private ObservableCollection<CSCheckBoxVM> _CSCheckBoxVM;
        public ObservableCollection<CSCheckBoxVM> CSCheckBoxList
        {
            get { return _CSCheckBoxVM; }
            set { base.SetValue("CSCheckBoxList", ref _CSCheckBoxVM, value); }
        }

        private bool _isSelectAll;
        public bool IsSelectAll
        {
            get { return _isSelectAll; }
            set { base.SetValue("IsSelectAll", ref _isSelectAll, value); }
        }
    }

    public class CSVM : ModelBase
    {
        private Int32? m_Sysno;
        public Int32? SysNo
        {
            get { return this.m_Sysno; }
            set { this.SetValue("SysNo", ref m_Sysno, value); }
        }

        private String m_UserName;
        public String UserName
        {
            get { return this.m_UserName; }
            set { this.SetValue("UserName", ref m_UserName, value); }
        }

        private Int32? m_UnderlingNum;
        public Int32? UnderlingNum
        {
            get { return this.m_UnderlingNum; }
            set { this.SetValue("UnderlingNum", ref m_UnderlingNum, value); }
        }

        private CSRole? m_Role;
        public CSRole? Role
        {
            get { return this.m_Role; }
            set { this.SetValue("Role", ref m_Role, value); }
        }

        private Int32? m_IPPUserSysNo;
        public Int32? IPPUserSysNo
        {
            get { return this.m_IPPUserSysNo; }
            set { this.SetValue("IPPUserSysNo", ref m_IPPUserSysNo, value); }
        }

        private Int32? m_LeaderSysNo;
        public Int32? LeaderSysNo
        {
            get { return this.m_LeaderSysNo; }
            set { this.SetValue("LeaderSysNo", ref m_LeaderSysNo, value); }
        }

        private Int32? m_LeaderIPPUserSysNo;
        public Int32? LeaderIPPUserSysNo
        {
            get { return this.m_LeaderIPPUserSysNo; }
            set { this.SetValue("LeaderIPPUserSysNo", ref m_LeaderIPPUserSysNo, value); }
        }

        private String m_LeaderUserName;
        public String LeaderUserName
        {
            get { return this.m_LeaderUserName; }
            set { this.SetValue("LeaderUserName", ref m_LeaderUserName, value); }
        }

        private Int32? m_ManagerSysNo;
        public Int32? ManagerSysNo
        {
            get { return this.m_ManagerSysNo; }
            set { this.SetValue("ManagerSysNo", ref m_ManagerSysNo, value); }
        }

        private Int32? m_ManagerIPPUserSysNo;
        public Int32? ManagerIPPUserSysNo
        {
            get { return this.m_ManagerIPPUserSysNo; }
            set { this.SetValue("ManagerIPPUserSysNo", ref m_ManagerIPPUserSysNo, value); }
        }

        private String m_ManagerUserName;
        public String ManagerUserName
        {
            get { return this.m_ManagerUserName; }
            set { this.SetValue("ManagerUserName", ref m_ManagerUserName, value); }
        }

        private List<Int32> m_CSIPPUserSysNos;
        public List<Int32> CSIPPUserSysNos
        {
            get { return this.m_CSIPPUserSysNos; }
            set { this.SetValue("CSIPPUserSysNos", ref m_CSIPPUserSysNos, value); }
        }

        private List<String> m_CSUserNames;
        public List<String> CSUserNames
        {
            get { return this.m_CSUserNames; }
            set { this.SetValue("CSUserNames", ref m_CSUserNames, value); }
        }
        public Visibility TextBlockVisibility
        {
            get
            {
                if (UnderlingNum <= 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility HyperlinkButtonVisibility
        {
            get
            {
                if (UnderlingNum > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
    }

    public class CSCheckBoxVM : ModelBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { base.SetValue("Name", ref _name, value); }
        }
        private int _SysNo;
        public int SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }
    }
}
