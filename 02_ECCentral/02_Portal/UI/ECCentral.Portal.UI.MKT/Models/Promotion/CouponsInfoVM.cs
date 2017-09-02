using System;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Customer;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class CouponsInfoViewModel : ModelBase
    {
        public CouponsInfoViewModel()
        {
            this.WebChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                this.WebChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            this.WebChannel = this.WebChennelList[0];

            this.CouponChannelType = CouponsMKTType.MKTOnline;
            this.CouponRuleType = CouponsRuleType.ProductDiscount;
            this.ProductRangeType = CouponsProductRangeType.AllProducts;
          
            ProductCondition = new PSProductConditionViewModel();
            ProductCondition.ListRelVendorViewModel = new List<RelVendorViewModel>();
            ProductCondition.RelBrands = new RelBrandViewModel();
            ProductCondition.RelBrands.IsIncludeRelation = true;
            ProductCondition.RelCategories = new RelCategory3ViewModel();
            ProductCondition.RelCategories.IsIncludeRelation = true;
            ProductCondition.RelProducts = new RelProductViewModel();
            ProductCondition.RelProducts.IsIncludeRelation = true;
            CustomerCondition = new PSCustomerConditionViewModel();
            CustomerCondition.RelAreas = new RelAreaViewModel();
            CustomerCondition.RelCustomerRanks = new RelCustomerRankViewModel();
            CustomerCondition.RelCustomers = new RelCustomerViewModel();
            OrderCondition = new PSOrderConditionViewModel();
            UsingFrequencyCondition = new PSActivityFrequencyConditionViewModel();
            OrderAmountDiscountRule = new PSOrderAmountDiscountRuleViewModel();

            BindCondition = CouponsBindConditionType.None;
            ValidPeriod = CouponsValidPeriodType.All;
            BindRule = new CouponBindRuleViewModel();
            MerchantSysNo = 1;
            InitList();
        }

        private bool m_IsOnlyViewMode = false;
        public bool IsOnlyViewMode
        {
            get
            {
                return m_IsOnlyViewMode;
            }
            set
            {
                m_IsOnlyViewMode = value;
            }
        }

        public void InitList()
        {
            this.CouponChannelTypeList = EnumConverter.GetKeyValuePairs<CouponsMKTType>();
            this.CouponRuleTypeList = EnumConverter.GetKeyValuePairs<CouponsRuleType>();
            this.ProductRangeTypeList = EnumConverter.GetKeyValuePairs<CouponsProductRangeType>();
            this.BindConditionList = EnumConverter.GetKeyValuePairs<CouponsBindConditionType>();
            this.ValidPeriodList = EnumConverter.GetKeyValuePairs<CouponsValidPeriodType>();
            this.BindProductConditionList = EnumConverter.GetKeyValuePairs<ProductRangeType>();
        }

        private List<WebChannelVM> m_WebChennelList;
        public List<WebChannelVM> WebChennelList
        {
            get { return this.m_WebChennelList; }
            set { this.SetValue("WebChennelList", ref m_WebChennelList, value); }
        }

        private List<KeyValuePair<CouponsMKTType?, string>> m_CouponChannelTypeList;
        public List<KeyValuePair<CouponsMKTType?, string>> CouponChannelTypeList
        {
            get { return this.m_CouponChannelTypeList; }
            set { this.SetValue("CouponChannelTypeList", ref m_CouponChannelTypeList, value); }
        }

        private List<KeyValuePair<CouponsRuleType?, string>> m_CouponRuleTypeList;
        public List<KeyValuePair<CouponsRuleType?, string>> CouponRuleTypeList
        {
            get { return this.m_CouponRuleTypeList; }
            set { this.SetValue("CouponRuleTypeList", ref m_CouponRuleTypeList, value); }
        }

        private List<KeyValuePair<CouponsProductRangeType?, string>> m_ProductRangeTypeList;
        public List<KeyValuePair<CouponsProductRangeType?, string>> ProductRangeTypeList
        {
            get { return this.m_ProductRangeTypeList; }
            set { this.SetValue("ProductRangeTypeList", ref m_ProductRangeTypeList, value); }
        }

        private List<KeyValuePair<CouponsBindConditionType?, string>> m_BindConditionList;
        public List<KeyValuePair<CouponsBindConditionType?, string>> BindConditionList
        {
            get { return this.m_BindConditionList; }
            set { this.SetValue("BindConditionList", ref m_BindConditionList, value); }
        }
        private List<KeyValuePair<CouponsValidPeriodType?, string>> m_ValidPeriodList;
        public List<KeyValuePair<CouponsValidPeriodType?, string>> ValidPeriodList
        {
            get { return this.m_ValidPeriodList; }
            set { this.SetValue("ValidPeriodList", ref m_ValidPeriodList, value); }
        }


        private bool? m_IsDiscountAmount = true;
        public bool? IsDiscountAmount
        {
            get { return this.m_IsDiscountAmount; }
            set { this.SetValue("IsDiscountAmount", ref m_IsDiscountAmount, value); }
        }

        private bool? m_IsDiscountPercent = false;
        public bool? IsDiscountPercent
        {
            get { return this.m_IsDiscountPercent; }
            set { this.SetValue("IsDiscountPercent", ref m_IsDiscountPercent, value); }
        }

        private bool? m_IsDiscountSubtract = false;
        public bool? IsDiscountSubtract
        {
            get { return this.m_IsDiscountSubtract; }
            set { this.SetValue("IsDiscountSubtract", ref m_IsDiscountSubtract, value); }
        }

        private bool? m_IsDiscountFinal = false;
        public bool? IsDiscountFinal
        {
            get { return this.m_IsDiscountFinal; }
            set { this.SetValue("IsDiscountFinal", ref m_IsDiscountFinal, value); }
        }

        private Int32? m_PayTypeSysNo;
        public Int32? PayTypeSysNo
        {
            get { return this.m_PayTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private Int32? m_ShipTypeSysNo;
        public Int32? ShipTypeSysNo
        {
            get { return this.m_ShipTypeSysNo; }
            set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value); }
        }

        private bool? m_IsCustomerNoLimit = true;
        public bool? IsCustomerNoLimit
        {
            get { return this.m_IsCustomerNoLimit; }
            set { this.SetValue("IsCustomerNoLimit", ref m_IsCustomerNoLimit, value); }
        }

        private bool? m_IsCustomerRank = false;
        public bool? IsCustomerRank
        {
            get { return this.m_IsCustomerRank; }
            set { this.SetValue("IsCustomerRank", ref m_IsCustomerRank, value); }
        }

        private bool? m_IsCustomerID = false;
        public bool? IsCustomerID
        {
            get { return this.m_IsCustomerID; }
            set { this.SetValue("IsCustomerID", ref m_IsCustomerID, value); }
        }

        private bool? m_IsAreaNoLimit = true;
        public bool? IsAreaNoLimit
        {
            get { return this.m_IsAreaNoLimit; }
            set { this.SetValue("IsAreaNoLimit", ref m_IsAreaNoLimit, value); }
        }

        private bool? m_IsAreaLimit = false;
        public bool? IsAreaLimit
        {
            get { return this.m_IsAreaLimit; }
            set { this.SetValue("IsAreaLimit", ref m_IsAreaLimit, value); }
        }

        private List<KeyValuePair<ProductRangeType?, string>> m_BindProductConditionList;
        public List<KeyValuePair<ProductRangeType?, string>> BindProductConditionList
        {
            get { return this.m_BindProductConditionList; }
            set { this.SetValue("BindProductConditionList", ref m_BindProductConditionList, value); }
        }

        /**************下面是原生态，上面是为了UI额外加的*******************/

        #region 基础
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private string m_Title;

        [Validate(ValidateType.Required)]
        public string Title
        {
            get { return this.m_Title; }
            set { this.SetValue("Title", ref m_Title, value); }
        }


        private String m_AuditUser;
        public String AuditUser
        {
            get { return this.m_AuditUser; }
            set { this.SetValue("AuditUser", ref m_AuditUser, value); }
        }

        private DateTime m_AuditDate;
        public DateTime AuditDate
        {
            get { return this.m_AuditDate; }
            set { this.SetValue("AuditDate", ref m_AuditDate, value); }
        }

        private String m_InUser;
        public String InUser
        {
            get { return this.m_InUser; }
            set { this.SetValue("InUser", ref m_InUser, value); }
        }

        private DateTime? m_InDate;
        public DateTime? InDate
        {
            get { return this.m_InDate; }
            set { this.SetValue("InDate", ref m_InDate, value); }
        }

        private String m_EditUser;
        public String EditUser
        {
            get { return this.m_EditUser; }
            set { this.SetValue("EditUser", ref m_EditUser, value); }
        }

        private DateTime? m_EditDate;
        public DateTime? EditDate
        {
            get { return this.m_EditDate; }
            set { this.SetValue("EditDate", ref m_EditDate, value); }
        }

        private WebChannelVM m_WebChannel;
        public WebChannelVM WebChannel
        {
            get { return this.m_WebChannel; }
            set { this.SetValue("WebChannel", ref m_WebChannel, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private CouponsStatus? m_Status = CouponsStatus.Init;
        public CouponsStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }



        private String m_UserDescription;
        public String UserDescription
        {
            get { return this.m_UserDescription; }
            set { this.SetValue("UserDescription", ref m_UserDescription, value); }
        }

        private Int32? m_MerchantSysNo;
        public Int32? MerchantSysNo
        {
            get { return this.m_MerchantSysNo; }
            set { this.SetValue("MerchantSysNo", ref m_MerchantSysNo, value); }
        }

        private string m_EIMSSysNo;
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "必须是整数，且大于0")]
        [Validate(ValidateType.Required)]
        public string EIMSSysNo
        {
            get { return this.m_EIMSSysNo; }
            set { this.SetValue("EIMSSysNo", ref m_EIMSSysNo, value); }
        }

        private CouponsMKTType? m_CouponChannelType;
        public CouponsMKTType? CouponChannelType
        {
            get { return this.m_CouponChannelType; }
            set { this.SetValue("CouponChannelType", ref m_CouponChannelType, value); }
        }

        private CouponsRuleType m_CouponRuleType;
        public CouponsRuleType CouponRuleType
        {
            get { return this.m_CouponRuleType; }
            set { this.SetValue("CouponRuleType", ref m_CouponRuleType, value); }
        }

        private CouponsProductRangeType m_ProductRangeType;
        public CouponsProductRangeType ProductRangeType
        {
            get { return this.m_ProductRangeType; }
            set { this.SetValue("ProductRangeType", ref m_ProductRangeType, value); }
        }
        #endregion

        private Boolean? m_IsAutoUse = false;
        public Boolean? IsAutoUse
        {
            get { return this.m_IsAutoUse; }
            set { this.SetValue("IsAutoUse", ref m_IsAutoUse, value); }
        }

        #region 发放规则
        private Boolean? m_IsSendMail = false;
        public Boolean? IsSendMail
        {
            get { return this.m_IsSendMail; }
            set { this.SetValue("IsSendMail", ref m_IsSendMail, value); }
        }

        private Boolean? m_IsAutoBinding = false;
        public Boolean? IsAutoBinding
        {
            get { return this.m_IsAutoBinding; }
            set
            {
                this.SetValue("IsAutoBinding", ref m_IsAutoBinding, value);
                if (!m_IsAutoBinding.HasValue || m_IsAutoBinding == false)
                {
                    BindingDate = null;
                }
            }
        }

        private DateTime? m_BindingDate;
        public DateTime? BindingDate
        {
            get { return this.m_BindingDate; }
            set { this.SetValue("BindingDate", ref m_BindingDate, value); }
        }

        private CouponsBindConditionType? m_BindCondition;
        public CouponsBindConditionType? BindCondition
        {
            get { return this.m_BindCondition; }
            set { this.SetValue("BindCondition", ref m_BindCondition, value); }
        }

        private CouponsValidPeriodType? m_ValidPeriod;
        public CouponsValidPeriodType? ValidPeriod
        {
            get { return this.m_ValidPeriod; }
            set { this.SetValue("ValidPeriod", ref m_ValidPeriod, value); }
        }

        private DateTime? m_CustomBindBeginDate;
        public DateTime? CustomBindBeginDate
        {
            get { return this.m_CustomBindBeginDate; }
            set { this.SetValue("CustomBindBeginDate", ref m_CustomBindBeginDate, value); }
        }

        private DateTime? m_CustomBindEndDate;
        public DateTime? CustomBindEndDate
        {
            get { return this.m_CustomBindEndDate; }
            set { this.SetValue("CustomBindEndDate", ref m_CustomBindEndDate, value); }
        }

        private String m_BindingStatus;
        public String BindingStatus
        {
            get { return this.m_BindingStatus; }
            set { this.SetValue("BindingStatus", ref m_BindingStatus, value); }
        }
        #endregion


        #region 优惠券代码设置
        private CouponCodeSettingViewModel m_CouponCodeSetting;
        public CouponCodeSettingViewModel CouponCodeSetting
        {
            get { return this.m_CouponCodeSetting; }
            set { this.SetValue("CouponCodeSetting", ref m_CouponCodeSetting, value); }
        }

        private Boolean? m_IsExistThrowInTypeCouponCode = false;
        public Boolean? IsExistThrowInTypeCouponCode
        {
            get { return this.m_IsExistThrowInTypeCouponCode; }
            set { this.SetValue("IsExistThrowInTypeCouponCode", ref m_IsExistThrowInTypeCouponCode, value); }
        }
        #endregion


        #region 条件
        private DateTime? m_StartTime;
        [Validate(ValidateType.Required)]
        public DateTime? StartTime
        {
            get { return this.m_StartTime; }
            set { this.SetValue("StartTime", ref m_StartTime, value); }
        }

        private DateTime? m_EndTime;
        [Validate(ValidateType.Required)]
        public DateTime? EndTime
        {
            get { return this.m_EndTime; }
            set { this.SetValue("EndTime", ref m_EndTime, value); }
        }

        private PSProductConditionViewModel m_ProductCondition;
        public PSProductConditionViewModel ProductCondition
        {
            get { return this.m_ProductCondition; }
            set { this.SetValue("ProductCondition", ref m_ProductCondition, value); }
        }

        private PSCustomerConditionViewModel m_CustomerCondition;
        public PSCustomerConditionViewModel CustomerCondition
        {
            get { return this.m_CustomerCondition; }
            set { this.SetValue("CustomerCondition", ref m_CustomerCondition, value); }
        }

        private PSOrderConditionViewModel m_OrderCondition;
        public PSOrderConditionViewModel OrderCondition
        {
            get { return this.m_OrderCondition; }
            set { this.SetValue("OrderCondition", ref m_OrderCondition, value); }
        }

        private PSActivityFrequencyConditionViewModel m_UsingFrequencyCondition;
        public PSActivityFrequencyConditionViewModel UsingFrequencyCondition
        {
            get { return this.m_UsingFrequencyCondition; }
            set { this.SetValue("UsingFrequencyCondition", ref m_UsingFrequencyCondition, value); }
        }
        
        #endregion

        #region 规则，优惠券中，使用PriceDiscountRuleList和PriceDiscountRuleList

        private ObservableCollection<PSPriceDiscountRuleViewModel> m_PriceDiscountRule;
        public ObservableCollection<PSPriceDiscountRuleViewModel> PriceDiscountRule
        {
            get { return this.m_PriceDiscountRule; }
            set { this.SetValue("PriceDiscountRule", ref m_PriceDiscountRule, value); }
        }

        private PSOrderAmountDiscountRuleViewModel m_OrderAmountDiscountRule;
        public PSOrderAmountDiscountRuleViewModel OrderAmountDiscountRule
        {
            get { return this.m_OrderAmountDiscountRule; }
            set { this.SetValue("OrderAmountDiscountRule", ref m_OrderAmountDiscountRule, value); }
        }
        #endregion


        public bool HasCouponCodeApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCode_Approve); }
            //get { return true; }
        }

        public bool HasCouponCodeEditPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCode_Edit); }
            //get { return true; }
        }

        public bool HasCouponCodeStopApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCodeStop_Approve); }
        }

        private CouponBindRuleViewModel m_BindRule;
        /// <summary>
        ///购物赠送型 绑定规则
        /// </summary>
        public CouponBindRuleViewModel BindRule
        {
            get { return this.m_BindRule; }
            set { this.SetValue("BindRule", ref m_BindRule, value); }
        }
    }

    public class CouponBindRuleViewModel : ModelBase
    {
        public CouponBindRuleViewModel()
        {
            RelProducts = new RelProductViewModel();
        }

        private string m_AmountLimit;
        /// <summary>
        /// 触发门槛金额
        /// </summary>
        [Validate(ValidateType.Regex, @"^([1-9]\d*|0)\.?[0-9]{0,2}$", ErrorMessage = "必须是大于等于1的数字")]
        public string AmountLimit
        {
            get { return this.m_AmountLimit; }
            set { this.SetValue("AmountLimit", ref m_AmountLimit, value); }
        }


        public ProductRangeType m_ProductRangeType;

        public ProductRangeType ProductRangeType
        {
            get { return this.m_ProductRangeType; }
            set { this.SetValue("ProductRangeType", ref m_ProductRangeType, value); }
        }


        private RelProductViewModel m_RelProducts;
        /// <summary>
        /// 商品范围
        /// </summary>
        public RelProductViewModel RelProducts
        {
            get { return this.m_RelProducts; }
            set { this.SetValue("RelProducts", ref m_RelProducts, value); }
        }
    }
}
