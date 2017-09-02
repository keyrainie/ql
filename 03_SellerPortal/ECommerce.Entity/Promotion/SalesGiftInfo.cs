using System;
using System.Collections.Generic;
using ECommerce.Enums;
using ECommerce.WebFramework;

namespace ECommerce.Entity.Promotion
{
    public class SalesGiftInfo : EntityBase
    {
        public int? SysNo { get; set; }
        public string Title { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? OrderMinAmount { get; set; }
        public string Description { get; set; }
        public string PromotionLink { get; set; }
        public string Memo { get; set; }
        public SaleGiftStatus? Status { get; set; }
        public SaleGiftType? Type { get; set; }
        public string StatusText
        {
            get
            {
                if (Status.HasValue)
                {
                    return Status.Value.GetEnumDescription();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public bool? IsGlobalProduct { get; set; }
        public int? ItemGiftCount { get; set; }
        public SaleGiftDiscountBelongType? DisCountType { get; set; }
        //主商品列表:
        public List<SalesGiftMainProductRuleInfo> ProductRuleList { get; set; }
        //赠品列表:
        public List<SalesGiftProductRuleInfo> GiftRuleList { get; set; }


        /// <summary>
        /// 审核人 
        /// </summary>
        public string AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }


        /// <summary>
        /// 最后一次编辑用户
        /// </summary>
        public string EditUser { get; set; }

        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
        public int? VendorType { get; set; }


    }

    public class SalesGiftProductRuleInfo
    {
        public int? SysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public ProductStatus? ProductStatus { get; set; }
        public int? PromotionSysNo { get; set; }
        public int? Count { get; set; }
        public int? Priority { get; set; }
        public decimal? PlusPrice { get; set; }
        public int? VendorSysNo { get; set; }
    }
    public class SalesGiftMainProductRuleInfo
    {


        public int? SysNo { get; set; }
        public int? PromotionSysNo { get; set; }
        public string Type { get; set; }
        public int? C3SysNo { get; set; }
        public int? BrandSysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public ProductStatus? ProductStatus { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int? BuyCount { get; set; }
        public AndOrType? ComboType { get; set; }
    }
}
