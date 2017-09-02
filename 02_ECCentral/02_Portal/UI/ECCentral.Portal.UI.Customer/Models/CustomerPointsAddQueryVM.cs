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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerPointsAddQueryVM : ModelBase
    {
        public CustomerPointsAddQueryVM()
        {

            statusList = EnumConverter.GetKeyValuePairs<CustomerPointsAddRequestStatus>(EnumConverter.EnumAppendItemType.All);
            sysAccountList = new ObservableCollection<CodeNamePair>();
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;

            OwnByDepartmentVisibility = Visibility.Collapsed;
            OwnByReasonVisibility = Visibility.Collapsed;

        }
        public List<KeyValuePair<CustomerPointsAddRequestStatus?, string>> statusList { get; set; }

        public ObservableCollection<CodeNamePair> sysAccountList { get; set; }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private DateTime? createDateFrom;
        private DateTime? createDateTo;
        private int? channelSysNo;
        private string customerID;
        private int customerSysNo;
        private string soSysNo;
        private string ownByDepartment;
        private string neweggAccount;
        private CustomerPointsAddRequestStatus? status;

        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set
            {
                base.SetValue("CreateDateFrom", ref createDateFrom, value);
            }
        }
        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set
            {
                base.SetValue("CreateDateTo", ref createDateTo, value);
            }
        }
        public int? ChannelSysNo
        {
            get { return channelSysNo; }
            set
            {
                base.SetValue("ChannelSysNo", ref channelSysNo, value);
            }

        }
        public string CustomerID
        {
            get { return customerID; }
            set
            {
                base.SetValue("CustomerID", ref customerID, value);
            }
        }
        public int CustomerSysNo
        {
            get { return customerSysNo; }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return soSysNo; }
            set
            {
                base.SetValue("SOSysNo", ref soSysNo, value);
            }
        }
        public string OwnByDepartment
        {
            get { return ownByDepartment; }
            set
            {
                base.SetValue("OwnByDepartment", ref ownByDepartment, value);
            }
        }

        /// <summary>
        /// 责任部门显示与否
        /// </summary>
        private Visibility _OwnByDepartmentVisibility;

        public Visibility OwnByDepartmentVisibility
        {
            get { return _OwnByDepartmentVisibility; }
            set
            {
                base.SetValue("OwnByDepartmentVisibility", ref _OwnByDepartmentVisibility, value);
            }
        }

        /// <summary>
        /// 原因显示与否
        /// </summary>
        private Visibility _OwnByReasonVisibility;

        public Visibility OwnByReasonVisibility
        {
            get { return _OwnByReasonVisibility; }
            set
            {
                base.SetValue("OwnByReasonVisibility", ref _OwnByReasonVisibility, value);
            }
        }

        public string NeweggAccount
        {
            get { return neweggAccount; }
            set
            {
                base.SetValue("NeweggAccount", ref neweggAccount, value);
            }
        }
        public CustomerPointsAddRequestStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        public bool HasExportRight { get; set; }

        private string ownByReason;

        public string OwnByReason
        {
            get { return ownByReason; }
            set { ownByReason = value; }
        }

        public string NeweggAccountDesc { get; set; }
        public string OwnByDepartmentDesc { get; set; }
        public string OwnByReasonDesc { get; set; }
    }

    public class CustomerPointsAddItemQueryVM : ModelBase
    {

        private int? sysNo;
        private int pointAddRequestSysNo;
        private int? soSysNo;
        private int productSysNo;
        private string productID;

        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }

        }
        public int PointAddRequestSysNo
        {
            get { return pointAddRequestSysNo; }
            set
            {
                base.SetValue("PointAddRequestSysNo", ref pointAddRequestSysNo, value);
            }
        }
        public int? SOSysNo
        {
            get { return soSysNo; }
            set
            {
                base.SetValue("SOSysNo", ref soSysNo, value);
            }
        }
        public int ProductSysNo
        {
            get { return productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref productSysNo, value);
            }
        }
        public string ProductID
        {
            get { return productID; }
            set
            {
                base.SetValue("ProductID", ref productID, value);
            }
        }
    }
}
