using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 渠道商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductChannelInfo : IIdentity
    {

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 渠道信息
        /// </summary>
        [DataMember]
        public ChannelInfo ChannelInfo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }

        /// <summary>
        /// 渠道商品ID
        /// </summary>
        [DataMember]
        public string SynProductID { get; set; }

        /// <summary>
        /// 淘宝Sku号
        /// </summary>
        [DataMember]
        public string TaoBaoSku { get; set; }

        /// <summary>
        /// 一致库存比例
        /// </summary>
        [DataMember]
        public int? InventoryPercent { get; set; }

        /// <summary>
        /// 渠道价格比例
        /// </summary>
        [DataMember]
        public int? ChannelPricePercent { get; set; }        

        /// <summary>
        /// 可销售数量
        /// </summary>
        [DataMember]
        public int? ChannelSellCount { get; set; }

        /// <summary>
        /// 安全库存
        /// </summary>
        [DataMember]
        public int? SafeInventoryQty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductChannelInfoStatus Status { get; set; }

        /// <summary>
        /// 是否同步促销
        /// </summary>
        [DataMember]
        public BooleanEnum IsUsePromotionPrice { get; set; }

        /// <summary>
        /// 是否指定库存
        /// </summary>
        [DataMember]
        public BooleanEnum IsAppointInventory { get; set; }

        /// <summary>
        /// 是否清库
        /// </summary>
        [DataMember]
        public BooleanEnum IsClearInventory { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 最大分仓数
        /// </summary>
        [DataMember]
        public int? MaxStockQty { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [DataMember]
        public int? OnlineQty { get; set; }


        [DataMember]
        public List<int> SysNoList { set; get; } 
    }

    /// <summary>
    /// 渠道信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ChannelInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 订单渠道编号
        /// </summary>
        [DataMember]
        public string SOChannelCode { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        [DataMember]
        public int ChannelType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 是否泰隆优选退货
        /// </summary>
        [DataMember]
        public BooleanEnum IsRMAByNewegg { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

    }

    /// <summary>
    /// 商品渠道时段价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductChannelPeriodPrice : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品渠道信息
        /// </summary>
        [DataMember]
        public ProductChannelInfo ChannelProductInfo { get; set; }

        /// <summary>
        /// 时段价格
        /// </summary>
        [DataMember]
        public decimal PeriodPrice { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 活动说明
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductChannelPeriodPriceStatus Status { get; set; }

        /// <summary>
        /// 是否更改价格
        /// </summary>
        [DataMember]
        public BooleanEnum IsChangePrice { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        [DataMember]
        public ProductChannelPeriodPriceOperate Operate { get; set; }     

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public UserInfo AuditUser { get; set; }
    }
}
