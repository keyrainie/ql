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
using ECCentral.BizEntity.Common;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
namespace ECCentral.Portal.UI.Customer.Models
{
    public class ShipTypeSMSQueryVM : ModelBase
    {
        public ShipTypeSMSQueryVM()
        {
            ShipTypeSMSStatusList = EnumConverter.GetKeyValuePairs<ShipTypeSMSStatus>(EnumConverter.EnumAppendItemType.All);
            SMSTypeList = new List<CodeNamePair>();
            ShippingTypeList = new List<ShippingType>();
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;

        }
        public List<KeyValuePair<ShipTypeSMSStatus?, string>> ShipTypeSMSStatusList { get; set; }


        public List<CodeNamePair> SMSTypeList { get; set; }

        private List<ShippingType> _ShippingTypeList;

        public List<ShippingType> ShippingTypeList
        {
            get { return _ShippingTypeList; }
            set { base.SetValue("ShippingTypeList", ref _ShippingTypeList, value); }
        }

    
        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private ShipTypeSMSStatus? m_Status;
        public ShipTypeSMSStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private int? _SMSType;
        public int? SMSType
        {
            get { return _SMSType; }
            set { this.SetValue("SMSType", ref _SMSType, value); }
        }

        private Int32? m_ShipTypeSysNo;
        public Int32? ShipTypeSysNo
        {
            get { return this.m_ShipTypeSysNo; }
            set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value); }
        }
   

    }
    public class ShipTypeSMSVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_ShipTypeSysNo;
        [Validate(ValidateType.Required)]
        public Int32? ShipTypeSysNo
        {
            get { return this.m_ShipTypeSysNo; }
            set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value); }
        }
        private string _ShipTypeName;
        public string ShipTypeName
        {
            get { return _ShipTypeName; }
            set { this.SetValue("ShipTypeName", ref _ShipTypeName, value); }
        }


        private string m_SMSTypeName;

        public string SMSTypeName
        {
            get { return this.m_SMSTypeName; }
            set { this.SetValue("SMSTypeName", ref m_SMSTypeName, value); }
        }

        private int? _SMSType;
        [Validate(ValidateType.Required)]
        public int? SMSType
        {
            get { return _SMSType; }
            set { this.SetValue("SMSType", ref _SMSType, value); }
        }

        private String m_SMSContent;
        [Validate(ValidateType.Required)]
        public String SMSContent
        {
            get { return this.m_SMSContent; }
            set { this.SetValue("SMSContent", ref m_SMSContent, value); }
        }

        private ShipTypeSMSStatus? m_Status;
        [Validate(ValidateType.Required)]
        public ShipTypeSMSStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private string _ChannelID;
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _ChannelID; }
            set { this.SetValue("ChannelID", ref _ChannelID, value); }
        }


    }
    public class ShipTypeSMSMaintainVM : ModelBase
    {
        public ShipTypeSMSMaintainVM()
        {
            ShipTypeSMSStatusList = EnumConverter.GetKeyValuePairs<ShipTypeSMSStatus>(EnumConverter.EnumAppendItemType.Select);
            SMSTypeList = new ObservableCollection<CodeNamePair>();
            ShippingTypeList = new ObservableCollection<ShippingType>();
            EntityVM = new ShipTypeSMSVM();
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_Select });
        }
        public List<KeyValuePair<ShipTypeSMSStatus?, string>> ShipTypeSMSStatusList { get; set; }

        public ObservableCollection<CodeNamePair> SMSTypeList { get; set; }

        private ObservableCollection<ShippingType> _ShippingTypeList;

        public ObservableCollection<ShippingType> ShippingTypeList
        {
            get { return _ShippingTypeList; }
            set { base.SetValue("ShippingTypeList", ref _ShippingTypeList, value); }
        }

        private ShipTypeSMSVM _EntityVM;

        public ShipTypeSMSVM EntityVM
        {
            get { return _EntityVM; }
            set { base.SetValue("EntityVM", ref _EntityVM, value); }
        }

        private string _ChannelID;
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _ChannelID; }
            set { this.SetValue("ChannelID", ref _ChannelID, value); }
        }

        public List<UIWebChannel> WebChannelList { get; set; }

    }

    public class ShipTypeSMSTemplateVM : ModelBase
    {
        private int? _SysNo;

        public int? SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }

        private string _Template;
        [Validate(ValidateType.Required)]
        public string Template
        {
            get { return _Template; }
            set { base.SetValue("Template", ref _Template, value); }
        }

    }
}
