using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 团购
    /// </summary>
    public class GroupBuyingInfo : IIdentity, IWebChannel, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public LanguageContent GroupBuyingTitle { get; set; }
        /// <summary>
        /// 团购简述
        /// </summary>
        public LanguageContent GroupBuyingDesc { get; set; }
        /// <summary>
        /// 团购详细描述
        /// </summary>
        public LanguageContent GroupBuyingDescLong { get; set; }

        /// <summary>
        /// 团购规则描述
        /// </summary>
        public LanguageContent GroupBuyingRules { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 团购类型编号
        /// </summary>
        public int? GroupBuyingTypeSysNo { get; set; }
        /// <summary>
        /// 地区编号
        /// </summary>
        public int? GroupBuyingAreaSysNo { get; set; }
        /// <summary>
        /// 团购类型名
        /// </summary>
        public string GroupBuyingTypeName { get; set; }
        /// <summary>
        /// 地区名
        /// </summary>
        public string GroupBuyingAreaName { get; set; }

        /// <summary>
        /// 市场价格
        /// </summary>
        public decimal? BasicPrice { get; set; }

        /// <summary>
        /// 团购活动状态
        /// </summary>
        public GroupBuyingStatus? Status { get; set; }
        /// <summary>
        /// 时间条件：起始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }
        /// <summary>
        /// 时间条件：结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 团购商品促销图,大图,需要多语言
        /// </summary>
        public LanguageContent GroupBuyingPicUrl { get; set; }

        /// <summary>
        /// 团购商品促销图,中图,需要多语言
        /// </summary>
        public LanguageContent GroupBuyingMiddlePicUrl { get; set; }

        /// <summary>
        /// 团购商品促销图,小图,需要多语言
        /// </summary>
        public LanguageContent GroupBuyingSmallPicUrl { get; set; }
        /// <summary>
        /// 是否组团购,商品具有组的概念，比如内裤有尺码，颜色两种分组属性。
        /// 用户可以指定一个商品所属组内的所有商品都参与本次团购活动。
        /// </summary>
        public bool? IsByGroup { get; set; }
        /// <summary>
        /// 成团时间
        /// </summary>
        public DateTime? SuccessDate { get; set; }
        /// <summary>
        /// 商品原价
        /// </summary>
        public decimal? OriginalPrice { get; set; }
        /// <summary>
        /// 商品团购价格
        /// </summary>
        public decimal? GBPrice { get; set; }
        /// <summary>
        /// 当前已卖数量
        /// </summary>
        public int? CurrentSellCount { get; set; }
        /// <summary>
        /// 是否已结算
        /// </summary>
        public GroupBuyingSettlementStatus? SettlementStatus //IsSettlement
        { get; set; }
        /// <summary>
        /// 显示优先级
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 每个团购订单最小购买该商品数量
        /// </summary>
        public int? MinCountPerOrder { get; set; }
        /// <summary>
        /// 每个团购订单最大可购买该商品数量
        /// </summary>
        public int? MaxCountPerOrder { get; set; }
        /// <summary>
        /// 每个Customer ID限购次数
        /// </summary>
        public int? LimitOrderCount { get; set; }
        /// <summary>
        /// Customer 已使用的次数 
        /// </summary>
        public int? CustomerUsedFrequency { get; set; }
        /// <summary>
        /// 阶梯价格
        /// </summary>
        public List<PSPriceDiscountRule> PriceRankList { get; set; }

        /// <summary>
        /// 结算价格
        /// </summary>
        public decimal? CostAmt { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string EditUser { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser { get; set; }
        /// <summary>
        /// 审核原因
        /// </summary>
        public string Reasons { get; set; }
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 团购类型
        /// </summary>
        public Dictionary<int, String> GroupBuyingTypeList { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public Dictionary<int, String> GroupBuyingAreaList { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int? GroupBuyingPoint { get; set; }

        /// <summary>
        /// SellerPortal团购编号
        /// </summary>
        public int? RequestSysNo { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        public int GroupBuyingVendorSysNo { get; set; }
        /// <summary>
        /// 商家名
        /// </summary>
        public string GroupBuyingVendorName { get; set; }

        /// <summary>
        /// 团购类型
        /// </summary>
        public GroupBuyingCategoryType? CategoryType { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 供应商门店编号
        /// </summary>
        public List<int> VendorStoreSysNoList { get; set; }

        /// <summary>
        /// 团购券有效期
        /// </summary>
        public DateTime? CouponValidDate { get; set; }

        /// <summary>
        /// 抽奖规则
        /// </summary>
        public string LotteryRule { get; set; }

        /// <summary>
        /// 团购分类编号
        /// </summary>
        public int? GroupBuyingCategorySysNo { get; set; }

        /// <summary>
        /// 是否预约
        /// </summary>
        public bool IsWithoutReservation { get; set; }

        /// <summary>
        /// 是否代金券
        /// </summary>
        public bool IsVouchers { get; set; }
    }
}
