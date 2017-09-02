using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SearchedKeywordsQueryVM: ModelBase
    {
        public SearchedKeywordsQueryVM()
        {
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<YNStatus?, string>> ShowStatusList { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        private string editUser;
        public string EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
        }

        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 编辑时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 展示状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 添加用户类型
        /// </summary>
        private KeywordsOperateUserType? createUserType;
        public KeywordsOperateUserType? CreateUserType
        {
            get { return createUserType; }
            set { base.SetValue("CreateUserType", ref createUserType, value); }
        }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
    }

    public class SearchedKeywordsVM : ModelBase
    {
        public SearchedKeywordsVM()
        {
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public int? JDCount { get; set; }

        public int? ItemCount { get; set; }

        /// <summary>
        /// SysNo
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<YNStatus?, string>> ShowStatusList { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        private string keywords;
        [Validate(ValidateType.Required)]
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        private string editUser;
        public string EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
        }

        /// <summary>
        /// 编辑时间
        /// </summary>
        private DateTime? editDate;
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 编辑时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        private DateTime? beginDate;
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { base.SetValue("BeginDate", ref beginDate, value); }
        }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }


        /// <summary>
        /// 优先级
        /// </summary>
        private string priority;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 展示状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 添加用户类型
        /// </summary>
        private KeywordsOperateUserType? createUserType;
        public KeywordsOperateUserType? CreateUserType
        {
            get { return createUserType; }
            set { base.SetValue("CreateUserType", ref createUserType, value); }
        }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
    }
}
