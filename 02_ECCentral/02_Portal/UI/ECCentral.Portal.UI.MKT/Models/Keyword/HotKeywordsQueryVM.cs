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
    public class HotKeywordsQueryVM: ModelBase
    {
        public HotKeywordsQueryVM()
        {
            this.YNStatusList = new List<KeyValuePair<NYNStatus?, string>>() 
            {
                new KeyValuePair<NYNStatus?, string>(null,"所有"),
                new KeyValuePair<NYNStatus?, string>(NYNStatus.Yes,"否"),
                new KeyValuePair<NYNStatus?, string>(NYNStatus.No,"是"),
            };
        }

        private int? pageType;
        public int? PageType
        {
            get { return pageType; }
            set { base.SetValue("PageType", ref pageType, value); }
        }

        private int? pageID;
        public int? PageID
        {
            get { return pageID; }
            set { base.SetValue("PageID", ref pageID, value); }
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
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
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


        private string _editUserSysNo;
        /// <summary>
        /// 编辑人系统编号
        /// </summary>
        public string EditUserSysNo
        {
            get { return _editUserSysNo; }
            set
            {
                base.SetValue("EditUserSysNo", ref _editUserSysNo, value);
            }
        }
        private DateTime? _editDateFrom;
        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        public DateTime? EditDateFrom
        {
            get { return _editDateFrom; }
            set
            {
                base.SetValue("EditDateFrom", ref _editDateFrom, value);
            }
        }
        private DateTime? _editDateTo;
        /// <summary>
        /// 编辑时间结束于
        /// </summary>
        public DateTime? EditDateTo
        {
            get { return _editDateTo; }
            set
            {
                base.SetValue("EditDateTo", ref _editDateTo, value);
            }
        }
        private DateTime? _invalidDateFrom;
        /// <summary>
        /// 屏蔽时间开始
        /// </summary>
        public DateTime? InvalidDateFrom
        {
            get { return _invalidDateFrom; }
            set
            {
                base.SetValue("InvalidDateFrom", ref _invalidDateFrom, value);
            }
        }
        private DateTime? _invalidDateTo;
        /// <summary>
        /// 屏蔽时间结束
        /// </summary>
        public DateTime? InvalidDateTo
        {
            get { return _invalidDateTo; }
            set
            {
                base.SetValue("InvalidDateTo", ref _invalidDateTo, value);
            }
        }

        private string _companyCode;
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
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

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<NYNStatus?, string>> YNStatusList { get; set; }
    }
}
