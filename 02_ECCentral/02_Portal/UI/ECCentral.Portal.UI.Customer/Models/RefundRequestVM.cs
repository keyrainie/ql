using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
namespace ECCentral.Portal.UI.Customer.Models
{
    public class RefundRequestVM : ModelBase
    {
        public RefundRequestVM()
        {
            RefundTypeList = new ObservableCollection<CodeNamePair>();
            RequestTypeList = EnumConverter.GetKeyValuePairs<RefundRequestType>(EnumConverter.EnumAppendItemType.All);
            RefundStatusList = EnumConverter.GetKeyValuePairs<RefundRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();

            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }
        public ObservableCollection<CodeNamePair> RefundTypeList { get; set; }
        public List<KeyValuePair<RefundRequestType?, string>> RequestTypeList { get; set; }
        public List<KeyValuePair<RefundRequestStatus?, string>> RefundStatusList { get; set; }


        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private DateTime? _CreateFrom;

        public DateTime? CreateFrom
        {
            get { return _CreateFrom; }
            set { base.SetValue("CreateFrom", ref _CreateFrom, value); }
        }
        private DateTime? _CreateTo;

        public DateTime? CreateTo
        {
            get { return _CreateTo; }
            set { base.SetValue("CreateTo", ref _CreateTo, value); }
        }
        private DateTime? _EditFrom;

        public DateTime? EditFrom
        {
            get { return _EditFrom; }
            set { base.SetValue("EditFrom", ref _EditFrom, value); }

        }
        private DateTime? _EditTo;

        public DateTime? EditTo
        {
            get { return _EditTo; }
            set { base.SetValue("EditTo", ref _EditTo, value); }
        }
        private string _CustomerId;

        public string CustomerId
        {
            get { return _CustomerId; }
            set { base.SetValue("CustomerId", ref _CustomerId, value); }
        }
        private int? _SysNo;

        public int? SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }
        private string _EditUserName;

        public string EditUserName
        {
            get { return _EditUserName; }
            set { base.SetValue("EditUserName", ref _EditUserName, value); }
        }
        private int? _SOSysNo;

        public int? SOSysNo
        {
            get { return _SOSysNo; }
            set { base.SetValue("SOSysNo", ref _SOSysNo, value); }
        }
        private RefundRequestType? _RequestType;

        public RefundRequestType? RequestType
        {
            get { return _RequestType; }
            set { base.SetValue("RequestType", ref _RequestType, value); }
        }
        private string  _RefundType;

        public string RefundType
        {
            get { return _RefundType; }
            set { base.SetValue("RefundType", ref _RefundType, value); }
        }
        private RefundRequestStatus? _Status;

        public RefundRequestStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }
    }
}
