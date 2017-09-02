using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerPrePayVM : ModelBase
    {
        public CustomerPrePayVM()
        {
            PrepayTypeList = new ObservableCollection<CodeNamePair>();
            PrepayStatusList = EnumConverter.GetKeyValuePairs<PrepayStatus>(EnumConverter.EnumAppendItemType.All);
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;

        }
        public ObservableCollection<CodeNamePair> PrepayTypeList { get; set; }
        public List<KeyValuePair<PrepayStatus?, string>> PrepayStatusList { get; set; }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }


        private DateTime? _CreateDateFrom;

        public DateTime? CreateDateFrom
        {
            get { return _CreateDateFrom; }
            set { base.SetValue("CreateDateFrom", ref _CreateDateFrom, value); }
        }
        private DateTime? _CreateDateTo;

        public DateTime? CreateDateTo
        {
            get { return _CreateDateTo; }
            set { base.SetValue("CreateDateTo", ref _CreateDateTo, value); }
        }
        private int? _SysNo;

        public int? OrderSysNo
        {
            get { return _SysNo; }
            set { base.SetValue("OrderSysNo", ref _SysNo, value); }

        }
        private int? _PrePayType;

        public int? PrePayType
        {
            get { return _PrePayType; }
            set { base.SetValue("PrePayType", ref _PrePayType, value); }

        }
        private string _CustomerSysNo;

        public string CustomerSysNo
        {
            get { return _CustomerSysNo; }
            set { base.SetValue("CustomerSysNo", ref _CustomerSysNo, value); }
        }
        private PrepayStatus? _Status;

        public PrepayStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }

    }
}
