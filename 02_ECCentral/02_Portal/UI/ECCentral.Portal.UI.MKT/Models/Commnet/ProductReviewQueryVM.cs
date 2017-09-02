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
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductReviewQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// cs deal with number
        /// </summary>
        public int? ComplainSysNo { get; set; }

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
        /// 操作符
        /// </summary>
        private string operation;
        public string Operation
        {
            get { return operation; }
            set { base.SetValue("Operation", ref operation, value); }
        }
        /// <summary>
        /// 蛋数
        /// </summary>
        //private string score;
        //public string Score
        //{
        //    get { return score; }
        //    set { base.SetValue("Score", ref score, value); }
        //}

        /// <summary>
        /// 订单编号    提交CS处理需要SONo
        /// </summary>
        public int? SOSysno { get; set; }

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
        /// 用户提问内容
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set { base.SetValue("Title", ref title, value); }
        }

        /// <summary>
        /// 优点
        /// </summary>
        private string prons;
        public string Prons
        {
            get { return prons; }
            set { base.SetValue("Prons", ref prons, value); }
        }

        /// <summary>
        /// 缺点
        /// </summary>
        private string cons;
        public string Cons
        {
            get { return cons; }
            set { base.SetValue("Cons", ref cons, value); }
        }

        /// <summary>
        /// 服务质量
        /// </summary>
        private string service;
        public string Service
        {
            get { return service; }
            set { base.SetValue("Service", ref service, value); }
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


        /// <summary>
        /// 是否上传图片
        /// </summary>
        private YNStatus? image;
        public YNStatus? Image
        {
            get { return image; }
            set { base.SetValue("Image", ref image, value); }
        }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 是否精华
        /// </summary>
        private YNStatus? isDigest;
        public YNStatus? IsDigest
        {
            get { return isDigest; }
            set { base.SetValue("IsDigest", ref isDigest, value); }
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
        /// 是否置底
        /// </summary>
        private YNStatus? isBottom;
        public YNStatus? IsBottom
        {
            get { return isBottom; }
            set { base.SetValue("IsBottom", ref isBottom, value); }
        }

        /// <summary>
        /// 最有用
        /// </summary>
        private YNStatus? mostUseFul;
        public YNStatus? MostUseFul
        {
            get { return mostUseFul; }
            set { base.SetValue("MostUseFul", ref mostUseFul, value); }
        }

        /// <summary>
        /// 最有用候选
        /// </summary>
        private YNStatus? mostUseFulCandidate;
        public YNStatus? MostUseFulCandidate
        {
            get { return mostUseFulCandidate; }
            set { base.SetValue("MostUseFulCandidate", ref mostUseFulCandidate, value); }
        }

        /// <summary>
        /// 首页服务热评
        /// </summary>
        private YNStatus? isServiceHotReview;
        public YNStatus? IsServiceHotReview
        {
            get { return isServiceHotReview; }
            set { base.SetValue("IsServiceHotReview", ref isServiceHotReview, value); }
        }

        /// <summary>
        /// 首页热评
        /// </summary>
        private YNStatus? isIndexHotReview;
        public YNStatus? IsIndexHotReview
        {
            get { return isIndexHotReview; }
            set { base.SetValue("IsIndexHotReview", ref isIndexHotReview, value); }
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
        private string vendorType;
        public string VendorType
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
        /// 商品状态
        /// </summary>
        private ECCentral.BizEntity.IM.ProductStatus? productStatus;
        public ECCentral.BizEntity.IM.ProductStatus? ProductStatus
        {
            get { return productStatus; }
            set { base.SetValue("ProductStatus", ref productStatus, value); }
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
        [Validate(ValidateType.Regex, @"^[0-9]{0,8}$", ErrorMessage = "编号必须是整数，且大于等于0")]
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
        /// CS处理状态
        /// </summary>
        private ReviewProcessStatus? complainStatus;
        public ReviewProcessStatus? ComplainStatus
        {
            get { return complainStatus; }
            set { base.SetValue("ComplainStatus", ref complainStatus, value); }
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

        /// <summary>
        /// 总蛋数
        /// </summary>
        private decimal? score;
        public decimal? Score
        {
            get { return score; }
            set { base.SetValue("Score", ref score, value); }
        }

        /// <summary>
        /// 商家评分总分数
        /// </summary>
        private string vendorScore;
        public string VendorScore
        {
            get { return vendorScore; }
            set { base.SetValue("VendorScore", ref vendorScore, value); }
        }

        /// <summary>
        /// 用户有用评论数
        /// </summary>
        private string usefulCount;
        [Validate(ValidateType.Regex, @"^[0-9]{0,4}$", ErrorMessage = "编号必须是整数，且大于等于0")]
        public string UsefulCount
        {
            get { return usefulCount; }
            set { base.SetValue("UsefulCount", ref usefulCount, value); }
        }

        /// <summary>
        /// 用户无用评论数
        /// </summary>
        private int? unUsefulCount;
        public int? UnUsefulCount
        {
            get { return unUsefulCount; }
            set { base.SetValue("UnUsefulCount", ref unUsefulCount, value); }
        }

        /// <summary>
        /// 回复数
        /// </summary>
        private int? replyCount;
        public int? ReplyCount
        {
            get { return replyCount; }
            set { base.SetValue("ReplyCount", ref replyCount, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string csNote;
        public string CSNote
        {
            get { return csNote; }
            set { base.SetValue("CSNote", ref csNote, value); }
        }

        public string InUser { get; set; }

        public string EditUser { get; set; }

        public DateTime? InDate { get; set; }

        public DateTime? EditDate { get; set; }

        #region 扩展checkbox

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool BIsTop
        {
            get
            {
                return IsTop == YNStatus.Yes;
            }
            set
            {
                if (value)
                    IsTop = YNStatus.Yes;
                else
                    IsTop = YNStatus.No;
            }
        }

        /// <summary>
        /// 是否置底
        /// </summary>
        public bool BIsBottom
        {
            get
            {
                return IsBottom == YNStatus.Yes;
            }
            set
            {
                if (value)
                    IsBottom = YNStatus.Yes;
                else
                    IsBottom = YNStatus.No;
            }
        }


        /// <summary>
        /// 最有用候选
        /// </summary>
        public bool BMostUseFulCandidate
        {
            get
            {
                return MostUseFulCandidate == YNStatus.Yes;
            }
            set
            {
                if (value)
                    MostUseFulCandidate = YNStatus.Yes;
                else
                    MostUseFulCandidate = YNStatus.No;
            }
        }

        /// <summary>
        /// 首页服务热评
        /// </summary>
        public bool BIsServiceHotReview
        {
            get
            {
                return IsServiceHotReview == YNStatus.Yes;
            }
            set
            {
                if (value)
                    IsServiceHotReview = YNStatus.Yes;
                else
                    IsServiceHotReview = YNStatus.No;
            }
        }

        /// <summary>
        /// 首页热评
        /// </summary>
        public bool BIsIndexHotReview
        {
            get
            {
                return IsIndexHotReview == YNStatus.Yes;
            }
            set
            {
                if (value)
                    IsIndexHotReview = YNStatus.Yes;
                else
                    IsIndexHotReview = YNStatus.No;
            }
        }
        #endregion

        /// <summary>
        /// 评论厂商回复列表
        /// </summary>
        public List<ECCentral.BizEntity.MKT.ProductReviewReply> VendorReplyList { get; set; }

        /// <summary>
        /// 评论回复列表
        /// </summary>
        public List<ECCentral.BizEntity.MKT.ProductReviewReply> ProductReviewReplyList { get; set; }

        /// <summary>
        /// 产品用户评论-邮件日志
        /// </summary>
        public ProductReviewMailLog ProductReviewMailLog { get; set; }

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
        /// 评论类型
        /// </summary>
        private ReviewType? reviewType;
        public ReviewType? ReviewType
        {
            get { return reviewType; }
            set
            {
                base.SetValue("ReviewType", ref reviewType, value);
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

        public bool HasProductReviewApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductReview_Approve); }
        }

        public bool HasProductReviewCancelPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductReview_Cancel); }
        }

        public bool HasProductReviewReadPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductReview_Read); }
        }

        public bool HasProductReviewExportPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductReview_Export); }
        }
    }

}
