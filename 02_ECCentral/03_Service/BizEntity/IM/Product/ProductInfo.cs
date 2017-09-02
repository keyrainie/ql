using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class ProductInfo : ProductCommonInfo, IWebChannel, IMarketingPlace
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 源商品编号
        /// </summary>
        [DataMember]
        public string SourceProductID { get; set; }

        /// <summary>
        /// 商品促销标题
        /// </summary>
        [DataMember]
        public LanguageContent PromotionTitle { get; set; }

        /// <summary>
        /// 商品时效性促销语
        /// </summary>
        [DataMember]
        public IList<ProductTimelyPromotionTitle> ProductTimelyPromotionTitle { get; set; }

        /// <summary>
        /// 商品促销类型
        /// </summary>
        [DataMember]
        public String PromotionType { get; set; }

        /// <summary>
        /// 商品全称
        /// </summary>
        [DataMember]
        public String ProductName
        {
            get { return (ProductBasicInfo == null || ProductBasicInfo.ProductTitle == null ? string.Empty : ProductBasicInfo.ProductTitle.Content) + (PromotionTitle == null ? string.Empty : PromotionTitle.Content); }
            set { }
        }

        /// <summary>
        /// 商品价格信息
        /// </summary>
        [DataMember]
        public ProductPriceInfo ProductPriceInfo { get; set; }

        /// <summary>
        /// 价格更改申请
        /// </summary>
        [DataMember]
        public ProductPriceRequestInfo ProductPriceRequest { get; set; }

        /// <summary>
        /// 商品代销属性
        /// </summary>
        [DataMember]
        public VendorConsignFlag ProductConsignFlag { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        [DataMember]
        public ProductStatus ProductStatus { get; set; }

        /// <summary>
        /// 商品销售区域
        /// </summary>
        [DataMember]
        public IList<ProductSalesAreaInfo> ProductSalesAreaInfoList { get; set; }

        /// <summary>
        /// 商品附件
        /// </summary>
        [DataMember]
        public List<ProductAttachmentInfo> ProductAttachmentList { get; set; }

        /// <summary>
        /// 商品采购信息
        /// </summary>
        [DataMember]
        public ProductPOInfo ProductPOInfo { get; set; }

        /// <summary>
        /// 商品批次管理信息
        /// </summary>
        [DataMember]
        public ProductBatchManagementInfo ProductBatchManagementInfo { get; set; }

        /// <summary>
        /// 商品付款类型
        /// </summary>
        [DataMember]
        public ProductPayType ProductPayType { get; set; }

        /// <summary>
        /// 渠道信息
        /// </summary>
        [DataMember]
        public WebChannel WebChannel { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 商家信息
        /// </summary>
        [DataMember]
        public Merchant Merchant { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 库存模式
        /// </summary>
        [DataMember]
        public ProductInventoryType InventoryType { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        [DataMember]
        public string OrginCode { get; set; }

        
    }
}
