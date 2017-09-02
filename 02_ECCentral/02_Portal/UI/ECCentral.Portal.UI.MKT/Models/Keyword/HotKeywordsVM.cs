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
    public class HotKeywordsVM: ModelBase
    {
        public HotKeywordsVM()
        {
            this.YNStatusList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private int? pageType;
        [Validate(ValidateType.Required)]
        public int? PageType
        {
            get { return pageType; }
            set { base.SetValue("PageType", ref pageType, value); }
        }

        private string pageTypeName;
        public string PageTypeName
        {
            get { return pageTypeName; }
            set { base.SetValue("PageTypeName", ref pageTypeName, value); }
        }
        private string pageIDName;
        public string PageIDName
        {
            get { return pageIDName; }
            set { base.SetValue("PageIDName", ref pageIDName, value); }
        }

        private int? pageID;
        public int? PageID
        {
            get { return pageID; }
            set { base.SetValue("PageID", ref pageID, value); }
        }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 状态  对应 IsOnlineShow
        /// </summary>
        private NYNStatus? status;
        public NYNStatus? IsOnlineShow
        {
            get { return status; }
            set { base.SetValue("IsOnlineShow", ref status, value); }
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
        /// 优先级
        /// </summary>
        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

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
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<YNStatus?, string>> YNStatusList { get; set; }

        public string EditUser { get; set; }

        public string CompanyCode { get; set; }

        public DateTime? EditDate { get; set; }

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
