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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.MKT.Models
{
    public class TopItemQueryVM : ModelBase
    {
        public TopItemQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID = "0";
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private int? _FrontPageSize = 24;

        public int? FrontPageSize
        {
            get { return _FrontPageSize; }
            set { base.SetValue("FrontPageSize", ref _FrontPageSize, value); }
        }

        public int? ProductSysNo
        {
            get;
            set;
        }
        private string _ProductID;

        public string ProductID
        {
            get { return _ProductID; }
            set { base.SetValue("ProductID", ref _ProductID, value); }
        }
        #region topitemconfig

        private bool _ISSendMailStore;

        public bool ISSendMailStore
        {
            get { return _ISSendMailStore; }
            set { base.SetValue("ISSendMailStore", ref _ISSendMailStore, value); }
        }

        private bool _ISShowTopStore;

        public bool ISShowTopStore
        {
            get { return _ISShowTopStore; }
            set { base.SetValue("ISShowTopStore", ref _ISShowTopStore, value); }
        }

        private bool _ExtendFlag;

        public bool ExtendFlag
        {
            get { return _ExtendFlag; }
            set { base.SetValue("ExtendFlag", ref _ExtendFlag, value); }
        }

        public string FrontPageUrlBase4SubCategory { get; set; }
        public string FrontPageUrlBase4Category { get; set; }
        #endregion

        public bool HasTopItemBatchMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_TopItem_BatchMaintain); }
        }
    }

    public class TopItemVM : ModelBase
    {
        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { base.SetValue("IsChecked", ref _IsChecked, value); }
        }


        private int _ProductSysNo;

        public int ProductSysNo
        {
            get { return _ProductSysNo; }
            set { base.SetValue("ProductSysNo", ref _ProductSysNo, value); }
        }

        private string _ProductID;

        public string ProductID
        {
            get { return _ProductID; }
            set { base.SetValue("ProductID", ref _ProductID, value); }
        }
        private string _ProductName;

        public string ProductName
        {
            get { return _ProductName; }
            set { base.SetValue("ProductName", ref _ProductName, value); }

        }
        private int _OnlineQty;

        public int OnlineQty
        {
            get { return _OnlineQty; }
            set { base.SetValue("OnlineQty", ref _OnlineQty, value); }
        }
        private decimal _CurrentPrice;

        public decimal CurrentPrice
        {
            get { return _CurrentPrice; }
            set { base.SetValue("CurrentPrice", ref _CurrentPrice, value); }
        }

        public string CurrentPriceDisplay
        {
            get { return "￥" + CurrentPrice.ToString("0.00"); }

        }

        private decimal _JDPrice;

        public decimal JDPrice
        {
            get { return _JDPrice; }
            set { base.SetValue("JDPrice", ref _JDPrice, value); }
        }

        public string JDPriceDisplay
        {
            get { return "￥" + JDPrice.ToString("0.00"); }

        }

        private string _PageNumber;

        public string PageNumber
        {
            get { return _PageNumber; }
            set { base.SetValue("PageNumber", ref _PageNumber, value); }
        }

        public string PageNumberStr
        {
            get { return string.Format("第{0}页", PageNumber); }
        }

        private string _PageUrl;

        public string PageUrl
        {
            get { return _PageUrl; }
            set { base.SetValue("PageUrl", ref _PageUrl, value); }
        }

        private bool _IsSetTop;

        public bool IsSetTop
        {
            get { return _IsSetTop; }
            set { base.SetValue("IsSetTop", ref _IsSetTop, value); }
        }

        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9]\d{0,2}$", ErrorMessage = "请输入1至999的整数！")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _EditUserName;

        public string EditUserName
        {
            get { return _EditUserName; }
            set { base.SetValue("EditUserName", ref _EditUserName, value); }
        }
        private DateTime? _EditDate;

        public DateTime? EditDate
        {
            get { return _EditDate; }
            set { base.SetValue("EditDate", ref _EditDate, value); }
        }

      

    }
}
