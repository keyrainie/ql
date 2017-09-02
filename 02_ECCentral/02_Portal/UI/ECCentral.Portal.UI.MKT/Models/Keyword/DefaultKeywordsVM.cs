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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class DefaultKeywordsVM : ModelBase
    {
        public string CompanyCode { get; set; }

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
        /// 排除的页面ID
        /// </summary>
        private string exceptPageID;
        public string ExceptPageID
        {
            get { return exceptPageID; }
            set { base.SetValue("ExceptPageID", ref exceptPageID, value); }
        }

        private string sysNo;
        public string SysNo
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

        /// <summary>
        /// 扩展生效
        /// </summary>
        private bool? extend;
        public bool? Extend
        {
            get { return extend; }
            set { base.SetValue("Extend", ref extend, value); }
        }

        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private DateTime? _beginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set
            {
                base.SetValue("BeginDate", ref _beginDate, value);
            }
        }

        private DateTime? _endDate;
        /// <summary>
        /// 结束时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
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
    }
}
