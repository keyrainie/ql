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
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SubscriptionQueryVM : ModelBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 电子邮件地址
        /// </summary>
        private string email;
        public string Email
        {
            get { return email; }
            set { base.SetValue("Email", ref email, value); }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string ipAddress;
        public string IPAddress
        {
            get { return ipAddress; }
            set { base.SetValue("IPAddress", ref ipAddress, value); }
        }

        /// <summary>
        /// IP订阅时间
        /// </summary>
        private DateTime? inDate;
        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
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

        /// <summary>
        /// 订阅分类名称
        /// </summary>
        private string _subscriptionCategoryName;
        public string SubscriptionCategoryName
        {
            get { return _subscriptionCategoryName; }
            set { SetValue("SubscriptionCategoryName", ref _subscriptionCategoryName, value); }
        }
    }
}
