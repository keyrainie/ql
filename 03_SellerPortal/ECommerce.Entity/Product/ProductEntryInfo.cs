using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.WebFramework;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品备案信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductEntryInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品组编号
        /// </summary>
        [DataMember]
        public int ProductGroupSysNo { get; set; }
        /// <summary>
        /// 商品英文名称
        /// </summary>
        [DataMember]
        public string ProductNameEN { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [DataMember]
        public string Specifications { get; set; }
        /// <summary>
        /// 功能
        /// </summary>
        [DataMember]
        public string Functions { get; set; }
        /// <summary>
        /// 成份
        /// </summary>
        [DataMember]
        public string Component { get; set; }
        /// <summary>
        /// 产地编码
        /// </summary>
        [DataMember]
        public string Origin { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        public string Purpose { get; set; }
        /// <summary>
        /// 计税单位数量
        /// </summary>
        [DataMember]
        public decimal TaxQty { get; set; }
        /// <summary>
        /// 计税单位
        /// </summary>
        [DataMember]
        public string TaxUnit { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        [DataMember]
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        [DataMember]
        public decimal GrossWeight { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        public ProductBizType BizType { get; set; }
        /// <summary>
        /// 申报关区
        /// </summary>
        [DataMember]
        public int ApplyDistrict { get; set; }
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
        /// 申报数量
        /// </summary>
        [DataMember]
        public int ApplyQty { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        [DataMember]
        public decimal SuttleWeight { get; set; }
        /// <summary>
        /// 其他备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }
        /// <summary>
        /// 物资序号2
        /// </summary>
        [DataMember]
        public string SuppliesSerialNo1 { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 税则号
        /// </summary>
        [DataMember]
        public string TariffCode { get; set; }
        /// <summary>
        /// 运输存储方式
        /// </summary>
        [DataMember]
        public ProductStoreType StoreType { get; set; }
        /// <summary>
        /// 备案号
        /// </summary>
        [DataMember]
        public string EntryCode { get; set; }
        /// <summary>
        /// 进口检疫审批许可证确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark1 { get; set; }
        /// <summary>
        /// 输出国或地区官方出具检疫证书确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark2 { get; set; }
        /// <summary>
        /// 原产地证明确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark3 { get; set; }
        /// <summary>
        /// 品牌方授权确认（直邮）
        /// </summary>
        [DataMember]
        public string Remark4 { get; set; }
        /// <summary>
        /// 商品其他名称
        /// </summary>
        [DataMember]
        public string ProductOthterName { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public string ManufactureDate { get; set; }
        /// <summary>
        /// 来源公司，无效，默认0
        /// </summary>
        [DataMember]
        public int SourceCompany { get; set; }
        [DataMember]
        public string BMCode { get; set; }
        [DataMember]
        public string UPCCode { get; set; }
        [DataMember]
        public int DefaultLeadTimeDays { get; set; }
        [DataMember]
        public int? RealLeadTimeDays { get; set; }
        /// <summary>
        /// 报关状态
        /// </summary>
        [DataMember]
        public ProductEntryStatus EntryStatus { get; set; }
        [DataMember]
        public string EntryStatusText { get { return this.EntryStatus.GetEnumDescription(); } }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int BrandSysNo { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 产地名称
        /// </summary>
        [DataMember]
        public string CountryName { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        [DataMember]
        public string AuditNote { get; set; }
    }
}
