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

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductConsultReplyQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 咨询编号
        /// </summary>
        private int? consultSysNo;
        public int? ConsultSysNo
        {
            get { return consultSysNo; }
            set { base.SetValue("ConsultSysNo", ref consultSysNo, value); }
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
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        /// <summary>
        /// 回复内容
        /// </summary>
        private string replyContent;
        public string ReplyContent
        {
            get { return replyContent; }
            set { base.SetValue("ReplyContent", ref replyContent, value); }
        }

        /// <summary>
        /// 主题
        /// </summary>
        private string topicContent;
        public string TopicContent
        {
            get { return topicContent; }
            set { base.SetValue("TopicContent", ref topicContent, value); }
        }

        /// <summary>
        /// 回复人类型，是否是vendor 
        /// </summary>
        private string type;
        public string Type
        {
            get { return type; }
            set { base.SetValue("Type", ref type, value); }
        }

        public ReplyVendor TypeDescription
        {
            get
            {
                if (Type == "M")//先写死了
                    return ReplyVendor.YES;
                else
                    return ReplyVendor.NO;
            }
        }

        /// <summary>
        /// 是否置顶
        /// </summary>
        private YNStatus? isTop;
        public YNStatus? IsTop
        {
            get { return isTop; }
            set { base.SetValue("IsTop", ref isTop, value); }
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        private ECCentral.BizEntity.IM.ProductStatus? productStatus;
        public ECCentral.BizEntity.IM.ProductStatus? ProductStatus
        {
            get { return productStatus; }
            set { base.SetValue("ProductStatus", ref productStatus, value); }
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

        /// <summary>
        ///可能是回复数量，用来判断是否重复
        /// </summary>
        private int? replyCount;
        public int? ReplyCount
        {
            get { return replyCount; }
            set { base.SetValue("ReplyCount", ref replyCount, value); }
        }

        /// <summary>
        /// 咨询状态，也可用于回复状态，但需要去掉已经回复R状态
        /// </summary>
        private string status;
        public string Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 是否重复
        /// </summary>
        //public string CountNum { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public string VendorName { get; set; }

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
        //private string productName;
        //public string ProductName
        //{
        //    get { return productName; }
        //    set { base.SetValue("ProductName", ref productName, value); }
        //}

        /// <summary>
        /// 商品编号
        /// </summary>
        //private int? productSysNo;
        //public int? ProductSysNo
        //{
        //    get { return productSysNo; }
        //    set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        //}

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
        public string ProductGroupNo
        {
            get { return productGroupNo; }
            set { base.SetValue("ProductGroupNo", ref productGroupNo, value); }
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

        public bool HasProductConsultReplyBatchApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductConsult_ReplyBatchApprove); }
        }

        public bool HasProductConsultReplyBatchCancelPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductConsult_ReplyBatchCancel); }
        }

        public bool HasProductConsultReplyBatchReadPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductConsult_ReplyBatchRead); }
        }
    }
}

