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
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class LeaveWordQueryVM : ModelBase
    {
        private string sysNo;
        [Validate(ValidateType.Regex, @"^[0-9]\d*$", ErrorMessage = "编号必须是整数，且大于等于0")]
        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 主题
        /// </summary>
        private string subject;
        public string Subject
        {
            get { return subject; }
            set { base.SetValue("Subject", ref subject, value); }
        }

        private string csNote;
        public string CSNote
        {
            get { return csNote; }
            set { base.SetValue("CSNote", ref csNote, value); }
        }

        private string replyContent;
        public string ReplyContent
        {
            get { return replyContent; }
            set { base.SetValue("ReplyContent", ref replyContent, value); }
        }
        /// <summary>
        /// 处理人
        /// </summary>
        private int? updateUserSysNo;
        public int? UpdateUserSysNo
        {
            get { return updateUserSysNo; }
            set { base.SetValue("UpdateUserSysNo", ref updateUserSysNo, value); }
        }

        /// <summary>
        /// 是否是有效case
        /// </summary>
        private bool isValidCase;
        public bool IsValidCase
        {
            get { return isValidCase; }
            set { base.SetValue("IsValidCase", ref isValidCase, value); }
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        private string soSysNo;
        [Validate(ValidateType.Regex, @"^[0-9]\d*$", ErrorMessage = "订单编号必须是整数，且大于等于0")]
        public string SOSysNo
        {
            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        private string customerSysNo;
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { base.SetValue("CustomerName", ref customerName, value); }
        }
        
        /// <summary>
        /// 用户Email
        /// </summary>
        public string CustomerEmail { get; set; }
        
        /// <summary>
        /// 最后更新人
        /// </summary>
        public string LastEditUserName { get; set; }

        /// <summary>
        /// 新回复的邮件内容
        /// </summary>
        public string MailReplyContent { get; set; }
        
        /// <summary>
        /// 评论开始时间
        /// </summary>
        private DateTime? createTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get { return createTimeFrom; }
            set { base.SetValue("CreateTimeFrom", ref createTimeFrom, value); }
        }

        /// <summary>
        /// 评论结束时间
        /// </summary>
        private DateTime? createTimeTo;
        public DateTime? CreateTimeTo
        {
            get { return createTimeTo; }
            set { base.SetValue("CreateTimeTo", ref createTimeTo, value); }
        }

        /// <summary>
        /// 处理开始时间
        /// </summary>
        private DateTime? updateTimeFrom;
        public DateTime? UpdateTimeFrom
        {
            get { return updateTimeFrom; }
            set { base.SetValue("UpdateTimeFrom", ref updateTimeFrom, value); }
        }

        /// <summary>
        /// 处理结束时间
        /// </summary>
        private DateTime? updateTimeTo;
        public DateTime? UpdateTimeTo
        {
            get { return updateTimeTo; }
            set { base.SetValue("UpdateTimeTo", ref updateTimeTo, value); }
        }

        /// <summary>
        /// 处理状态
        /// </summary>
        private ECCentral.BizEntity.MKT.CommentProcessStatus? status;
        public ECCentral.BizEntity.MKT.CommentProcessStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 超期时间状态
        /// </summary>
        private ECCentral.BizEntity.MKT.OverTimeStatus? overTimeStatus;
        public ECCentral.BizEntity.MKT.OverTimeStatus? OverTimeStatus
        {
            get { return overTimeStatus; }
            set { base.SetValue("OverTimeStatus", ref overTimeStatus, value); }
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
