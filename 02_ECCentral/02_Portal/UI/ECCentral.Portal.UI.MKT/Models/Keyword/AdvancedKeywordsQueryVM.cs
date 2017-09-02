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
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class AdvancedKeywordsQueryVM : ModelBase
    {
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
        /// 状态      D=无效  A=有效
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        private string priority;
      [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        private string linkUrl;
        public string LinkUrl
        {
            get { return linkUrl; }
            set { base.SetValue("LinkUrl", ref linkUrl, value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? beginDate;
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { base.SetValue("BeginDate", ref beginDate, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }

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
    
    public class AdvancedKeywordsVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
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
        private string showName;
        [Validate(ValidateType.Required)]
        public string ShowName
        {
            get { return showName; }
            set { base.SetValue("ShowName", ref showName, value); }
        }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        private ADStatus? status;
        [Validate(ValidateType.Required)]
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        /// <summary>
        /// 是否自动转换？？0=否  1=是
        /// </summary>
        private NYNStatus? autoRedirectSwitch;
        [Validate(ValidateType.Required)]
        public NYNStatus? AutoRedirectSwitch
        {
            get { return autoRedirectSwitch; }
            set { base.SetValue("AutoRedirectSwitch", ref autoRedirectSwitch, value); }
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
        /// 链接地址
        /// </summary>
        private string linkUrl;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.URL)]
        public string LinkUrl
        {
            get { return linkUrl; }
            set { base.SetValue("LinkUrl", ref linkUrl, value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? beginDate;
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { base.SetValue("BeginDate", ref beginDate, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        private bool? isChecked;
        public bool? IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

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