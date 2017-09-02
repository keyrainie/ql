using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductPriceRequestVM : ProductPriceInfoVM
    {

        
        
        /// <summary>
        /// 拒绝理由
        /// </summary>
        public string DenyReason { get; set; }

        /// <summary>
        /// PM申请理由
        /// </summary>
        public string PMMemo { get; set; }

        /// <summary>
        /// TL审核理由
        /// </summary>
        public string TLMemo { get; set; }

        /// <summary>
        /// PMD审核理由
        /// </summary>
        public string PMDMemo { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ProductPriceRequestStatus? RequestStatus { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        public ProductPriceRequestAuditType? AuditType { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo FinalAuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? FinalAuditTime
        {
            get;
            set;
        }

        /// <summary>
        ///旧价格单据
        /// </summary>
        public ProductPriceInfoVM OldPrice { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int? AvailableQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        public int? ConsignQty { get; set; }

        /// <summary>
        /// 最后一次采购日期
        /// </summary>
        public DateTime? LastInTime { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public int? SysNo { get; set; }

        ///// <summary>
        ///// 三级类
        ///// </summary>
      public CategoryInfo Category { get; set; }

        /// <summary>
        /// 最后一次更新日期
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        public bool HasAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && (AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_PrimaryAuditPrice) ||
                       AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_AdvancedAuditPrice));
            }
        }

        public bool HasQuickApprovePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemQuickApprove);
            }
        }

        /// <summary>
        /// 初级审核权限
        /// </summary>
        public bool HasPrimaryAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_PrimaryAuditPrice);
            }
        }

        /// <summary>
        /// 高级审核权限
        /// </summary>
        public bool HasAdvancedAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_AdvancedAuditPrice);
            }
        }
    }

    public class ProductPriceInfoVM : ModelBase
    {
        /// <summary>
        /// 商品市场价
        /// </summary>
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 商家售价
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 商品返现
        /// </summary>
        public decimal? CashRebate { get; set; }

        /// <summary>
        /// 商品返点
        /// </summary>
        public int? Point { get; set; }

        /// <summary>
        /// 商品折扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 商品成本
        /// </summary>
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 商品去税成本
        /// </summary>
        public decimal UnitCostWithoutTax { get; set; }

        /// <summary>
        /// 商品正常采购价格
        /// </summary>
        public decimal VirtualPrice { get; set; }

        /// <summary>
        /// 批发价格
        /// </summary>
        public List<ProductWholeSalePriceInfo> ProductWholeSalePriceInfo { get; set; }

        /// <summary>
        /// 会员价格
        /// </summary>
        public List<ProductRankPriceInfo> ProductRankPrice { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        public ProductPayType PayType { get; set; }

        /// <summary>
        /// 每单最小订购数量
        /// </summary>
        public int MinCountPerOrder { get; set; }

        /// <summary>
        /// 每天最大订购数量
        /// </summary>
        public int MaxCountPerDay { get; set; }

        /// <summary>
        /// 是否使用AlipayVIP价格
        /// </summary>
        public IsUseAlipayVipPrice? IsUseAlipayVipPrice { get; set; }

        /// <summary>
        /// AlipayVIP价格
        /// </summary>
        public decimal? AlipayVipPrice { get; set; }

        /// <summary>
        /// 京东价格
        /// </summary>
        public decimal? JDPrice { get; set; }

        /// <summary>
        /// 亚马逊价格
        /// </summary>
        public decimal? AMPrice { get; set; }

        public int IsWholeSale { get; set; }

        public int IsExistRankPrice { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 含优惠券、赠品、随心配毛利率
        /// </summary>
        public decimal NewMargin { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        public decimal MarginAmount { get; set; }

        /// <summary>
        ///  含优惠券、赠品、随心配毛利
        /// </summary>
        public decimal NewMarginAmount { get; set; }

        public int GiftSysNo { get; set; }
        public string GiftSysNoString 
        { 
            get 
            {
                string result = ResProductPriceApprove.None;
                if(GiftSysNo>0)
                {
                    result = GiftSysNo.ToString();
                }
                return result;
            } 
        }

        public int CouponSysNo { get; set; }
        public string CouponSysNoString
        {
            get
            {
                string result = ResProductPriceApprove.None;
                if (CouponSysNo > 0)
                {
                    result = CouponSysNo.ToString();
                }
                return result;
            }
        }
    }
}
