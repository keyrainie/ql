using System;
using System.Runtime.Serialization;

using ECommerce.Enums;

namespace ECommerce.Entity.Promotion.GroupBuying
{
    /// <summary>
    /// 团购详情
    /// </summary>
    [Serializable]
    [DataContract]
    public class GroupBuyingInfo
    {
        /// <summary>
        /// 团购编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 团购标题
        /// </summary>
        [DataMember]
        public string GroupBuyingTitle { get; set; }
        /// <summary>
        /// 团购简述
        /// </summary>
        [DataMember]
        public string GroupBuyingDesc { get; set; }
        /// <summary>
        /// 团购大图
        /// </summary>
        [DataMember]
        public string GroupBuyingPicUrl { get; set; }
        /// <summary>
        /// 团购小图
        /// </summary>
        [DataMember]
        public string GroupBuyingSmallPicUrl { get; set; }
        /// <summary>
        /// 团购中图
        /// </summary>
        [DataMember]
        public string GroupBuyingMiddlePicUrl { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 团购结束时间
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 每单限购
        /// </summary>
        [DataMember]
        public int MaxPerOrder { get; set; }
        /// <summary>
        /// 当前售出数量
        /// </summary>
        [DataMember]
        public int CurrentSellCount { get; set; }
        /// <summary>
        /// 团购类型编号
        /// </summary>
        [DataMember]
        public int GroupBuyingTypeSysNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public GroupBuyingStatus Status
        {
            get
            {
                GroupBuyingStatus status = GroupBuyingStatus.CheckPending;
                switch (this.SourceStatus)
                {
                    case "O":
                        status = GroupBuyingStatus.CheckPending;
                        break;
                    case "P":
                        status = GroupBuyingStatus.Ready;
                        break;
                    case "A":
                        status = GroupBuyingStatus.Running;
                        break;
                    case "F":
                        status = GroupBuyingStatus.Finish;
                        break;
                    case "D":
                        status = GroupBuyingStatus.Voided;
                        break;
                }
                return status;
            }
        }
        [DataMember]
        public string SourceStatus { get; set; }
        /// <summary>
        /// 团购规则描述
        /// </summary>
        [DataMember]
        public string GroupBuyingRules { get; set; }
        /// <summary>
        /// 团购详细描述
        /// </summary>
        [DataMember]
        public string GroupBuyingDescLong { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品组编号
        /// </summary>
        [DataMember]
        public int ProductGroupSysno { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 当前价
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int OnlineQty { get; set; }
        /// <summary>
        /// 关税税率
        /// </summary>
        [DataMember]
        public decimal TaxRate { get; set; }
        /// <summary>
        /// 商品型号
        /// </summary>
        [DataMember]
        public string ProductMode { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        [DataMember]
        public string CountryName { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public string ProductDescLong { get; set; }
        /// <summary>
        /// 图片描述
        /// </summary>
        [DataMember]
        public string ProductPhotoDesc { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        [DataMember]
        public string Performance { get; set; }
        /// <summary>
        /// 快照价
        /// </summary>
        [DataMember]
        public decimal SnapShotCurrentPrice { get; set; }
        /// <summary> 
        /// 商品原返现金额 
        /// </summary> 
        [DataMember]
        public decimal SnapShotCashRebate { get; set; }
        /// <summary> 
        /// 商品原赠送积分 
        /// </summary> 
        [DataMember]
        public decimal SnapShotPoint { get; set; }
        /// <summary> 
        /// 商品原进口关税 
        /// </summary> 
        [DataMember]
        public decimal SnapShotTariffPrice { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public int SellerSysNo { get; set; }


        public decimal RealPrice
        {
            get
            {
                var taxFee = CurrentPrice * TaxRate;
                if (taxFee <= 50m)
                {
                    taxFee = 0;
                }
                return CurrentPrice + taxFee;
            }
        }
    }
}
