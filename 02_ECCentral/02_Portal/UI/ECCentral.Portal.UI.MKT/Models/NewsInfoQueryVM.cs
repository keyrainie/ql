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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.MKT.Models
{
    public class NewsInfoQueryVM : ModelBase
    {
        public NewsInfoQueryVM()
        {
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<NewsStatus>(EnumConverter.EnumAppendItemType.All);
            this.SelectRangeList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID = "0";
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }
        private string _SelectedArea;

        public string SelectedArea
        {
            get { return _SelectedArea; }
            set { base.SetValue("SelectedArea", ref _SelectedArea, value); }
        }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<NewsStatus?, string>> ShowStatusList { get; set; }

        /// <summary>
        /// 所有，是，否
        /// </summary>
        public List<KeyValuePair<bool?, string>> SelectRangeList { get; set; }
        private bool? _SelectRange;

        public bool? SelectRange
        {
            get { return _SelectRange; }
            set { base.SetValue("SelectRange", ref _SelectRange, value); }
        }

        /// <summary>
        /// 标题
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set { base.SetValue("Title", ref title, value); }
        }

        /// <summary>
        /// 副标题
        /// </summary>
        private string subtitle;
        public string Subtitle
        {
            get { return subtitle; }
            set { base.SetValue("Subtitle", ref subtitle, value); }
        }

        /// <summary>
        /// 编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 编辑人
        /// </summary>
        private string inUser;
        public string InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }

        /// <summary>
        /// 是否展示评论
        /// </summary>
        private bool? isShow;
        public bool? IsShow
        {
            get { return isShow; }
            set { base.SetValue("IsShow", ref isShow, value); }
        }

        /// <summary>
        /// 置顶
        /// </summary>
        private bool? isSetTop;
        public bool? IsSetTop
        {
            get { return isSetTop; }
            set { base.SetValue("IsSetTop", ref isSetTop, value); }
        }

        /// <summary>
        /// 漂红
        /// </summary>
        private bool? isRed;
        public bool? IsRed
        {
            get { return isRed; }
            set { base.SetValue("IsRed", ref isRed, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private NewsStatus? status;
        public NewsStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? inDateFromTo;
        public DateTime? InDateFromTo
        {
            get { return inDateFromTo; }
            set { base.SetValue("InDateFromTo", ref inDateFromTo, value); }
        }



        /// <summary>
        /// 类别
        /// </summary>
        private int? newsType;
        public int? NewsType
        {
            get { return newsType; }
            set { base.SetValue("NewsType", ref newsType, value); }
        }

        private int? _ReferenceSysNo;

        /// <summary>
        /// 大区
        /// </summary>
        public int? ReferenceSysNo
        {
            get { return _ReferenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref _ReferenceSysNo, value); }
        }

    }

    public class NewsInfoMaintainVM : ModelBase
    {
        public NewsInfoMaintainVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //      this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
            this.ReplyCsutomerRankList = EnumConverter.GetKeyValuePairs<CustomerRank>(EnumConverter.EnumAppendItemType.All);
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<NewsStatus>();
        }


        public string CompanyCode { get; set; }
        public List<UIWebChannel> WebChannelList { get; set; }
        private string _ChannelID="0";
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }
        public int? SysNo { get; set; }

        private int? _NewsType;     
        public int? NewsType
        {
            get { return _NewsType; }
            set { base.SetValue("NewsType", ref _NewsType, value); }
        }
        private int? _ReferenceSysNo;

        public int? ReferenceSysNo
        {
            get { return _ReferenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref _ReferenceSysNo, value); }
        }

        private string _SelectedArea;

        public string SelectedArea
        {
            get { return _SelectedArea; }
            set { base.SetValue("SelectedArea", ref _SelectedArea, value); }
        }
        public List<int> AreaShow
        {
            get
            {
                var list = new List<int>();
                if (!string.IsNullOrEmpty(SelectedArea))
                {
                    SelectedArea.Split(',').ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            list.Add(int.Parse(item));
                        }
                    });

                }
                return list;
            }
            set
            {
                SelectedArea = value.Join(",");
            }
        }

        private string _Title;
        [Validate(ValidateType.Required)]
        public string Title
        {
            get { return _Title; }
            set { base.SetValue("Title", ref _Title, value); }
        }
        /// <summary>
        /// 副标题
        /// </summary>
        private string subtitle;
        public string Subtitle
        {
            get { return subtitle; }
            set { base.SetValue("Subtitle", ref subtitle, value); }
        }
        private string _LinkUrl;
        [Validate(ValidateType.URL)]
        public string LinkUrl
        {
            get { return _LinkUrl; }
            set { base.SetValue("LinkUrl", ref _LinkUrl, value); }
        }

        private string _CoverImageUrl;
        public string CoverImageUrl
        {
            get { return _CoverImageUrl; }
            set { base.SetValue("CoverImageUrl", ref _CoverImageUrl, value); }
        }

        private string _Content;
        //[Validate(ValidateType.Required)]
        //[Validate(ValidateType.MaxLength, 2000)]
        public string Content
        {
            get { return _Content; }
            set { base.SetValue("Content", ref _Content, value); }
        }
        private string _ExpireDate;
        [Validate(ValidateType.Required)]
        public string ExpireDate
        {
            get { return _ExpireDate; }
            set { base.SetValue("ExpireDate", ref _ExpireDate, value); }
        }
        private bool _Extendflag;

        public bool Extendflag
        {
            get { return _Extendflag; }
            set { base.SetValue("Extendflag", ref _Extendflag, value); }
        }

        private bool _TopMost;

        public bool TopMost
        {
            get { return _TopMost; }
            set { base.SetValue("TopMost", ref _TopMost, value); }
        }


        private bool _IsRed;

        public bool IsRed
        {
            get { return _IsRed; }
            set { base.SetValue("IsRed", ref _IsRed, value); }
        }

        private bool _EnableComment;

        public bool EnableComment
        {
            get { return false; }
            set { base.SetValue("EnableComment", ref _EnableComment, value); }
        }
        private NewsStatus? status;
        [Validate(ValidateType.Required)]
        public NewsStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        private CustomerRank? _EnableReplyRank;

        public CustomerRank? EnableReplyRank
        {
            get { return _EnableReplyRank; }
            set { base.SetValue("EnableReplyRank", ref _EnableReplyRank, value); }
        }

        private bool _IsHomePageShow;

        public bool IsHomePageShow
        {
            get { return _IsHomePageShow; }
            set { base.SetValue("IsHomePageShow", ref _IsHomePageShow, value); }
        }
        private bool _IsC1Show;

        public bool IsC1Show
        {
            get { return _IsC1Show; }
            set { base.SetValue("IsC1Show", ref _IsC1Show, value); }
        }
        private bool _IsC2Show;

        public bool IsC2Show
        {
            get { return _IsC2Show; }
            set { base.SetValue("IsC2Show", ref _IsC2Show, value); }
        }

        private string _ContainPageId;

        public string ContainPageId
        {
            get { return _ContainPageId; }
            set { base.SetValue("ContainPageId", ref _ContainPageId, value); }
        }

        public List<KeyValuePair<NewsStatus?, string>> ShowStatusList { get; set; }
        public List<KeyValuePair<CustomerRank?, string>> ReplyCsutomerRankList { get; set; }

        private string _Priority;
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return _Priority; }
            set { base.SetValue("Priority", ref _Priority, value); }
        }
    }
}
