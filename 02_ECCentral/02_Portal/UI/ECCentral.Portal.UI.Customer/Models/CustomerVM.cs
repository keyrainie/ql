using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;


namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerVM : ModelBase
    {
        public CustomerVM()
        {
            this.BasicInfo = new CustomerBasicVM();
            this.ScoreInfo = new ScoreVM();
            this.ExperienceInfo = new ExperienceVM();
            this.AccountPeriodInfo = new CustomerAccountPeriodVM();
            this.AgentInfo = new AgentInfoVM();
            this.ShippingAddress = new ShippingAddressVM();
            this.ShippingAddressList = new ObservableCollection<ShippingAddressVM>();
            this.ValueAddedTaxInfoList = new ObservableCollection<ValueAddedTaxInfoVM>();
        }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        public CustomerBasicVM BasicInfo
        {
            get;
            set;
        }

        public ScoreVM ScoreInfo
        {
            get;
            set;
        }

        public ExperienceVM ExperienceInfo
        {
            get;
            set;
        }

        public AgentInfoVM AgentInfo
        {
            get;
            set;
        }

        public ShippingAddressVM ShippingAddress { get; set; }

        public CustomerAccountPeriodVM AccountPeriodInfo { get; set; }

        public ObservableCollection<ShippingAddressVM> ShippingAddressList
        {
            get;
            set;
        }

        public ObservableCollection<ValueAddedTaxInfoVM> ValueAddedTaxInfoList
        {
            get;
            set;
        }


        #region UI扩展信息

        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                SetValue("IsEdit", ref isEdit, value);
                this.BasicInfo.IsEdit = value;
                this.ScoreInfo.IsEdit = value;
                this.ExperienceInfo.IsEdit = value;
                this.AgentInfo.IsEdit = value;
            }
        }
        private bool hasAgent;

        public bool HasAgent
        {
            get { return AgentInfo != null && !string.IsNullOrEmpty(AgentInfo.CertificateNo); }
            set { SetValue("HasAgent", ref hasAgent, value); }
        }

        #endregion UI扩展信息
    }

    public class CustomerBasicVM : ModelBase
    {
        public CustomerBasicVM()
        {
            this.UserStatusList = EnumConverter.GetKeyValuePairs<CustomerStatus>();
            this.Status = CustomerStatus.Valid;

            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.None);
            this.IsBadCustomer = false;

            this.AvtarStatusList = EnumConverter.GetKeyValuePairs<AvtarShowStatus>();
            this.AvtarImageStatus = AvtarShowStatus.Show;

            this.CustomerTypes = EnumConverter.GetKeyValuePairs<CustomerType>();
            this.CustomersType = (int)CustomerType.Personal;

            this.Genders = EnumConverter.GetKeyValuePairs<Gender>();
            this.Gender = BizEntity.Customer.Gender.Male;            
          
            this.CompanyCode = CPApplication.Current.CompanyCode;

            this.Pwd = "1234";
        }

        public string CompanyCode { get; set; }

        public List<WebChannelVM> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private int? channelSysNo;
        public int? ChannelSysNo
        {
            get { return channelSysNo; }
            set { base.SetValue("ChannelSysNo", ref channelSysNo, value); }
        }

        private int? customerSysNo;
        public int? CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }

        public string Pwd { get; set; }

        public bool? IsSubscribe { get; set; }

        private string customerID;
        [Validate(ValidateType.Required)]
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

        private string identityCard;
        [Validate(ValidateType.Regex, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", ErrorMessage = "请输入正确的身份证号码")]
        public string IdentityCard
        {
            get
            {
                return identityCard;
            }
            set
            {
                SetValue("IdentityCard", ref identityCard, value);
            }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        private string customerName;
        //[Validate(ValidateType.Required)]
        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                SetValue("CustomerName", ref customerName, value);
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
                SetValue("FromLinkSource", ref fromLinkSource, value);
            }
        }

        private CustomerStatus status;
        public CustomerStatus Status
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

        private bool? isBadCustomer;
        public bool? IsBadCustomer
        {
            get
            {
                return isBadCustomer;
            }
            set
            {
                SetValue("IsBadCustomer", ref isBadCustomer, value);
            }
        }

        /// <summary>
        /// 恶意用户备注信息
        /// </summary>
        private string badCustomerMemo;
        [Validate(ValidateType.Required)]
        public string BadCustomerMemo
        {
            get
            {
                return badCustomerMemo;
            }
            set
            {
                SetValue("BadCustomerMemo", ref badCustomerMemo, value);
            }
        }

        private bool? _IsEmailConfirmed;

        public bool? IsEmailConfirmed
        {
            get { return _IsEmailConfirmed; }
            set { SetValue("IsEmailConfirmed", ref _IsEmailConfirmed, value); }
        }

        private bool? _CellPhoneConfirmed;

        public bool? CellPhoneConfirmed
        {
            get { return _CellPhoneConfirmed; }
            set { SetValue("CellPhoneConfirmed", ref _CellPhoneConfirmed, value); }
        }


        private CustomerType customersType;
        public CustomerType CustomersType
        {
            get
            {
                return customersType;
            }
            set
            {
                SetValue("CustomersType", ref customersType, value);
            }
        }

        private int companyCustomer;
        public int CompanyCustomer
        {
            get
            {
                return companyCustomer;
            }
            set
            {
                SetValue("CompanyCustomer", ref companyCustomer, value);
            }
        }

        private string _AvtarImage;

        public string AvtarImage
        {
            get { return _AvtarImage; }
            set
            {
                SetValue("AvtarImage", ref _AvtarImage, value);
                SetAvtarImage();
            }
        }

        private string _AvtarImageBasePath;

        public string AvtarImageBasePath
        {
            get { return _AvtarImageBasePath; }
            set
            {
                SetValue("AvtarImageBasePath", ref _AvtarImageBasePath, value);
                SetAvtarImage();
            }
        }

        private AvtarShowStatus avtarImageStatus;
        public AvtarShowStatus AvtarImageStatus
        {
            get
            {
                return avtarImageStatus;
            }
            set
            {
                SetValue("AvtarImageStatus", ref avtarImageStatus, value);
            }
        }


        private CustomerRank? _Rank;

        public CustomerRank? Rank
        {
            get { return _Rank; }
            set
            {
                SetValue("Rank", ref _Rank, value);
                SetAvtarImage();
            }
        }
        #region Detail
        private string email;
        [Validate(ValidateType.Email)]
        [Validate(ValidateType.Required)]
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                SetValue("Email", ref email, value);
            }
        }

        private string phone;
        //[Validate(ValidateType.Phone)]
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                SetValue("Phone", ref phone, value);
            }
        }

        private string dwellZip;
        [Validate(ValidateType.ZIP)]
        //[Validate(ValidateType.Required)]
        public string DwellZip
        {
            get
            {
                return dwellZip;
            }
            set
            {
                SetValue("DwellZip", ref dwellZip, value);
            }
        }

        private string cellPhone;
        [Validate(ValidateType.Regex, @"^[0]?[1][3|5|8][0-9]{9}$")]
        public string CellPhone
        {
            get
            {
                return cellPhone;
            }
            set
            {
                SetValue("CellPhone", ref cellPhone, value);
            }
        }

        private Gender gender;
        public Gender Gender
        {
            get
            {
                return gender;
            }
            set
            {
                SetValue("Gender", ref gender, value);
            }
        }

        private DateTime? birthday;
        public DateTime? Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                SetValue("Birthday", ref birthday, value);
            }
        }

        private string fax;
        [Validate(ValidateType.Phone)]
        public string Fax
        {
            get
            {
                return fax;
            }
            set
            {
                SetValue("Fax", ref fax, value);
            }
        }

        private DateTime? _RegisterTime;

        public DateTime? RegisterTime
        {
            get { return _RegisterTime; }
            set { SetValue("RegisterTime", ref _RegisterTime, value); }
        }


        private string _RegisterIPAddress;

        public string RegisterIPAddress
        {
            get { return _RegisterIPAddress; }
            set { SetValue("RegisterIPAddress", ref _RegisterIPAddress, value); }
        }


        public DateTime? LastLoginDate
        {
            get;
            set;
        }

        public string RecommendedByCustomerID
        {
            get;
            set;
        }

        public int? RecommendedByCustomerSysNo { get; set; }

        private string dwellAreaSysNo;
        //[Validate(ValidateType.Required)]
        public string DwellAreaSysNo
        {
            get
            {
                return dwellAreaSysNo;
            }
            set
            {
                SetValue("DwellAreaSysNo", ref dwellAreaSysNo, value);
            }
        }

        private string dwellAddress;
        //[Validate(ValidateType.Required)]
        public string DwellAddress
        {
            get
            {
                return dwellAddress;
            }
            set
            {
                SetValue("DwellAddress", ref dwellAddress, value);
            }
        }

        #endregion

        #region UI 扩展信息
        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                SetValue("IsEdit", ref isEdit, value);
                if (!value)
                {
                    BadCustomerMemoVisible = Visibility.Collapsed;
                }
            }
        }

        public bool? OriginalIsBadCustomer { get; set; }

        public List<KeyValuePair<CustomerStatus?, string>> CompanyTypeList { get; set; }
        ////public List<KeyValuePair<CompanyType?, string>> CompanyTypeList
        ////{
        ////    get;
        ////    set;
        ////}
        public List<KeyValuePair<CustomerStatus?, string>> UserStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<Boolean?, string>> YNList
        {
            get;
            set;
        }
        public List<KeyValuePair<AvtarShowStatus?, string>> AvtarStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<CustomerType?, string>> CustomerTypes
        {
            get;
            set;
        }
        public List<KeyValuePair<Gender?, string>> Genders
        {
            get;
            set;
        }

        public List<CodeNamePair> CompanyCustomers
        {
            get;
            set;
        }

        /// <summary>
        /// 控制是否显示恶意用户的备注和确认按钮
        /// </summary>
        private Visibility badCustomerMemoVisible = Visibility.Collapsed;
        public Visibility BadCustomerMemoVisible
        {
            get
            {
                return badCustomerMemoVisible;
            }
            set
            {
                SetValue("BadCustomerMemoVisible", ref badCustomerMemoVisible, value);
            }
        }

        /// <summary>
        /// 查看推荐用户信息按钮是否可用
        /// </summary>
        public bool CanViewRecommandedUser
        {
            get
            {
                return this.IsEdit && !string.IsNullOrEmpty(this.RecommendedByCustomerID);
            }
        }
        private string _AvtarImage68Path;
        public string AvtarImage68Path
        {
            get { return _AvtarImage68Path; }
            set
            {
                SetValue("AvtarImage68Path", ref _AvtarImage68Path, value);
            }
        }

        private string _AvtarImage48Path;

        public string AvtarImage48Path
        {
            get { return _AvtarImage48Path; }
            set
            {
                SetValue("AvtarImage48Path", ref _AvtarImage48Path, value);
            }
        }
        private void SetAvtarImage()
        {
            if (!string.IsNullOrEmpty(AvtarImage))
            {
                AvtarImage48Path = string.Format("{0}{1}", AvtarImageBasePath, AvtarImage);
                AvtarImage68Path = string.Format("{0}{1}", AvtarImageBasePath, AvtarImage);
            }
            else
            {
                AvtarImage48Path = string.Format("/Images/Customer/CustomerRankImage/P48Rank{0}.jpg", Rank.HasValue ? (int)Rank.Value : 1);
                AvtarImage68Path = string.Format("/Images/Customer/CustomerRankImage/P68Rank{0}.jpg", Rank.HasValue ? (int)Rank.Value : 1);
            }
        }
        #endregion
    }

    public class ScoreVM : ModelBase
    {
        public ScoreVM()
        {
            this.VIPRanks = EnumConverter.GetKeyValuePairs<VIPRank>();
            this.VIPRank = BizEntity.Customer.VIPRank.NormalAuto;
            this.Rank = CustomerRank.Ferrum;
            this.TotalScore = 0;
            this.ValidScore = 0;
            this.ValidPrepayAmt = 0;
        }

        public int? CustomerSysNo
        {
            get;
            set;
        }

        public string CustomerName { get; set; }

        public int? TotalScore
        {
            get;
            set;
        }

        public int? ValidScore
        {
            get;
            set;
        }

        public CustomerRank? Rank
        {
            get;
            set;
        }

        public AuctionRank? AuctionRank
        {
            get;
            set;
        }

        private string cardNo;
        public string CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                SetValue("CardNo", ref cardNo, value);
            }
        }

        private VIPRank vipRank;
        public VIPRank VIPRank
        {
            get
            {
                return vipRank;
            }
            set
            {
                SetValue("VIPRank", ref vipRank, value);
            }
        }

        private string note;
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                SetValue("Note", ref note, value);
            }
        }

        public string ContributeRank
        {
            get;
            set;
        }

        private decimal? _ValidPrepayAmt;
        public decimal? ValidPrepayAmt
        {
            get { return _ValidPrepayAmt; }
            set
            {
                base.SetValue("ValidPrepayAmt", ref _ValidPrepayAmt, value);
                BtnPrepayToBankEnable = value > 0;
            }
        }

        public DateTime? PointExpiringDate
        {
            get;
            set;
        }

        public int? PromotionRankSign { get; set; }

        public List<KeyValuePair<VIPRank?, string>> VIPRanks
        {
            get;
            set;
        }

        #region UI扩展信息
        private bool _BtnPrepayToBankEnable = true;
        public bool BtnPrepayToBankEnable
        {
            get
            {
                return _BtnPrepayToBankEnable;
            }
            set
            {
                SetValue("BtnPrepayToBankEnable", ref _BtnPrepayToBankEnable, value);
            }
        }
        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                SetValue("IsEdit", ref isEdit, value);
            }
        }

        #endregion
    }

    public class ExperienceVM : ModelBase
    {
        public int? CustomerSysNo
        {
            get;
            set;
        }

        private int? totalSOMoney;
        public int? TotalSOMoney
        {
            get
            {
                return totalSOMoney;
            }
            set
            {
                SetValue("TotalSOMoney", ref totalSOMoney, value);
            }
        }

        public ExperienceLogType? Type { get; set; }

        private string amount;
        /// <summary>
        /// 说明：定义成string类型是为了方便UI验证   
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Amount
        {
            get
            {
                return amount;
            }
            set
            {
                SetValue("Amount", ref amount, value);
            }
        }

        private string memo;
        [Validate(ValidateType.Required)]
        public string Memo
        {
            get
            {
                return memo;
            }
            set
            {
                SetValue("Memo", ref memo, value);
            }
        }

        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                SetValue("IsEdit", ref isEdit, value);
            }
        }
    }

    public class AgentInfoVM : ModelBase
    {
        public AgentInfoVM()
        {
            this.AgentTypes = EnumConverter.GetKeyValuePairs<AgentType>();
            this.AgentType = BizEntity.Customer.AgentType.Personal;
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.None);
            this.DMRequest = YNStatus.N;
            this.PosterRequest = YNStatus.N;
            this.OpenedToPublic = YNStatus.Y;
        }

        public int? TransactionNumber
        {
            get;
            set;
        }
        public int? CustomerSysNo
        {
            get;
            set;
        }

        private AgentType agentType;
        public AgentType AgentType
        {
            get
            {
                return agentType;
            }
            set
            {
                SetValue("AgentType", ref agentType, value);
            }
        }

        private string _AreaSysNo;
        [Validate(ValidateType.Required)]
        public string AreaSysNo
        {
            get
            {
                return _AreaSysNo;
            }
            set
            {
                SetValue("AreaSysNo", ref _AreaSysNo, value);
            }
        }

        private string college;
        [Validate(ValidateType.Required)]
        public string College
        {
            get
            {
                return college;
            }
            set
            {
                SetValue("College", ref college, value);
            }
        }

        private string certificateNo;
        [Validate(ValidateType.Required)]
        public string CertificateNo
        {
            get
            {
                return certificateNo;
            }
            set
            {
                SetValue("CertificateNo", ref certificateNo, value);
            }
        }

        private string major;
        [Validate(ValidateType.Required)]
        public string Major
        {
            get
            {
                return major;
            }
            set
            {
                SetValue("Major", ref major, value);
            }
        }

        private string profession;
        [Validate(ValidateType.Required)]
        public string Profession
        {
            get
            {
                return profession;
            }
            set
            {
                SetValue("Profession", ref profession, value);
            }
        }

        private string studentNo;
        [Validate(ValidateType.Required)]
        public string StudentNo
        {
            get
            {
                return studentNo;
            }
            set
            {
                SetValue("StudentNo", ref studentNo, value);
            }
        }

        private string qq;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d{5,13}$")]
        public string QQ
        {
            get
            {
                return qq;
            }
            set
            {
                SetValue("QQ", ref qq, value);
            }
        }

        private string msn;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Email)]
        public string MSN
        {
            get
            {
                return msn;
            }
            set
            {
                SetValue("MSN", ref msn, value);
            }
        }

        private string schollmatePhone;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0]?[1][3|5|8][0-9]{9}$")]
        public string SchoolmatePhone
        {
            get
            {
                return schollmatePhone;
            }
            set
            {
                SetValue("SchoolmatePhone", ref schollmatePhone, value);
            }
        }

        private DateTime? graduateDate;
        public DateTime? GraduateDate
        {
            get
            {
                return graduateDate;
            }
            set
            {
                SetValue("GraduateDate", ref graduateDate, value);
            }
        }

        private string schoolBBS;
        [Validate(ValidateType.Required)]
        public string SchoolBBS
        {
            get
            {
                return schoolBBS;
            }
            set
            {
                SetValue("SchoolBBS", ref schoolBBS, value);
            }
        }

        private YNStatus posterRequest;
        public YNStatus PosterRequest
        {
            get
            {
                return posterRequest;
            }
            set
            {
                SetValue("PosterRequest", ref posterRequest, value);
            }
        }

        private YNStatus dmRequest;
        public YNStatus DMRequest
        {
            get
            {
                return dmRequest;
            }
            set
            {
                SetValue("DMRequest", ref dmRequest, value);
            }
        }

        private YNStatus openedToPublic;
        public YNStatus OpenedToPublic
        {
            get
            {
                return openedToPublic;
            }
            set
            {
                SetValue("OpenedToPublic", ref openedToPublic, value);
            }
        }

        private string companyName;
        [Validate(ValidateType.Required)]
        public string CompanyName
        {
            get
            {
                return companyName;
            }
            set
            {
                SetValue("CompanyName", ref companyName, value);
            }
        }

        private string companyPhone;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Phone)]
        public string CompanyPhone
        {
            get
            {
                return companyPhone;
            }
            set
            {
                SetValue("CompanyPhone", ref companyPhone, value);
            }
        }

        private string affectRange;
        [Validate(ValidateType.Required)]
        public string AffectRange
        {
            get
            {
                return affectRange;
            }
            set
            {
                SetValue("AffectRange", ref affectRange, value);
            }
        }

        private string homePhone;
        [Validate(ValidateType.Phone)]
        [Validate(ValidateType.Required)]
        public string HomePhone
        {
            get
            {
                return homePhone;
            }
            set
            {
                SetValue("HomePhone", ref homePhone, value);
            }
        }

        private string suggest;
        [Validate(ValidateType.Required)]
        public string Suggest
        {
            get
            {
                return suggest;
            }
            set
            {
                SetValue("Suggest", ref suggest, value);
            }
        }

        public string Status
        {
            get;
            set;
        }

        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                SetValue("IsEdit", ref isEdit, value);
            }
        }

        public List<KeyValuePair<AgentType?, string>> AgentTypes
        {
            get;
            set;
        }
        public List<KeyValuePair<Boolean?, string>> YNList
        {
            get;
            set;
        }
    }
}
