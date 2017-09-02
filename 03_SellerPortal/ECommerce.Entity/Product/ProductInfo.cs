using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductInfo : EntityBase
    {
        public int SysNo { get; set; }
        public string CouponName { get; set; }
        public string CouponDesc { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int MerchantSysNo { get; set; }
        public string CreateTimeString
        {
            get
            {
                return InDate.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }
    }

    /// <summary>
    /// 商品上下架信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductOnSaleInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品默认图片
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }
        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int BrandSysNo { get; set; }
        /// <summary>
        /// 品牌状态， A有效
        /// </summary>
        [DataMember]
        public string BrandStatus { get; set; }
        /// <summary>
        /// C3
        /// </summary>
        [DataMember]
        public int C3SysNo { get; set; }
        /// <summary>
        /// C3状态，1有效
        /// </summary>
        [DataMember]
        public int CategoryStatus { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public string ProductDesc { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        [DataMember]
        public string ProductDescLong { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal? BasicPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember]
        public decimal? CurrentPrice { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        [DataMember]
        public string Performance { get; set; }
        /// <summary>
        /// 报关状态
        /// </summary>
        [DataMember]
        public ProductEntryStatus EntryStatus { get; set; }
        /// <summary>
        /// 税则号
        /// </summary>
        [DataMember]
        public string TariffCode { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 备案号
        /// </summary>
        [DataMember]
        public string EntryCode { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        public ProductBizType BizType { get; set; }
        /// <summary>
        /// 货号
        /// </summary>
        [DataMember]
        public string ProductSKUNO { get; set; }
        /// <summary>
        /// 物资序号
        /// </summary>
        [DataMember]
        public string SuppliesSerialNo { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        [DataMember]
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 申报数量
        /// </summary>
        [DataMember]
        public int ApplyQty { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        [DataMember]
        public decimal GrossWeight { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        [DataMember]
        public decimal SuttleWeight { get; set; }
        /// <summary>
        /// 商品交易类型
        /// </summary>
        [DataMember]
        public TradeType ProductTradeType { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus Status { get; set; }
    }
}
