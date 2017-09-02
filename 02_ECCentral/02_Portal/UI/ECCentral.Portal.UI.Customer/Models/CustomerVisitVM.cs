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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class VisitCustomerVM : ModelBase
    {
        public int? Sysno
        {
            get;
            set;
        }
        public string CustomerID
        {
            get;
            set;
        }
        public int? CustomerSysNo
        {
            get;
            set;
        }
        public bool? IsActive
        {
            get;
            set;
        }
        public DateTime LastBuyTime
        {
            get;
            set;
        }
        public DateTime LastActiveTime
        {
            get;
            set;
        }
        public VisitDealStatus LastCallStatus
        {
            get;
            set;
        }
        public int? ContactCount
        {
            get;
            set;
        }
        public decimal? OrderAmount
        {
            get;
            set;
        }
        public int? OrderCount
        {
            get;
            set;
        }
        public bool? IsRMA
        {
            get;
            set;
        }
        public YNStatusThree ConsumeDesire
        {
            get;
            set;
        }
        public int? Status
        {
            get;
            set;
        }
        public DateTime LastCallTime
        {
            get;
            set;
        }
        public decimal? SpiderRMAAmt
        {
            get;
            set;
        }
        public VisitDealStatus LastMaintenanceStatus
        {
            get;
            set;
        }
        public VisitCallResult LastCallReason
        {
            get;
            set;
        }


    }

    public class WaitingVisitCustomerVM : ModelBase
    {
        public int? Sysno
        {
            get;
            set;
        }
        public int? CustomerSysNo
        {
            get;
            set;
        }
        public int? Status
        {
            get;
            set;
        }
        public int? LockUserSysNo
        {
            get;
            set;
        }
        public string Phone
        {
            get;
            set;
        }
        public DateTime LastBuyDate
        {
            get;
            set;
        }
        public DateTime CreateDate
        {
            get;
            set;
        }
        public int? CreateUserSysNo
        {
            get;
            set;
        }
        public int? LastEditUserSysNo
        {
            get;
            set;
        }
        public DateTime LastEditDate
        {
            get;
            set;
        }
        public DateTime LastCallTime
        {
            get;
            set;
        }
        public int? VisitSysNo
        {
            get;
            set;
        }
        public DateTime RegisterTime
        {
            get;
            set;
        }
    }

    public class VisitLogVM : ModelBase
    {
        public int? Sysno
        {
            get;
            set;
        }
        public int? CustomerSysNo
        {
            get;
            set;
        }
        public string CustomerID
        {
            get;
            set;
        }
        public int? VisitSysNo
        {
            get;
            set;
        }
        public VisitDealStatus? DealStatus //CallStatus
        {
            get;
            set;
        }
        public VisitCallResult? CallResult //TelType
        {
            get;
            set;
        }
        public DateTime? RemindDate
        {
            get;
            set;
        }
        private string note;
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return note; }
            set
            {
                SetValue<string>("Note", ref note, value);
            }
        }
        public string qq;
        [Validate(ValidateType.Regex, @"^\d{5,12}$", ErrorMessageResourceName = "Validate_QQ", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        public string QQ
        {
            get { return qq; }
            set
            {

                SetValue<string>("QQ", ref qq, value);
            }
        }
        public string msn;
        [Validate(ValidateType.Email, ErrorMessageResourceName = "Validate_MSN", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        public string MSN
        {
            get { return msn; }
            set
            {

                SetValue<string>("MSN", ref msn, value);
            }
        }
        public YNStatusThree? ConsumeDesire
        {
            get;
            set;
        }
        public int? SoSysNo
        {
            get;
            set;
        }

        public string InUserAcct { get; set; }
        public int? InUserSysNo { get; set; }
        public DateTime? InDate { get; set; }

        public string EditUserAcct { get; set; }
        public int? EditUserSysNo { get; set; }
        public DateTime? EditDate { get; set; }
        public string CompanyCode { get; set; }

    }

    public class CustomerVisitDetailView : ModelBase
    {
        public CustomerMaster Customer { get; set; }
        public VisitCustomerVM VisitInfo { get; set; }
        public List<VisitLogVM> VisitLogs { get; set; }
        public List<VisitLogVM> MaintenanceLogs { get; set; }
    }

    public class CustomerVisitVM : ModelBase
    {
        public CustomerMaster Customer { get; set; }
        public VisitCustomerVM Visit { get; set; }
        public CustomerVisitVM()
        {
            Customer = new CustomerMaster();
            Visit = new VisitCustomerVM();
        }
    }

    public class CustomerVisitQueryVM : ModelBase
    {
        public CustomerVisitQueryVM()
        {
            PageInfo = new QueryFilter.Common.PagingInfo();
        }

        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        private VisitSeachType? seachType;
        public VisitSeachType? SeachType
        {
            get { return seachType; }
            set { SetValue<VisitSeachType?>("SeachType", ref seachType, value); }
        }

        private string customerSysNo;
        /// <summary>
        /// 顾客编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { SetValue<string>("CustomerSysNo", ref customerSysNo, value); }
        }

        private CustomerRank? customerRank;
        /// <summary>
        /// 顾客等级
        /// </summary>
        public CustomerRank? CustomerRank
        {
            get { return customerRank; }
            set { SetValue<CustomerRank?>("CustomerRank", ref customerRank, value); }
        }


        private string fromTotalAmount;
        /// <summary>
        /// 较小累积消费
        /// </summary>
        [Validate(ValidateType.Regex, @"^(\d{0,13})+(.\d+)?$", ErrorMessageResourceName = "Validate_Amount", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        public string FromTotalAmount
        {
            get { return fromTotalAmount; }
            set { SetValue<string>("FromTotalAmount", ref fromTotalAmount, value); }
        }

        private string toTotalAmount;
        /// <summary>
        /// 较大累积消费
        /// </summary>
        [Validate(ValidateType.Regex, @"^(\d{0,13})+(.\d+)?$", ErrorMessageResourceName = "Validate_Amount", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        public string ToTotalAmount
        {
            get { return toTotalAmount; }
            set { SetValue<string>("ToTotalAmount", ref toTotalAmount, value); }
        }
        private DateTime? fromLastSODate;

        /// <summary>
        /// 较小最后下单时间
        /// </summary>
        public DateTime? FromLastSODate
        {
            get { return fromLastSODate; }
            set { SetValue<DateTime?>("FromLastSODate", ref fromLastSODate, value); }
        }

        private DateTime? toLastSODate;
        /// <summary>
        /// 较大最后下单时间
        /// </summary>
        public DateTime? ToLastSODate
        {
            get { return toLastSODate; }
            set { SetValue<DateTime?>("ToLastSODate", ref toLastSODate, value); }
        }

        private bool? isVip;
        /// <summary>
        /// 是否是Vip
        /// </summary>
        public bool? IsVip
        {
            get { return isVip; }
            set { SetValue<bool?>("IsVip", ref isVip, value); }
        }
        private VisitDealStatus? dealStatus;
        /// <summary>
        /// 回访处理状态
        /// </summary>
        public VisitDealStatus? DealStatus
        {
            get { return dealStatus; }
            set { SetValue<VisitDealStatus?>("DealStatus", ref dealStatus, value); }
        }
        private List<VisitCallResult> callResult;
        /// <summary>
        /// 回访电话结果
        /// </summary>
        public List<VisitCallResult> CallResult
        {
            get { return callResult; }
            set { SetValue<List<VisitCallResult>>("CallResult", ref callResult, value); }
        }

        private YNStatusThree? consumeDesire;
        /// <summary>
        /// 购买意愿
        /// </summary>
        public YNStatusThree? ConsumeDesire
        {
            get { return consumeDesire; }
            set { SetValue<YNStatusThree?>("ConsumeDesire", ref consumeDesire, value); }
        }
        private YNStatus? isActivated;
        /// <summary>
        /// 是否被激活
        /// </summary>
        public YNStatus? IsActivated
        {
            get { return isActivated; }
            set { SetValue<YNStatus?>("IsActivated", ref isActivated, value); }
        }

        private YNStatus? isMaintain;
        /// <summary>
        /// 是否有维护
        /// </summary>
        public YNStatus? IsMaintain
        {
            get { return isMaintain; }
            set { SetValue<YNStatus?>("IsMaintain", ref isMaintain, value); }
        }

        private int? lastEditorSysNo;
        /// <summary>
        /// 维护者编号
        /// </summary>
        public int? LastEditorSysNo
        {
            get { return lastEditorSysNo; }
            set { SetValue<int?>("LastEditorSysNo", ref lastEditorSysNo, value); }
        }
        private DateTime? fromVisitDate;
        /// <summary>
        /// 较早回访时间
        /// </summary>
        public DateTime? FromVisitDate
        {
            get { return fromVisitDate; }
            set { SetValue<DateTime?>("FromVisitDate", ref fromVisitDate, value); }
        }
        private DateTime? toVisitDate;
        /// <summary>
        /// 较近回访时间
        /// </summary>
        public DateTime? ToVisitDate
        {
            get { return toVisitDate; }
            set { SetValue<DateTime?>("ToVisitDate", ref toVisitDate, value); }
        }
        private DateTime? fromOrderDate;
        /// <summary>
        /// 较早下单时间
        /// </summary>
        public DateTime? FromOrderDate
        {
            get { return fromOrderDate; }
            set { SetValue<DateTime?>("FromOrderDate", ref fromOrderDate, value); }
        }
        private DateTime? toOrderDate;
        /// <summary>
        /// 较近下单时间
        /// </summary>
        public DateTime? ToOrderDate
        {
            get { return toOrderDate; }
            set { SetValue<DateTime?>("ToOrderDate", ref toOrderDate, value); }
        }
        private string _CustomerID;

        public string CustomerID
        {
            get { return _CustomerID; }
            set { SetValue("CustomerID", ref _CustomerID, value); }
        }
        private string _CustomerName;

        public string CustomerName
        {
            get { return _CustomerName; }
            set { SetValue("CustomerName", ref _CustomerName, value); }
        }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { SetValue("Address", ref _Address, value); }
        }
        private string _Phone;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex,@"^\d{11,}$",ErrorMessage="号码至少为11位")]
        public string Phone
        {
            get { return _Phone; }
            set { SetValue("Phone", ref _Phone, value); }

        }
        private string _ShipType;
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "Validate_SoDate", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        [Validate(ValidateType.Regex, @"^\d{0,4}$", ErrorMessageResourceName = "Validate_SoDate", ErrorMessageResourceType = typeof(ResCustomerVisit))]
        public string ShipType
        {
            get { return _ShipType; }
            set { SetValue("ShipType", ref _ShipType, value); }

        }
        private int _ShipTypeCondition;

        public int ShipTypeCondition
        {
            get { return _ShipTypeCondition; }
            set { SetValue("ShipTypeCondition", ref _ShipTypeCondition, value); }

        }
        private string _CompanyCode;

        public string CompanyCode
        {
            get { return _CompanyCode; }
            set { SetValue("CompanyCode", ref _CompanyCode, value); }

        }
        private string _Email;
        [Validate(ValidateType.Email)]
        public string Email
        {
            get { return _Email; }
            set { SetValue("Email", ref _Email, value); }

        }
        private DateTime? _SpiderOrderDateFrom;

        public DateTime? SpiderOrderDateFrom
        {
            get { return _SpiderOrderDateFrom; }
            set { SetValue("SpiderOrderDateFrom", ref _SpiderOrderDateFrom, value); }

        }
        private DateTime? _SpiderOrderDateTo;

        public DateTime? SpiderOrderDateTo
        {
            get { return _SpiderOrderDateTo; }
            set { SetValue("SpiderOrderDateTo", ref _SpiderOrderDateTo, value); }

        }
        private string _CustomerSysNos;

        public string CustomerSysNos
        {
            get { return _CustomerSysNos; }
            set { SetValue("CustomerSysNos", ref _CustomerSysNos, value); }

        }
        private bool _IsSpiderSearch;

        public bool IsSpiderSearch
        {
            get { return _IsSpiderSearch; }
            set { SetValue("IsSpiderSearch", ref _IsSpiderSearch, value); }
        }
    }

    public class CustomerVisitView : ModelBase
    {
        public int TotalCount { get; set; }
        public List<CustomerVisitVM> VisitList
        {
            get;
            set;
        }
        public CustomerVisitQueryVM QueryInfo
        {
            get;
            set;
        }
        public CustomerVisitView()
        {
            VisitList = new List<CustomerVisitVM>();
        }
    }

    public class CustomerVisitMaintainView : ModelBase
    {
        public CustomerMaster Customer { get; set; }
        public VisitLogVM Log
        {
            get;
            set;
        }
        public List<VisitLogVM> LogList { get; set; }
        public CustomerVisitMaintainView()
        {
            LogList = new List<VisitLogVM>();
        }
    }

    public class VisitOrderVM : ModelBase
    {
        public int? SysNo
        {
            get;
            set;
        }
        public int CustomerSysNo
        {
            get;
            set;
        }
        public int VisitSysNo
        {
            get;
            set;
        }
        public int SoSysNo
        {
            get;
            set;
        }
        public string RMASysNo
        {
            get;
            set;
        }
        public int CreateUserSysNo
        {
            get;
            set;
        }
        public int LastEditUserSysNo
        {
            get;
            set;
        }
        public string LanguageCode
        {
            get;
            set;
        }
        public string StoreCompanyCode
        {
            get;
            set;
        }
    }
}
