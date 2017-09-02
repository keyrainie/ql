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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductDiscussReplyQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 讨论系统编号
        /// </summary>
        private int? discussSysNo;
        public int? DiscussSysNo
        {
            get { return discussSysNo; }
            set { base.SetValue("DiscussSysNo", ref discussSysNo, value); }
        }

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
        /// 创建时间开始
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 更新时间开始
        /// </summary>
        private DateTime? editDateFrom;
        public DateTime? EditDateFrom
        {
            get { return editDateFrom; }
            set { base.SetValue("EditDateFrom", ref editDateFrom, value); }
        }

        /// <summary>
        /// 更新时间结束
        /// </summary>
        private DateTime? editDateTo;
        public DateTime? EditDateTo
        {
            get { return editDateTo; }
            set { base.SetValue("EditDateTo", ref editDateTo, value); }
        }

        /// <summary>
        /// 用户提问内容
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set { base.SetValue("Title", ref title, value); }
        }

        /// <summary>
        /// 回复的内容
        /// </summary>
        private string content;
        public string Content
        {
            get { return content; }
            set { base.SetValue("Content", ref content, value); }
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
        /// 客户编号
        /// </summary>
        private string customerSysNo;
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        /// <summary>
        /// PM
        /// </summary>
        private int? pmUserSysNo;
        public int? PMUserSysNo
        {
            get { return pmUserSysNo; }
            set { base.SetValue("PMUserSysNo", ref pmUserSysNo, value); }
        }

        private YNStatus needAdditionalText;
        public YNStatus NeedAdditionalText
        {
            get { return needAdditionalText; }
            set { base.SetValue("NeedAdditionalText", ref needAdditionalText, value); }
        }

        /// <summary>
        /// 所属商家
        /// </summary>
        private string vendorName;
        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }

        /// <summary>
        /// 商家类型
        /// </summary>
        private int? vendorType;
        public int? VendorType
        {
            get { return vendorType; }
            set { base.SetValue("VendorType", ref vendorType, value); }
        }

        /// <summary>
        /// 商品
        /// </summary>
        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }

        /// <summary>
        /// ProductID
        /// </summary>
        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }


        /// <summary>
        /// 按组查询 
        /// </summary>
        private bool isByGroup;
        public bool IsByGroup
        {
            get { return isByGroup; }
            set { base.SetValue("IsByGroup", ref isByGroup, value); }
        }

        /// <summary>
        /// 商品组编号
        /// </summary>
        private string productGroupNo;
        [Validate(ValidateType.Regex, @"^[0-9]{0,8}$", ErrorMessage = "商品组编号必须是整数，且大于等于0")]
        public string ProductGroupNo
        {
            get { return productGroupNo; }
            set { base.SetValue("ProductGroupNo", ref productGroupNo, value); }
        }

        /// <summary>
        ///评论状态 
        /// </summary>
        private string status;
        public string Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        /// <summary>
        /// 顾客类型
        /// </summary>
        private int? customerCategory;
        public int? CustomerCategory
        {
            get { return customerCategory; }
            set { base.SetValue("CustomerCategory", ref customerCategory, value); }
        }


        private ECCentral.BizEntity.IM.ProductStatus? productStatus;
        public ECCentral.BizEntity.IM.ProductStatus? ProductStatus
        {
            get { return productStatus; }
            set { base.SetValue("ProductStatus", ref productStatus, value); }
        }



        public string InUser { get; set; }

        public string EditUser { get; set; }

        public DateTime? InDate { get; set; }

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

        public bool HasProductDiscussReplyApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductDiscuss_ReplyApprove); }
        }

        public bool HasProductDiscussReplyCancelPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductDiscuss_ReplyCancel); }
        }

        public bool HasProductDiscussReplyReadPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductDiscuss_ReplyRead); }
        }

        public bool HasProductDiscussExportPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductDiscuss_Export); }
        }
    }
}
