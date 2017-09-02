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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerCalingMaintainVM : ModelBase
    {
        public CustomerCalingMaintainVM()
        {
            CallStatusList = EnumConverter.GetKeyValuePairs<CallsEventsStatus>(EnumConverter.EnumAppendItemType.Select);
            RecordOrigionList = new ObservableCollection<CodeNamePair>();
            CallReasonList = EnumConverter.GetKeyValuePairs<CustomerCallReason>(EnumConverter.EnumAppendItemType.Select);
            SubjectList = new ObservableCollection<CodeNamePair>();
            CallingLogList = new ObservableCollection<CallingLogVM>();
            NewCallingLog = new CallingLogVM();
            CompanyCode = CPApplication.Current.CompanyCode;
        }
        public List<KeyValuePair<CallsEventsStatus?, string>> CallStatusList { get; set; }
        public ObservableCollection<CodeNamePair> RecordOrigionList { get; set; }
        public ObservableCollection<CodeNamePair> SubjectList { get; set; }
        public List<KeyValuePair<CustomerCallReason?, string>> CallReasonList { get; set; }

        public int? SysNo { get; set; }
        private int? _CustomerSysNo;

        public int? CustomerSysNo
        {
            get { return _CustomerSysNo; }
            set { base.SetValue("CustomerSysNo", ref _CustomerSysNo, value); }
        }

        private string _CustomerID;

        public string CustomerID
        {
            get { return _CustomerID; }
            set { base.SetValue("CustomerID", ref _CustomerID, value); }
        }
        private string _CustomerName;

        public string CustomerName
        {
            get { return _CustomerName; }
            set { base.SetValue("CustomerName", ref _CustomerName, value); }
        }

        private string _Phone;


        public string Phone
        {
            get { return _Phone; }
            set { base.SetValue("Phone", ref _Phone, value); }
        }
        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { base.SetValue("Email", ref _Email, value); }
        }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { base.SetValue("Address", ref _Address, value); }
        }
        private string  _OrderSysNo;

        public string  OrderSysNo
        {
            get { return _OrderSysNo; }
            set { base.SetValue("OrderSysNo", ref _OrderSysNo, value); }
        }
        private string _FromLinkSource;

        public string FromLinkSource
        {
            get { return _FromLinkSource; }
            set { base.SetValue("FromLinkSource", ref _FromLinkSource, value); }
        }
        private CallingLogVM _NewCallingLog;
        public CallingLogVM NewCallingLog
        {
            get { return _NewCallingLog; }
            set { base.SetValue("NewCallingLog", ref _NewCallingLog, value); }
        }

        public ObservableCollection<CallingLogVM> CallingLogList { get; set; }

        public string CompanyCode { get; set; }
    }
    public class CallingLogVM : ModelBase
    {
        private int? _CustomerCallingSysNo;

        public int? CustomerCallingSysNo
        {
            get { return _CustomerCallingSysNo; }
            set { base.SetValue("CustomerCallingSysNo", ref _CustomerCallingSysNo, value); }
        }
        private CallsEventsStatus? _Status;
        [Validate(ValidateType.Required)]
        public CallsEventsStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }
        /// <summary>
        /// 情况描述
        /// </summary>
        private string _Question;
        [Validate(ValidateType.Required)]
        public string Question
        {
            get { return _Question; }
            set { base.SetValue("Question", ref _Question, value); }
        }

        private CustomerCallReason? _CallReason;
        [Validate(ValidateType.Required)]
        public CustomerCallReason? CallReason
        {
            get { return _CallReason; }
            set { base.SetValue("CallReason", ref _CallReason, value); }
        }

        private string _RecordOrigion;
        [Validate(ValidateType.Required)]
        public string RecordOrigion
        {
            get { return _RecordOrigion; }
            set { base.SetValue("RecordOrigion", ref _RecordOrigion, value); }
        }

        private int? _ReasonCodeSysNo;

        public int? ReasonCodeSysNo
        {
            get { return _ReasonCodeSysNo; }
            set { base.SetValue("ReasonCodeSysNo", ref _ReasonCodeSysNo, value); }
        }

        private string _ReasonCodePath;

        public string ReasonCodePath
        {
            get { return _ReasonCodePath; }
            set { base.SetValue("ReasonCodePath", ref _ReasonCodePath, value); }
        }

        private string _ProcessUser;

        public string ProcessUser
        {
            get { return _ProcessUser; }
            set { base.SetValue("ProcessUser", ref _ProcessUser, value); }
        }

        private DateTime? _ProcessTime;

        public DateTime? ProcessTime
        {
            get { return _ProcessTime; }
            set { base.SetValue("ProcessTime", ref _ProcessTime, value); }
        }

    }
}
