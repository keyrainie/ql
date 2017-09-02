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
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class NewsAdvReplyVM : ModelBase
    {

        public NewsAdvReplyVM()
        {
            this.CommentCategoryList = EnumConverter.GetKeyValuePairs<NewsAdvReplyStatus>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 展示模式
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.MKT.NewsAdvReplyStatus?, string>> CommentCategoryList { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 基本后编辑时间
        /// </summary>
        private DateTime? lastEditDate;
        public DateTime? LastEditDate
        {
            get { return lastEditDate; }
            set { base.SetValue("LastEditDate", ref lastEditDate, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? createDate;
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
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
        /// 创建时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo 
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 顾客ID
        /// </summary>
        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 顾客SysNo
        /// </summary>
        private string customerSysNo;
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        /// <summary>
        /// 相关处理人员
        /// </summary>
        private int? lastEditUserSysNo;
        public int? LastEditUserSysNo
        {
            get { return lastEditUserSysNo; }
            set { base.SetValue("LastEditUserSysNo", ref lastEditUserSysNo, value); }
        }

        public string LastEditUserName { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        private string referenceType;
        public string ReferenceType
        {
            get { return referenceType; }
            set { base.SetValue("ReferenceType", ref referenceType, value); }
        }

        /// <summary>
        /// 评论类型
        /// </summary>
        private string referenceSysNo;
        public string ReferenceSysNo
        {
            get { return referenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref referenceSysNo, value); }
        }

        /// <summary>
        /// 最后修改人(name)
        /// </summary>
        private string lastEditUserID;
        public string LastEditUserID
        {
            get { return lastEditUserID; }
            set { base.SetValue("LastEditUserID", ref lastEditUserID, value); }
        }

        /// <summary>
        /// 标题关键字
        /// </summary>
        private string replyContent;
        public string ReplyContent
        {
            get { return replyContent; }
            set { base.SetValue("ReplyContent", ref replyContent, value); }
        }

        /// <summary>
        /// 后台回复
        /// </summary>
        private string answerContent;
        public string AnswerContent
        {
            get { return answerContent; }
            set { base.SetValue("AnswerContent", ref answerContent, value); }
        }

        /// <summary>
        /// 是否上传图片
        /// </summary>
        private NYNStatus? isUploadImage;
        public NYNStatus? IsUploadImage
        {
            get { return isUploadImage; }
            set { base.SetValue("IsUploadImage", ref isUploadImage, value); }
        }

        public string Image { get; set; }

        /// <summary>
        /// 大图片地址
        /// </summary>
        //private string linkImage;
        //public string LinkImage
        //{
        //    get { return linkImage; }
        //    set { base.SetValue("LinkImage", ref linkImage, value); }
        //}

        /// <summary>
        /// 是否存在回复
        /// </summary>
        private NYNStatus? replyHasReplied;
        public NYNStatus? ReplyHasReplied
        {
            get { return replyHasReplied; }
            set { base.SetValue("ReplyHasReplied", ref replyHasReplied, value); }
        }

        /// <summary>
        /// 顾客IP地址
        /// </summary>
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set { base.SetValue("IpAddress", ref ipAddress, value); }
        }

        /// <summary>
        /// 前台展示状态
        /// </summary>
        private NewsAdvReplyStatus? status;
        public NewsAdvReplyStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 公司代码
        /// </summary>
        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
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


    public class NewsAdvReplyQueryVM : ModelBase
    {

        public NewsAdvReplyQueryVM()
        {
            this.CommentCategoryList = EnumConverter.GetKeyValuePairs<NewsAdvReplyStatus>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 展示模式
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.MKT.NewsAdvReplyStatus?, string>> CommentCategoryList { get; set; }

        private string sysNo;
        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }


        /// <summary>
        /// 基本后编辑时间
        /// </summary>
        private DateTime? lastEditDate;
        public DateTime? LastEditDate
        {
            get { return lastEditDate; }
            set { base.SetValue("LastEditDate", ref lastEditDate, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? createDate;
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
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
        /// 创建时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 顾客ID
        /// </summary>
        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 顾客SysNo
        /// </summary>
        private string customerSysNo;
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        /// <summary>
        /// 相关处理人员
        /// </summary>
        private int? lastEditUserSysNo;
        public int? LastEditUserSysNo
        {
            get { return lastEditUserSysNo; }
            set { base.SetValue("LastEditUserSysNo", ref lastEditUserSysNo, value); }
        }

        public string LastEditUserName { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        private string referenceType;
        public string ReferenceType
        {
            get { return referenceType; }
            set { base.SetValue("ReferenceType", ref referenceType, value); }
        }

        /// <summary>
        /// 评论类型
        /// </summary>
        private string referenceSysNo;
        [Validate(ValidateType.Regex, @"^(?:0|(?:[1-9]\d{0,7}))?$", ErrorMessage = "编号必须是整数，且大于等于0")]
        public string ReferenceSysNo
        {
            get { return referenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref referenceSysNo, value); }
        }

        /// <summary>
        /// 最后修改人(name)
        /// </summary>
        private string lastEditUserID;
        public string LastEditUserID
        {
            get { return lastEditUserID; }
            set { base.SetValue("LastEditUserID", ref lastEditUserID, value); }
        }

        /// <summary>
        /// 标题关键字
        /// </summary>
        private string replyContent;
        public string ReplyContent
        {
            get { return replyContent; }
            set { base.SetValue("ReplyContent", ref replyContent, value); }
        }

        /// <summary>
        /// 后台回复
        /// </summary>
        private string answerContent;
        public string AnswerContent
        {
            get { return answerContent; }
            set { base.SetValue("AnswerContent", ref answerContent, value); }
        }

        /// <summary>
        /// 是否上传图片
        /// </summary>
        private NYNStatus? isUploadImage;
        public NYNStatus? IsUploadImage
        {
            get { return isUploadImage; }
            set { base.SetValue("IsUploadImage", ref isUploadImage, value); }
        }

        /// <summary>
        /// 是否存在回复
        /// </summary>
        private NYNStatus? replyHasReplied;
        public NYNStatus? ReplyHasReplied
        {
            get { return replyHasReplied; }
            set { base.SetValue("ReplyHasReplied", ref replyHasReplied, value); }
        }

        /// <summary>
        /// 顾客IP地址
        /// </summary>
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set { base.SetValue("IpAddress", ref ipAddress, value); }
        }

        /// <summary>
        /// 前台展示状态
        /// </summary>
        private NewsAdvReplyStatus? status;
        public NewsAdvReplyStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 公司代码
        /// </summary>
        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
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
