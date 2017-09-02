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
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class CpsUserVM : ModelBase
    {

        public CpsUserVM() 
        {
            UserTypeList = EnumConverter.GetKeyValuePairs<UserType>(EnumConverter.EnumAppendItemType.None);
            ListWebSiteType = new List<WebSiteType>();
        }
        public int SysNo { get; set; }
        private CpsBasicUserInfoVM basicUser;
        public CpsBasicUserInfoVM BasicUser 
        {
            get { return basicUser; }
            set { SetValue("BasicUser", ref basicUser, value); }
        }
        private CpsReceivablesAccountVM receivablesAccount;
        public CpsReceivablesAccountVM ReceivablesAccount
        {
            get { return receivablesAccount; }
            set { SetValue("ReceivablesAccount", ref receivablesAccount, value); }
        }
        private CpsUserSourceVM userSource;
        public CpsUserSourceVM UserSource
        {
            get { return userSource; }
            set { SetValue("UserSource", ref userSource, value); }
        }
        /// <summary>
        /// 账户类型
        /// </summary>
        public UserType? UserType { get; set; }
        public List<KeyValuePair<UserType?, string>> UserTypeList { get; set; }

        /// <summary>
        /// 网站类型
        /// </summary>
        public WebSiteType WebSiteType { get; set; }
        public List<WebSiteType> ListWebSiteType { get; set; }

        public AuditStatus Status{get;set;}
        public string AuditNoClearanceInfo { get; set; }
     
       


    }
    public class CpsBasicUserInfoVM : ModelBase
    {
        /// <summary>
        /// 账户类型
        /// </summary>
        public UserType? UserType { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public IsActive? IsActive { get; set; }

        /// <summary>
        /// 联盟账号
        /// </summary>
        private string allianceAccount;
         [Validate(ValidateType.Required)]
        public string AllianceAccount 
        {
            get { return allianceAccount; }
            set { SetValue("AllianceAccount", ref allianceAccount, value); }
        }

        /// <summary>
        /// 网站类型
        /// </summary>
        private string webSiteCode;
         [Validate(ValidateType.Required)]
        public string WebSiteCode 
        {
            get { return webSiteCode; }
            set { SetValue("WebSiteCode", ref webSiteCode, value); }
        }
        /// <summary>
        /// 网址
        /// </summary>
        private string webSiteAddress;
         [Validate(ValidateType.Required)]
         [Validate(ValidateType.URL)]
        public string WebSiteAddress
        {
            get { return webSiteAddress; }
            set { SetValue("WebSiteAddress", ref webSiteAddress, value); }
        }

        /// <summary>
        /// 网站名称
        /// </summary>
        private string webSiteName;
         [Validate(ValidateType.Required)]
        public string WebSiteName
        {
            get { return webSiteName; }
            set { SetValue("WebSiteName", ref webSiteName, value); }
        }


        /// <summary>
        /// 邮箱
        /// </summary>
        private string email;
         [Validate(ValidateType.Required)]
         [Validate(ValidateType.Email)]
        public string Email
        {
            get { return email; }
            set { SetValue("Email", ref email, value); }
        }

        /// <summary>
        /// 联系人
        /// </summary>
        private string contact;
         [Validate(ValidateType.Required)]
        public string Contact
        {
            get { return contact; }
            set { SetValue("Contact", ref contact, value); }
        }

        /// <summary>
        /// 联系手机
        /// </summary>
        private string contactPhone;
          [Validate(ValidateType.Regex, @"^(13[0-9]|15[0|3|6|7|8|9]|18[8|9])\d{8}$", ErrorMessage = "请输入正确的手机号码!")]
         [Validate(ValidateType.Required)]
        public string ContactPhone
        {
            get { return contactPhone; }
            set { SetValue("ContactPhone", ref contactPhone, value); }
        }

        /// <summary>
        /// 通讯地址
        /// </summary>
        private string contactAddress;
         [Validate(ValidateType.Required)]
        public string ContactAddress
        {
            get { return contactAddress; }
            set { SetValue("ContactAddress", ref contactAddress, value); }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        private string zipCode;
         [Validate(ValidateType.ZIP)]
        public string ZipCode
        {
            get { return zipCode; }
            set { SetValue("ZipCode", ref zipCode, value); }
        }
    }

    public class CpsReceivablesAccountVM : ModelBase
    {
        public CpsReceivablesAccountVM()
        {
            IsLockList = EnumConverter.GetKeyValuePairs<IsLock>(EnumConverter.EnumAppendItemType.None);
            ReceivablesAccountTypeList = EnumConverter.GetKeyValuePairs<UserType>(EnumConverter.EnumAppendItemType.None);
            BankTypeList = new List<BankType>();
        }
        /// <summary>
        /// 收款账户类型
        /// </summary>
        public UserType? ReceivablesAccountType { get; set; }
        public List<KeyValuePair<UserType?, string>> ReceivablesAccountTypeList { get; set; }
        /// <summary>
        /// 收款人姓名
        /// </summary>
        private string receiveablesName;
         [Validate(ValidateType.Required)]
        public string ReceiveablesName
        {
            get { return receiveablesName; }
            set { SetValue("ReceiveablesName", ref receiveablesName, value); }
        }

        /// <summary>
        /// 开户银行Code
        /// </summary>
        private string brankCode;
         [Validate(ValidateType.Required)]
        public string BrankCode
        {
            get { return brankCode; }
            set { SetValue("BrankCode", ref brankCode, value); }
        }
        /// <summary>
        /// 开户银行名称
        /// </summary>
        private string brankName;
         [Validate(ValidateType.Required)]
        public string BrankName
        {
            get { return brankName; }
            set { SetValue("BrankName", ref brankName, value); }
        }

        /// <summary>
        ///开户银行支行
        /// </summary>
        private string branchBank;
         [Validate(ValidateType.Required)]
        public string BranchBank
        {
            get { return branchBank; }
            set { SetValue("BranchBank", ref branchBank, value); }
        }

        /// <summary>
        /// 开户卡号
        /// </summary>
        private string brankCardNumber;
         [Validate(ValidateType.Required)]
        public string BrankCardNumber
        {
            get { return brankCardNumber; }
            set { SetValue("BrankCardNumber", ref brankCardNumber, value); }
        }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public IsLock? IsLock { get; set; }
        public List<KeyValuePair<IsLock?, string>> IsLockList { get; set; }


        public BankType Bank { get; set; }
        public List<BankType> BankTypeList { get; set; }
    }
    public class CpsUserSourceVM : ModelBase
    {

        public int SysNo { get; set; }
        /// <summary>
        /// 帐户类型
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        private string chanlName;
          [Validate(ValidateType.Required)]
        public string ChanlName 
        {
            get { return chanlName; }
            set { SetValue("ChanlName", ref chanlName, value); }
        }

        /// <summary>
        /// Source
        /// </summary>
        private string source;
          [Validate(ValidateType.Required)]
        public string Source 
        {
            get { return source; }
            set { SetValue("Source", ref source, value); }
        }
    }
}
