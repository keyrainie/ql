using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    public class SaleGiftBatchInfo : IIdentity, IWebChannel, ICompany
    {

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion

        #region IWebChannel Members
        /// <summary>
        /// 所属渠道
        /// </summary>
        public WebChannel WebChannel
        {
            get;
            set;
        }

        #endregion

        #region ICompany Members
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 规则名称
        /// </summary>
        public LanguageContent RuleName { get; set; }

        /// <summary>
        /// 赠品类型
        /// </summary>
        public SaleGiftType? SaleGiftType
        { get; set; }

        public SaleGiftStatus? Status
        { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public LanguageContent RuleDescription { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate
        { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate
        { get; set; }


        /// <summary>
        /// 活动链接
        /// </summary>
        public String PromotionLink
        { get; set; }

        /// <summary>
        /// 折扣记入方式
        /// </summary>
        public SaleGiftDiscountBelongType? RebateCaculateMode
        { get; set; }


        /// <summary>
        /// 是否指定赠品
        /// </summary>
        public bool IsSpecifiedGift
        { get; set; }


        /// <summary>
        /// 总数量
        /// </summary>
        public int? TotalQty
        { get; set; }

        /// <summary>
        /// 购买的商品，左边列表
        /// </summary>
        public List<ProductItemInfo> ProductItems1
        { get; set; }


        /// <summary>
        /// 购买的商品，右边列表
        /// </summary>
        public List<ProductItemInfo> ProductItems2
        { get; set; }


        /// <summary>
        /// 赠品
        /// </summary>
        public List<ProductItemInfo> Gifts
        { get; set; }


        /// <summary>
        /// 组合类型
        /// </summary>
        public SaleGiftCombineType? CombineType
        { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        public int VendorSysNo { get; set; }
        public string VendorName { get; set; }
    }
}
