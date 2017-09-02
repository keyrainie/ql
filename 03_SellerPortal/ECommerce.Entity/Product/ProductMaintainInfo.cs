using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECommerce.Enums;
using ECommerce.WebFramework;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品维护信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductMaintainInfo : EntityBase
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        [DataMember]
        public ProductStatus Status { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        [DataMember]
        public string ProductMode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public string CreateTime { get; set; }
        /// <summary>
        /// 促销语
        /// </summary>
        [DataMember]
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 时效促销语
        /// </summary>
        [DataMember]
        public string TimePromotionTitle { get; set; }
        /// <summary>
        /// 时效促销语开始时间
        /// </summary>
        [DataMember]
        public string PromotionTitleBeginDate { get; set; }
        /// <summary>
        /// 时效促销语结束时间
        /// </summary>
        [DataMember]
        public string PromotionTitleEndDate { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        [DataMember]
        public decimal Length { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        [DataMember]
        public decimal Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [DataMember]
        public decimal Height { get; set; }
        /// <summary>
        /// 是否有时效促销语，0-无；1-有
        /// </summary>
        [DataMember]
        public int HaveTimePromotionTitle { get; set; }
        /// <summary>
        /// 备案状态
        /// </summary>
        [DataMember]
        public ProductEntryStatus EntryStatus { get; set; }
        [DataMember]
        public string EntryStatusText
        {
            get
            {
                return this.EntryStatus.GetEnumDescription();
            }
        }

        /// <summary>
        /// 售价
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 导购链接
        /// </summary>
        [DataMember]
        public string ProductOutUrl { get; set; }
        /// <summary>
        /// 商家类型（GUD-导购商家）
        /// </summary>
        [DataMember]
        public string MerchantInvoiceType { get; set; }
    }
}
