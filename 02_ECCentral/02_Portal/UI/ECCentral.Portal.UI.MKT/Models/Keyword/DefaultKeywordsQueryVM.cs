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
    public class DefaultKeywordsQueryVM : ModelBase
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
        /// 关键字
        /// </summary>
        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }

       
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private DateTime? _beginDateFrom;
        /// <summary>
        /// 开始时间范围从
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get { return _beginDateFrom; }
            set
            {
                base.SetValue("BeginDateFrom", ref _beginDateFrom, value);
            }
        }
        private DateTime? _beginDateTo;
        /// <summary>
        /// 开始时间范围到
        /// </summary>
        public DateTime? BeginDateTo
        {
            get { return _beginDateTo; }
            set
            {
                base.SetValue("BeginDateTo", ref _beginDateTo, value);
            }
        }
        private DateTime? _endDateFrom;
        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? EndDateFrom
        {
            get { return _endDateFrom; }
            set
            {
                base.SetValue("EndDateFrom", ref _endDateFrom, value);
            }
        }
        private DateTime? _endDateTo;
        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? EndDateTo
        {
            get { return _endDateTo; }
            set
            {
                base.SetValue("EndDateTo", ref _endDateTo, value);
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
