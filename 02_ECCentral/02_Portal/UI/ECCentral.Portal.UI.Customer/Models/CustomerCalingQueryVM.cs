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
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerCalingQueryVM : ModelBase
    {
        public CustomerCalingQueryVM()
        {
            CallingRMAStatusList = EnumConverter.GetKeyValuePairs<CallingRMAStatus>(EnumConverter.EnumAppendItemType.All);
            ComplainStatusList = EnumConverter.GetKeyValuePairs<SOComplainStatus>(EnumConverter.EnumAppendItemType.All); ;
            CallingStatusList = EnumConverter.GetKeyValuePairs<CallsEventsStatus>(EnumConverter.EnumAppendItemType.All);
            OperationSignList = EnumConverter.GetKeyValuePairs<OperationSignType>(EnumConverter.EnumAppendItemType.All);
            SoList = new List<SOVM>();
            CallingList = new List<CallingVM>();
            ComplainList = new List<ComplainVM>();
            RMAList = new List<RMAVM>();
        }
        public List<KeyValuePair<CallingRMAStatus?, string>> CallingRMAStatusList { get; set; }
        public List<KeyValuePair<CallsEventsStatus?, string>> CallingStatusList { get; set; }
        public List<KeyValuePair<SOComplainStatus?, string>> ComplainStatusList { get; set; }
        public List<KeyValuePair<OperationSignType?, string>> OperationSignList { get; set; }
        public ObservableCollection<CSInfo> CSList { get; set; }

        private List<SOVM> _SoList;
        public List<SOVM> SoList
        {
            get { return _SoList; }
            set { this.SetValue("SoList", ref _SoList, value); }
        }

        private List<CallingVM> _CallingList;

        public List<CallingVM> CallingList
        {
            get { return _CallingList; }
            set { this.SetValue("CallingList", ref _CallingList, value); }
        }
        private List<ComplainVM> _ComplainList;

        public List<ComplainVM> ComplainList
        {
            get { return _ComplainList; }
            set { this.SetValue("ComplainList", ref _ComplainList, value); }
        }
        private List<RMAVM> _RMAList;

        public List<RMAVM> RMAList
        {
            get { return _RMAList; }
            set { this.SetValue("RMAList", ref _RMAList, value); }
        }
        private DateTime? _CreateDateFrom;

        public DateTime? CreateDateFrom
        {
            get { return _CreateDateFrom; }
            set { this.SetValue("CreateDateFrom", ref _CreateDateFrom, value); }
        }
        private DateTime? _CreateDateTo;

        public DateTime? CreateDateTo
        {
            get { return _CreateDateTo; }
            set { this.SetValue("CreateDateTo", ref _CreateDateTo, value); }
        }
        private string _CustomerID;

        public string CustomerID
        {
            get { return _CustomerID; }
            set { this.SetValue("CustomerID", ref _CustomerID, value); }
        }
        private string _OrderSysNo;

        public string OrderSysNo
        {
            get { return _OrderSysNo; }
            set { this.SetValue("OrderSysNo", ref _OrderSysNo, value); }
        }
        private string _CustomerName;

        public string CustomerName
        {
            get { return _CustomerName; }
            set { this.SetValue("CustomerName", ref _CustomerName, value); }
        }
        private string _PhoneORCellphone;

        public string PhoneORCellphone
        {
            get { return _PhoneORCellphone; }
            set { this.SetValue("PhoneORCellphone", ref _PhoneORCellphone, value); }
        }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { this.SetValue("Address", ref _Address, value); }
        }
        private CallingRMAStatus? _RMAStatus;

        public CallingRMAStatus? RMAStatus
        {
            get { return _RMAStatus; }
            set { this.SetValue("RMAStatus", ref _RMAStatus, value); }
        }
        private CallsEventsStatus? _CallingStatus;

        public CallsEventsStatus? CallingStatus
        {
            get { return _CallingStatus; }
            set { this.SetValue("CallingStatus", ref _CallingStatus, value); }
        }
        private SOComplainStatus? _ComplainStatus;

        public SOComplainStatus? ComplainStatus
        {
            get { return _ComplainStatus; }
            set { this.SetValue("ComplainStatus", ref _ComplainStatus, value); }
        }
        private bool _IsReopen;

        public bool IsReopen
        {
            get { return _IsReopen; }
            set
            {
                this.SetValue("IsReopen", ref _IsReopen, value);
                if (!value)
                    ReopenCount = string.Empty;
            }
        }
        private string _ReopenCount;

        public string ReopenCount
        {
            get { return _ReopenCount; }
            set { this.SetValue("ReopenCount", ref _ReopenCount, value); }
        }
        private OperationSignType? _OperaterCallingHours;

        public OperationSignType? OperaterCallingHours
        {
            get { return _OperaterCallingHours; }
            set
            {
                this.SetValue("OperaterCallingHours", ref _OperaterCallingHours, value);
                if (value == null)
                    CallingHours = string.Empty;
            }
        }

        private string _CallingHours;

        public string CallingHours
        {
            get { return _CallingHours; }
            set { this.SetValue("CallingHours", ref _CallingHours, value); }
        }

        private int? _LastUpdateUserSysNo;

        public int? LastUpdateUserSysNo
        {
            get { return _LastUpdateUserSysNo; }
            set { this.SetValue("LastUpdateUserSysNo", ref _LastUpdateUserSysNo, value); }
        }

        private OperationSignType? _OperaterCallingTimes;

        public OperationSignType? OperaterCallingTimes
        {
            get { return _OperaterCallingTimes; }
            set
            {
                this.SetValue("OperaterCallingTimes", ref _OperaterCallingTimes, value);
                if (value == null)
                    CallingTimes = string.Empty;
            }
        }
        private string _CallingTimes;

        public string CallingTimes
        {
            get { return _CallingTimes; }
            set { this.SetValue("CallingTimes", ref _CallingTimes, value); }
        }

        private DateTime? _FinishDateFrom;

        public DateTime? FinishDateFrom
        {
            get { return _FinishDateFrom; }
            set { this.SetValue("FinishDateFrom", ref _FinishDateFrom, value); }
        }
        private DateTime? _FinishDateTo;

        public DateTime? FinishDateTo
        {
            get { return _FinishDateTo; }
            set { this.SetValue("FinishDateTo", ref _FinishDateTo, value); }
        }
        private DateTime? _CloseDateFrom;

        public DateTime? CloseDateFrom
        {
            get { return _CloseDateFrom; }
            set { this.SetValue("CloseDateFrom", ref _CloseDateFrom, value); }
        }
        private DateTime? _CloseDateTo;

        public DateTime? CloseDateTo
        {
            get { return _CloseDateTo; }
            set { this.SetValue("CloseDateTo", ref _CloseDateTo, value); }
        }
        private string _LogTitle;

        public string LogTitle
        {
            get { return _LogTitle; }
            set { this.SetValue("LogTitle", ref _LogTitle, value); }
        }

        #region for ui
        public bool HasExportRight { get; set; }
        #endregion

    }
    public class SOVM : ModelBase
    {
        public int SOID { get; set; }
        public FPCheckStatus? IsFPSO { get; set; }
        public int? CustomerSysNo { get; set; }
        public string CustomerID { get; set; }
        public string AuditName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AuditTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string UpdatedMan { get; set; }
        public SOStatus? SOStatus { get; set; }

        public string SOURL
        {
            get { return string.Format("{0}", SOID); }
        }
        public string CustomerURL
        {
            get { return string.Format("{0}", CustomerSysNo); }
        }
    }
    public class CallingVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_CustomerSysNo;
        public Int32? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private String m_CustomerName;
        public String CustomerName
        {
            get { return this.m_CustomerName; }
            set { this.SetValue("CustomerName", ref m_CustomerName, value); }
        }

        private String m_Phone;
        public String Phone
        {
            get { return this.m_Phone; }
            set { this.SetValue("Phone", ref m_Phone, value); }
        }

        private String m_Email;
        public String Email
        {
            get { return this.m_Email; }
            set { this.SetValue("Email", ref m_Email, value); }
        }

        private String m_Address;
        public String Address
        {
            get { return this.m_Address; }
            set { this.SetValue("Address", ref m_Address, value); }
        }

        private Int32? m_OrderSysNo;
        public Int32? OrderSysNo
        {
            get { return this.m_OrderSysNo; }
            set { this.SetValue("OrderSysNo", ref m_OrderSysNo, value); }
        }

        private String m_FromLinkSource;
        public String FromLinkSource
        {
            get { return this.m_FromLinkSource; }
            set { this.SetValue("FromLinkSource", ref m_FromLinkSource, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private Int32? m_CallReason;
        public Int32? CallReason
        {
            get { return this.m_CallReason; }
            set { this.SetValue("CallReason", ref m_CallReason, value); }
        }

        private Int32? m_RecordOrigion;
        public Int32? RecordOrigion
        {
            get { return this.m_RecordOrigion; }
            set { this.SetValue("RecordOrigion", ref m_RecordOrigion, value); }
        }

        private CallsEventsStatus? m_Status;
        public CallsEventsStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }
        private DateTime? m_CreateDate;
        public DateTime? CreateDate
        {
            get { return this.m_CreateDate; }
            set { this.SetValue("CreateDate", ref m_CreateDate, value); }
        }

        private DateTime? m_CloseDate;
        public DateTime? CloseDate
        {
            get { return this.m_CloseDate; }
            set { this.SetValue("CloseDate", ref m_CloseDate, value); }
        }

        private Int32? m_CloseUserSysNo;
        public Int32? CloseUserSysNo
        {
            get { return this.m_CloseUserSysNo; }
            set { this.SetValue("CloseUserSysNo", ref m_CloseUserSysNo, value); }
        }

        private DateTime? _LastEditDate;

        public DateTime? LastEditDate
        {
            get { return _LastEditDate; }
            set { this.SetValue("LastEditDate", ref _LastEditDate, value); }
        }

        private string _LastEditUserName;

        public string LastEditUserName
        {
            get { return _LastEditUserName; }
            set { this.SetValue("LastEditUserName", ref _LastEditUserName, value); }
        }
        private List<CallsEventsFollowUpLog> m_LogList;
        public List<CallsEventsFollowUpLog> LogList
        {
            get { return this.m_LogList; }
            set { this.SetValue("LogList", ref m_LogList, value); }
        }

        private string _LogTitle;
        public string LogTitle
        {
            get { return _LogTitle; }
            set { this.SetValue("LogTitle", ref _LogTitle, value); }
        }

        private CallingReferenceType? _ReferenceType;

        public CallingReferenceType? ReferenceType
        {
            get { return _ReferenceType; }
            set
            {
                if (value == 0)
                    value = null;
                _ReferenceType = value;
            }
        }

        public Visibility HLBOpenVisible
        {
            get
            {
                return Status != CallsEventsStatus.Handled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility HLBCloseVisible
        {
            get
            {
                return Status == CallsEventsStatus.Replied ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility HLBReopenVisible
        {
            get
            {
                return Status == CallsEventsStatus.Handled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility HLBToComplanVisible
        {
            get
            {
                return !ReferenceType.HasValue && Status.Value == CallsEventsStatus.Replied ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility HLBToRMAVisible
        {
            get
            {
                return !ReferenceType.HasValue && Status.Value == CallsEventsStatus.Replied && OrderSysNo != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility TBToComplanVisible
        {
            get
            {
                return ReferenceType.HasValue && ReferenceType.Value == CallingReferenceType.Complain ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility TBToRMAVisible
        {
            get
            {
                return ReferenceType.HasValue && ReferenceType.Value == CallingReferenceType.RMA ? Visibility.Visible : Visibility.Collapsed;
            }
        }


    }
    public class ComplainVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }
        private string _ComplainSysNo;
        public string ComplainSysNo
        {
            get { return this._ComplainSysNo; }
            set { this.SetValue("ComplainSysNo", ref _ComplainSysNo, value); }
        }


        private int? _SOSysNo;

        public int? SOSysNo
        {
            get { return _SOSysNo; }
            set { this.SetValue("SOSysNo", ref _SOSysNo, value); }
        }
        private DateTime? _CreateDate;

        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set { this.SetValue("CreateDate", ref _CreateDate, value); }
        }

        private DateTime? _LastEditDate;

        public DateTime? LastEditDate
        {
            get { return _LastEditDate; }
            set { this.SetValue("LastEditDate", ref _LastEditDate, value); }
        }

        private String _Subject;
        public String Subject
        {
            get { return this._Subject; }
            set { this.SetValue("Subject", ref _Subject, value); }
        }

        private string _LastEditUserName;

        public string LastEditUserName
        {
            get { return _LastEditUserName; }
            set { this.SetValue("LastEditUserName", ref _LastEditUserName, value); }
        }


        private SOComplainStatus? _ComplainStatus;
        public SOComplainStatus? ComplainStatus
        {
            get { return this._ComplainStatus; }
            set { this.SetValue("ComplainStatus", ref _ComplainStatus, value); }
        }

    }
    public class RMAVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_RegisterSysNo;
        public Int32? RegisterSysNo
        {
            get { return this.m_RegisterSysNo; }
            set { this.SetValue("RegisterSysNo", ref m_RegisterSysNo, value); }
        }

        private String m_SONumber;
        public String SONumber
        {
            get { return this.m_SONumber; }
            set { this.SetValue("SONumber", ref m_SONumber, value); }
        }
        private CallingRMAStatus? m_Status;
        public CallingRMAStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }
        private DateTime? m_CreateDate;
        public DateTime? CreateTime
        {
            get { return this.m_CreateDate; }
            set { this.SetValue("CreateTime", ref m_CreateDate, value); }
        }

        private DateTime? m_CloseDate;
        public DateTime? CloseTime
        {
            get { return this.m_CloseDate; }
            set { this.SetValue("CloseTime", ref m_CloseDate, value); }
        }

        private int? m_UpdateUserSysNo;
        public int? UpdateUserSysNo
        {
            get { return this.m_UpdateUserSysNo; }
            set { this.SetValue("UpdateUserSysNo", ref m_UpdateUserSysNo, value); }
        }

        private string m_UpdateUserName;
        public string UpdateUserName
        {
            get { return this.m_UpdateUserName; }
            set { this.SetValue("UpdateUserName", ref m_UpdateUserName, value); }
        }
        private int? m_ProductSysNo;
        public int? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private string m_ProductID;
        public string ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private string m_ProductName;
        public string ProductName
        {
            get { return this.m_ProductName; }
            set { this.SetValue("ProductName", ref m_ProductName, value); }
        }

    }
}
