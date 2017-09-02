using System;
using System.Linq;
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
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class PollListVM: ModelBase
    {
        public string CompanyCode { get; set; }

        private string sysNo;
        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 投票标题
        /// </summary>
        private string pollName;
        [Validate(ValidateType.Required)]
        public string PollName
        {
            get { return pollName; }
            set { base.SetValue("PollName", ref pollName, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 用户自定义
        /// </summary>
        private YNStatus? userDefined;
        public YNStatus? UserDefined
        {
            get { return userDefined; }
            set { base.SetValue("UserDefined", ref userDefined, value); }
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
        /// 投票数量
        /// </summary>
        private string pollCount;
        public string PollCount
        {
            get { return pollCount; }
            set { base.SetValue("PollCount", ref pollCount, value); }
        }


        /// <summary>
        /// 投票问题组
        /// </summary>
        public List<PollItemGroup> PollItemGroupList { get; set; }


        public bool IsActive
        {
            get
            {
                return Status == ADStatus.Active;
            }
            set
            {
                if (value)
                    Status = ADStatus.Active;
                else
                    Status = ADStatus.Deactive;
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

    public class PollListQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }


        private int? category1SysNo;
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }
        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }
        private int? category3SysNo;
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 投票标题
        /// </summary>
        private string pollName;
        public string PollName
        {
            get { return pollName; }
            set { base.SetValue("PollName", ref pollName, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 用户自定义
        /// </summary>
        private YNStatus? userDefined;
        public YNStatus? UserDefined
        {
            get { return userDefined; }
            set { base.SetValue("UserDefined", ref userDefined, value); }
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

    /// <summary>
    /// 投票问题组
    /// </summary>
    public class PollItemGroupVM : ModelBase
    {
        private int? pollSysNo;
        public int? PollSysNo
        {
            get { return pollSysNo; }
            set { base.SetValue("PollSysNo", ref pollSysNo, value); }
        }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 投票问题题
        /// </summary>
        private string groupName;
        [Validate(ValidateType.Required)]
        public string GroupName
        {
            get { return groupName; }
            set { base.SetValue("GroupName", ref groupName, value); }
        }

        private PollType? type;
        [Validate(ValidateType.Required)]
        public PollType? Type
        {
            get { return type; }
            set { base.SetValue("Type", ref type, value); }
        }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

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


    /// <summary>
    /// 投票问题组投票子项
    /// </summary>
    public class PollItemVM : ModelBase
    {
        private int? pollItemGroupSysno;
        public int? PollItemGroupSysno
        {
            get { return pollItemGroupSysno; }
            set { base.SetValue("PollItemGroupSysno", ref pollItemGroupSysno, value); }
        }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 投票问题题
        /// </summary>
        private string itemName;
        [Validate(ValidateType.Required)]
        public string ItemName
        {
            get { return itemName; }
            set { base.SetValue("ItemName", ref itemName, value); }
        }

        /// <summary>
        /// 选中次数
        /// </summary>
        public int? PollCount { get; set; }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }
    }
}
