using System;
using ECommerce.Utility;
using ECommerce.Entity.Member;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using System.Collections.Generic;

namespace ECommerce.Entity.Product
{
    public class MatchedTradingInfo
    {
        public MatchedTradingInfo()
        {
            CustomerInfo = new CustomerInfo();
            CustomerExtendInfo = new CustomerExtendInfo();
            GroupPropertyInfo = new GroupPropertyInfo();
            VendorInfo = new VendorInfo();
            ReplyList = new List<ProductMatchedTradingReplyInfo>();
            PagedReplyList = new PagedResult<ProductMatchedTradingReplyInfo>();
            AccountReplyList = new List<ProductMatchedTradingReplyInfo>();
        }

        public int SysNo { get; set; }
        public int ProductSysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int ReplyCount { get; set; }
        public DateTime InDate { get; set; }
        public DateTime EditDate { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public ProductStatus productStatus { get; set; }
        public string Status { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CashRebate { get; set; }
        public string ImageVersion { get; set; }
        public string DefaultImage { get; set; }
        public string ProductTitle { get; set; }
        public string ProductPromotionTitle { get; set; }
        /// <summary>
        /// 用户基本信息
        /// </summary>

        public CustomerInfo CustomerInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户扩展信息
        /// </summary>

        public CustomerExtendInfo CustomerExtendInfo
        {
            get;
            set;
        }



        public ProductMatchedTradingReplyInfo NeweggReply { get; set; }
        public ProductMatchedTradingReplyInfo ManufactureReply { get; set; }
        //用于分页绑定
        public PagedResult<ProductMatchedTradingReplyInfo> PagedReplyList { get; set; }
        //用于不分页绑定
        public List<ProductMatchedTradingReplyInfo> AccountReplyList { get; set; }
        public GroupPropertyInfo GroupPropertyInfo { get; set; }
        public VendorInfo VendorInfo { get; set; }
        public ProductPayType pointType { get; set; }

        /// <summary>
        /// 商品详情页专用
        /// </summary>
        public List<ProductMatchedTradingReplyInfo> ReplyList { get; set; }
    }
}
