using System;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleGiftInfoViewModel : ModelBase
    {

        public SaleGiftInfoViewModel()
        {
            this.OrderCondition = new PSOrderConditionViewModel();
            this.OrderCondition.OrderMinAmount = "0.00";


            InitList();
        }

        public void InitList()
        {
            this.WebChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                this.WebChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            this.WebChannel = this.WebChennelList[0];

            this.SaleGiftDiscountBelongTypeList = EnumConverter.GetKeyValuePairs<SaleGiftDiscountBelongType>();
            this.DisCountType = this.SaleGiftDiscountBelongTypeList[0].Key;

            this.SaleGiftTypeList = EnumConverter.GetKeyValuePairs<SaleGiftType>();
            this.Type = this.SaleGiftTypeList[0].Key;

            this.ProductCondition = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            this.BrandC3ScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            this.ProductScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            this.ProductOnlyList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            this.GiftItemList = new List<SaleGift_GiftItemViewModel>();

        }



        /*****************************List*****************************/
        private List<WebChannelVM> m_WebChennelList;
        public List<WebChannelVM> WebChennelList
        {
            get { return this.m_WebChennelList; }
            set { this.SetValue("WebChennelList", ref m_WebChennelList, value); }
        }

        private List<KeyValuePair<SaleGiftDiscountBelongType?, string>> m_SaleGiftDiscountBelongTypeList;
        public List<KeyValuePair<SaleGiftDiscountBelongType?, string>> SaleGiftDiscountBelongTypeList
        {
            get { return this.m_SaleGiftDiscountBelongTypeList; }
            set { this.SetValue("SaleGiftDiscountBelongTypeList", ref m_SaleGiftDiscountBelongTypeList, value); }
        }

        private List<KeyValuePair<SaleGiftType?, string>> m_SaleGiftTypeList;
        public List<KeyValuePair<SaleGiftType?, string>> SaleGiftTypeList
        {
            get { return this.m_SaleGiftTypeList; }
            set { this.SetValue("SaleGiftTypeList", ref m_SaleGiftTypeList, value); }
        }

        private ObservableCollection<SaleGift_RuleSettingViewModel> m_BrandC3ScopeList;
        /// <summary>
        /// 买满即送时，主商品-指定Brand,Category范围设置
        /// </summary>
        public ObservableCollection<SaleGift_RuleSettingViewModel> BrandC3ScopeList
        {
            get { return this.m_BrandC3ScopeList; }
            set { this.SetValue("BrandC3ScopeList", ref m_BrandC3ScopeList, value); }
        }
        private ObservableCollection<SaleGift_RuleSettingViewModel> m_ProductScopeList;
        /// <summary>
        /// 买满即送时，主商品-指定商品范围设置
        /// </summary>
        public ObservableCollection<SaleGift_RuleSettingViewModel> ProductScopeList
        {
            get { return this.m_ProductScopeList; }
            set { this.SetValue("ProductScopeList", ref m_ProductScopeList, value); }
        }

        private ObservableCollection<SaleGift_RuleSettingViewModel> m_ProductOnlyList;
        /// <summary>
        /// 非买满即送时，主商品设置
        /// </summary>
        public ObservableCollection<SaleGift_RuleSettingViewModel> ProductOnlyList
        {
            get { return this.m_ProductOnlyList; }
            set { this.SetValue("ProductOnlyList", ref m_ProductOnlyList, value); }
        }


        //---------------------------------------------------
        private Boolean m_IsGlobalProduct = false;
        /// <summary>
        /// 买满即送时，主商品-整网商品
        /// </summary>
        public Boolean IsGlobalProduct
        {
            get { return this.m_IsGlobalProduct; }
            set { this.SetValue("IsGlobalProduct", ref m_IsGlobalProduct, value); }
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private string m_Title;
        //[Validate(ValidateType.Required)]
        public string Title
        {
            get { return this.m_Title; }
            set { this.SetValue("Title", ref m_Title, value); }
        }

        private string m_Description;
        //[Validate(ValidateType.Required)]
        public string Description
        {
            get { return this.m_Description; }
            set { this.SetValue("Description", ref m_Description, value); }
        }

        private SaleGiftDiscountBelongType? m_DisCountType;
        public SaleGiftDiscountBelongType? DisCountType
        {
            get { return this.m_DisCountType; }
            set { this.SetValue("DisCountType", ref m_DisCountType, value); }
        }

        private String m_PromotionLink;
        [Validate(ValidateType.URL)]
        public String PromotionLink
        {
            get { return this.m_PromotionLink; }
            set { this.SetValue("PromotionLink", ref m_PromotionLink, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private SaleGiftStatus? m_Status = SaleGiftStatus.Init;
        public SaleGiftStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private SaleGiftType? m_Type;
        public SaleGiftType? Type
        {
            get { return this.m_Type; }
            set {
                if (value == SaleGiftType.Single || value == SaleGiftType.Multiple)
                {
                    VendorSysNo = "1";
                    VendorName = "泰隆优选";
                }
                this.SetValue("Type", ref m_Type, value); }
        }

        private String m_AuditUser;
        public String AuditUser
        {
            get { return this.m_AuditUser; }
            set { this.SetValue("AuditUser", ref m_AuditUser, value); }
        }

        private DateTime? m_AuditDate;
        public DateTime? AuditDate
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

        private string m_ItemGiftCount;
        [Validate(ValidateType.Interger)]
        public string ItemGiftCount
        {
            get { return this.m_ItemGiftCount; }
            set { this.SetValue("ItemGiftCount", ref m_ItemGiftCount, value); }
        }



        private SaleGiftGiftItemType? m_GiftComboType = SaleGiftGiftItemType.AssignGift;
        public SaleGiftGiftItemType? GiftComboType
        {
            get { return this.m_GiftComboType; }
            set { this.SetValue("GiftComboType", ref m_GiftComboType, value); }
        }

        private List<SaleGift_GiftItemViewModel> m_GiftItemList;
        public List<SaleGift_GiftItemViewModel> GiftItemList
        {
            get { return this.m_GiftItemList; }
            set { this.SetValue("GiftItemList", ref m_GiftItemList, value); }
        }

        private DateTime? m_BeginDate;
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get { return this.m_BeginDate; }
            set { this.SetValue("BeginDate", ref m_BeginDate, value); }
        }

        private DateTime? m_EndDate;
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return this.m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        private ObservableCollection<SaleGift_RuleSettingViewModel> m_ProductCondition;
        /// <summary>
        /// 数据库读出来的（Entity直接转换的）所有的主商品设置
        /// </summary>
        public ObservableCollection<SaleGift_RuleSettingViewModel> ProductCondition
        {
            get { return this.m_ProductCondition; }
            set { this.SetValue("ProductCondition", ref m_ProductCondition, value); }
        }

        private PSOrderConditionViewModel m_OrderCondition;
        public PSOrderConditionViewModel OrderCondition
        {
            get { return this.m_OrderCondition; }
            set { this.SetValue("OrderCondition", ref m_OrderCondition, value); }
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
        /// <summary>
        /// 库存高级权限
        /// </summary>
        public bool IsAccess
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Gift_IsSpecialCheck);
            }

        }

        public bool HasGiftApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Gift_Approve); }
        }

        public int? RequestSysNo { get; set; }

        private string m_VendorSysNo="1";
        public string VendorSysNo
        {
            get
            {
                return m_VendorSysNo;
            }
            set
            {
                this.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }
        private string m_VendorName = "泰隆优选";
        public string VendorName
        {
            get
            {
                
                return m_VendorName; 
            }
            set
            {
                this.SetValue("VendorName", ref m_VendorName, value);
            }
        }

    

        private bool _brandEnabled;
        public bool BrandEnabled 
        {
            get 
            {
                if (SysNo.HasValue && SysNo > 0)
                {
                    return false;
                }
                else if (Type == SaleGiftType.Full || Type == SaleGiftType.Vendor)
                    //|| Type == SaleGiftType.FirstOrder    || Type == SaleGiftType.Additional) 
                {
                    return true;
                }
                return false;
            }
            set 
            {
                _brandEnabled = value;
            }
        }
    }
}
