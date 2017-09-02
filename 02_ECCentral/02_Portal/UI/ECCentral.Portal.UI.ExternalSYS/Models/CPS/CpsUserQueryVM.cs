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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class CpsUserQueryVM : ModelBase
    {
        public CpsUserQueryVM() 
        {
            AuditStatusList = EnumConverter.GetKeyValuePairs<AuditStatus>(EnumConverter.EnumAppendItemType.All);
            UserTypeList = EnumConverter.GetKeyValuePairs<UserType>(EnumConverter.EnumAppendItemType.All);
            IsActiveList = EnumConverter.GetKeyValuePairs<IsActive>(EnumConverter.EnumAppendItemType.All);
            ListWebSiteType = new List<WebSiteType>();
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus? AuditStatus { get; set; }

        public List<KeyValuePair<AuditStatus?, string>> AuditStatusList{get;set;}
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
        /// <summary>
        /// 是否可用
        /// </summary>
        public IsActive? IsActive { get; set; }
        public List<KeyValuePair<IsActive?, string>> IsActiveList { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        private string customerID;
        public string CustomerID 
        {
            get { return customerID; }
            set { SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        private string receivablesName;
        public string ReceivablesName 
        {
            get { return receivablesName; }
            set { SetValue("ReceivablesName", ref receivablesName, value); }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        private string email;
        public string Email 
        {
            get { return email; }
            set { SetValue("Email", ref email, value); }
        }

        /// <summary>
        /// 手机
        /// </summary>
        private string phone;
        public string Phone 
        {
            get { return phone; }
            set { SetValue("Phone", ref phone, value); }
        }

        /// <summary>
        /// QQ/MSN 
        /// </summary>
        private string imMessenger;
        public string ImMessenger 
        {
            get { return imMessenger; }
            set { SetValue("ImMessenger", ref imMessenger, value); }
        }

        /// <summary>
        /// 注册日期从
        /// </summary>
        private DateTime? registerDateFrom;
        public DateTime? RegisterDateFrom 
        {
            get { return registerDateFrom; }
            set { SetValue("RegisterDateFrom", ref registerDateFrom, value); }
        }

        /// <summary>
        /// 注册日期到
        /// </summary>
        private DateTime? registerDateTo;
        public DateTime? RegisterDateTo 
        {
            get { return registerDateTo; }
            set { SetValue("RegisterDateTo", ref registerDateTo, value); }
        }

    }
    /// <summary>
    /// 只为CpsUser的查询条件服务
    /// </summary>
    public sealed class WebSiteType 
    {
        public string SelectValue{get;set;} //code
        public string Description { get; set; }//描述

    }

    public sealed class BankType 
    {
        public string SelectValue { get; set; } //code
        public string Description { get; set; }//描述
    }
}
