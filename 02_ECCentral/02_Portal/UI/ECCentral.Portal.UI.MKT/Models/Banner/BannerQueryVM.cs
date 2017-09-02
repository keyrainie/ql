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
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class BannerQueryVM : ModelBase
    {
        private int? _positionID;
        /// <summary>
        /// 位置系统编号
        /// </summary>
        public int? PositionID
        {
            get { return _positionID; }
            set
            {
                base.SetValue("PositionID", ref _positionID, value);
            }
        }
        private int? _pageType;
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get { return _pageType; }
            set
            {
                base.SetValue("PageType", ref _pageType, value);
            }
        }
        private int? _pageID;
        /// <summary>
        /// 页面编号
        /// </summary>
        public int? PageID
        {
            get { return _pageID; }
            set
            {
                base.SetValue("PageID", ref _pageID, value);
            }
        }
        private BannerType? _bannerType;
        /// <summary>
        /// 广告类型
        /// </summary>
        public BannerType? BannerType
        {
            get { return _bannerType; }
            set
            {
                base.SetValue("BannerType", ref _bannerType, value);
            }
        }
        private ADStatus? _status;
        /// <summary>
        /// 类型
        /// </summary>
        public ADStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
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
        private string _bannerTitle;
        /// <summary>
        /// 广告标题
        /// </summary>
        public string BannerTitle
        {
            get { return _bannerTitle; }
            set
            {
                base.SetValue("BannerTitle", ref _bannerTitle, value);
            }
        }
        private string _areaShow;
        /// <summary>
        /// 主要投放区域
        /// </summary>
        public string AreaShow
        {
            get { return _areaShow; }
            set
            {
                base.SetValue("AreaShow", ref _areaShow, value);
            }
        }

        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        /// <summary>
        /// 状态字典列表
        /// </summary>
        public List<KeyValuePair<ADStatus?, string>> StatusKVList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 广告类型字典列表
        /// </summary>
        public List<KeyValuePair<BannerType?, string>> BannerTypeKVList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<BannerType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
                //channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
                return channelList;
            }
        }
    }
}
