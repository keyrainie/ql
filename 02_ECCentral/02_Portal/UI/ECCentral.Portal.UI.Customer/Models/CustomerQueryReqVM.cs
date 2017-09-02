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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerQueryReqVM : ModelBase
    {
        private string customerSysNo;
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }

        private string customerID;
        public string CustomerID
        {
            get
            {
                return customerID;
            }
            set
            {
                base.SetValue("CustomerID", ref customerID, value);
            }
        }

        private string customerName;
        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                base.SetValue("CustomerName", ref customerName, value);
            }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        private string identityCard;
        public string IdentityCard
        {
            get
            {
                return identityCard;
            }
            set
            {
                base.SetValue("IdentityCard", ref identityCard, value);
            }
        }



        private string phone;
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                base.SetValue("Phone", ref phone, value);
            }
        }

        private string address;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                base.SetValue("Address", ref address, value);
            }
        }

        private CustomerStatus? status;
        public CustomerStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        private int? channelSysNo;
        public int? ChannelSysNo
        {
            get
            {
                return channelSysNo;
            }
            set
            {
                base.SetValue("ChannelSysNo", ref channelSysNo, value);
            }
        }



        private DateTime? registerTimeFrom;
        public DateTime? RegisterTimeFrom
        {
            get
            {
                return registerTimeFrom;
            }
            set
            {
                base.SetValue("RegisterTimeFrom", ref registerTimeFrom, value);
            }
        }

        private DateTime? registerTimeTo;
        public DateTime? RegisterTimeTo
        {
            get
            {
                return registerTimeTo;
            }
            set
            {
                base.SetValue("RegisterTimeTo", ref registerTimeTo, value);
            }
        }

        private bool isMoreQueryBuilderCheck;
        public bool IsMoreQueryBuilderCheck
        {
            get
            {
                return isMoreQueryBuilderCheck;
            }
            set
            {
                base.SetValue("IsMoreQueryBuilderCheck", ref isMoreQueryBuilderCheck, value);
            }
        }

        private string fromLinkSource;
        public string FromLinkSource
        {
            get
            {
                return fromLinkSource;
            }
            set
            {
                base.SetValue("FromLinkSource", ref fromLinkSource, value);
            }
        }

        private string recommendedByCustomerID;
        public string RecommendedByCustomerID
        {
            get
            {
                return recommendedByCustomerID;
            }
            set
            {
                base.SetValue("RecommendedByCustomerID", ref recommendedByCustomerID, value);
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                base.SetValue("Email", ref email, value);
            }
        }

        private CustomerVipOnly? isVip;
        public CustomerVipOnly? IsVip
        {
            get
            {
                return isVip;
            }
            set
            {
                base.SetValue("IsVip", ref isVip, value);
            }
        }

        private bool isBuyCountCheck;
        public bool IsBuyCountCheck
        {
            get
            {
                return isBuyCountCheck;
            }
            set
            {
                base.SetValue("IsBuyCountCheck", ref isBuyCountCheck, value);
            }
        }
        public int? IsBuyCountRadio
        {
            get
            {
                return IsBuyCountEndpointValue ? 0 : 1;
            }
        }

        private OperationSignType? operationSign;
        public OperationSignType? OperationSign
        {
            get
            {
                return operationSign;
            }
            set
            {
                base.SetValue("OperationSign", ref operationSign, value);
            }
        }

        private string buyCountValue;
        [Validate(ValidateType.Interger)]
        public string BuyCountValue
        {
            get
            {
                return buyCountValue;
            }
            set
            {
                base.SetValue("BuyCountValue", ref buyCountValue, value);
            }
        }

        private string buyCountBeginPoint;
        [Validate(ValidateType.Interger)]
        public string BuyCountBeginPoint
        {
            get
            {
                return buyCountBeginPoint;
            }
            set
            {
                base.SetValue("BuyCountBeginPoint", ref buyCountBeginPoint, value);
            }
        }

        private bool isBuyCountEndpointValue;
        public bool IsBuyCountEndpointValue
        {
            get
            {
                return isBuyCountEndpointValue;
            }
            set
            {
                base.SetValue("IsBuyCountEndpointValue", ref isBuyCountEndpointValue, value);
            }
        }

        private bool isBuyCountAreaValue;
        public bool IsBuyCountAreaValue
        {
            get
            {
                return isBuyCountAreaValue;
            }
            set
            {
                base.SetValue("IsBuyCountAreaValue", ref isBuyCountAreaValue, value);
            }
        }

        private string buyCountEndPoint;
        [Validate(ValidateType.Interger)]
        public string BuyCountEndPoint
        {
            get
            {
                return buyCountEndPoint;
            }
            set
            {
                base.SetValue("BuyCountEndPoint", ref buyCountEndPoint, value);
            }
        }

        private YNStatus? isEmailConfirmed;
        public YNStatus? IsEmailConfirmed
        {
            get
            {
                return isEmailConfirmed;
            }
            set
            {
                base.SetValue("IsEmailConfirmed", ref isEmailConfirmed, value);
            }
        }

        private YNStatus? isPhoneConfirmed;
        public YNStatus? IsPhoneConfirmed
        {
            get
            {
                return isPhoneConfirmed;
            }
            set
            {
                base.SetValue("IsPhoneConfirmed", ref isPhoneConfirmed, value);
            }
        }

        private AvtarShowStatus? avtarImageStatus;
        public AvtarShowStatus? AvtarImageStatus
        {
            get
            {
                return avtarImageStatus;
            }
            set
            {
                base.SetValue("AvtarImageStatus", ref avtarImageStatus, value);
            }
        }

        private CustomerType? customersType;
        public CustomerType? CustomersType
        {
            get
            {
                return customersType;
            }
            set
            {
                base.SetValue("CustomersType", ref customersType, value);
            }
        }

        public List<KeyValuePair<CustomerStatus?, string>> UserStatusList { get { return EnumConverter.GetKeyValuePairs<CustomerStatus>(EnumConverter.EnumAppendItemType.All); } }
        public List<KeyValuePair<YNStatus?, string>> YNList { get { return EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All); } }
        public List<KeyValuePair<AvtarShowStatus?, string>> AvtarStatusList { get { return EnumConverter.GetKeyValuePairs<AvtarShowStatus>(EnumConverter.EnumAppendItemType.All); } }
        public List<KeyValuePair<CustomerType?, string>> CustomerTypeList { get { return EnumConverter.GetKeyValuePairs<CustomerType>(EnumConverter.EnumAppendItemType.All); } }
        public List<KeyValuePair<OperationSignType?, string>> OperationSignTypeList { get { return EnumConverter.GetKeyValuePairs<OperationSignType>(); } }
        public List<KeyValuePair<CustomerVipOnly?, string>> CustomerVipOnlyList { get { return EnumConverter.GetKeyValuePairs<CustomerVipOnly>(EnumConverter.EnumAppendItemType.All); } }


        public string CompanyCode { get { return CPApplication.Current.CompanyCode; } }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        public string AvtarImgBasePath { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        private string vipCardNo;
        public string VipCardNo
        {
            get
            {
                return vipCardNo;
            }
            set
            {
                base.SetValue("VipCardNo", ref vipCardNo, value);
            }
        }

    }
}
