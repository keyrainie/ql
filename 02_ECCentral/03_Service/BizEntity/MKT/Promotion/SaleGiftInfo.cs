using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 赠品促销
    /// </summary>
    public class SaleGiftInfo : IIdentity, IWebChannel, ICompany
    {

        #region 基础
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public LanguageContent Title { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public LanguageContent Description { get; set; }

        /// <summary>
        /// 折扣计入方式 
        /// </summary>
        public SaleGiftDiscountBelongType? DisCountType { get; set; }
               

        /// <summary>
        /// 活动链接
        /// </summary>
        public string PromotionLink { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SaleGiftStatus? Status { get; set; }

        /// <summary>
        /// 赠品类型
        /// </summary>
        public SaleGiftType? Type { get; set; }


        /// <summary>
        /// 审核人 
        /// </summary>
        public string AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 最后一次编辑用户
        /// </summary>
        public string EditUser { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public WebChannel WebChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        #endregion

        

        /// <summary>
        /// 指定总数量，如果指定总数量有值并大于0时，则每个赠品商品数量不会使用
        /// </summary>
        public int? ItemGiftCount { get; set; }

        /// <summary>
        /// Y 全网       N 非全网
        /// </summary>
        public bool? IsGlobalProduct { get; set; }

        /// <summary>
        /// O 赠品池        A 非赠品池
        /// </summary>
        public SaleGiftGiftItemType? GiftComboType { get; set; }

        /// <summary>
        /// 赠品
        /// </summary>
        public List<RelProductAndQty> GiftItemList { get; set; }
        
        
        #region 条件
        /// <summary>
        /// 时间条件：起始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }
        /// <summary>
        /// 时间条件：结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 促销活动商品条件C3，Brand,Product列表
        /// </summary>
        public List<SaleGift_RuleSetting> ProductCondition { get; set; }

        /// <summary>
        /// 订单条件
        /// </summary>
        public PSOrderCondition OrderCondition { get; set; }

        #endregion

        #region 库存高级权限字段
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool IsAccess { get; set; }

        #endregion

        public int? RequestSysNo { get; set; }

        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }

        public int VendorType { get; set; }
    }

    /// <summary>
    /// 赠品规则
    /// </summary>
    public class SaleGift_RuleSetting
    {
        /// <summary>
        /// 促销系统编号
        /// </summary>
        public int? PromotionSysNo { get; set; }
        /// <summary>
        /// 关联的3级类别系统编号
        /// </summary>
        public SimpleObject RelC3 { get; set; }
        /// <summary>
        /// 关联品牌
        /// </summary>
        public SimpleObject RelBrand { get; set; }
        /// <summary>
        /// 关联产品
        /// </summary>
        public RelProductAndQty RelProduct { get; set; }
        /// <summary>
        /// 关联类别
        /// </summary>
        public SaleGiftSaleRuleType? Type { get; set; }
        /// <summary>
        /// 套餐类型
        /// </summary>
        public AndOrType? ComboType { get; set; }

    }

    
}
