using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM.Product
{
    [Serializable]
    [DataContract]
    public class ProductEntryInfo
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品英文名称
        /// </summary>
        [DataMember]
        public string ProductName_EN { get; set; }

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
        /// 产地
        /// </summary>
        [DataMember]
        public string Origin { get; set; }


        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        public string Purpose { get; set; }

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
        /// 计税单位数量
        /// </summary>
        [DataMember]
        public decimal? TaxQty { get; set; }


        /// <summary>
        /// 毛重
        /// </summary>
        [DataMember]
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        public EntryBizType? BizType { get; set; }


        /// <summary>
        /// 申报关区
        /// </summary>
        [DataMember]
        public int? ApplyDistrict { get; set; }

        /// <summary>
        /// 货号	自贸专区商品完成自贸区货物备案后，归并关系中的企业内部货号
        /// </summary>
        [DataMember]
        public string Product_SKUNO { get; set; }

        /// <summary>
        /// 物资序号	自贸专区商品完成自贸区货物备案后，填写该值
        /// </summary>
        [DataMember]
        public string Supplies_Serial_No { get; set; }


        /// <summary>
        /// 申报数量
        /// </summary>
        [DataMember]
        public int? ApplyQty { get; set; }


        /// <summary>
        /// 净重
        /// </summary>
        [DataMember]
        public decimal? SuttleWeight { get; set; }

        /// <summary>
        /// 其他备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 物资序号	自贸专区商品完成自贸区货物备案后，填写该值
        /// </summary>
        [DataMember]
        public string Supplies_Serial_No_1 { get; set; }

        /// <summary>
        /// 税则编号
        /// </summary>
        [DataMember]
        public string TariffCode { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 备案信息
        /// </summary>
        [DataMember]
        public string EntryCode { get; set; }

        /// <summary>
        /// 存储方式
        /// </summary>
        [DataMember]
        public StoreType? StoreType { get; set; }

        /// <summary>
        /// •	进口检疫审批许可证确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark1 { get; set; }

        /// <summary>
        /// •	输出国或地区官方出具检疫证书确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark2 { get; set; }

        /// <summary>
        /// •	原产地证明确认（自贸）
        /// </summary>
        [DataMember]
        public string Remark3 { get; set; }

        /// <summary>
        /// •	品牌方授权确认（直邮）
        /// </summary>
        [DataMember]
        public string Remark4 { get; set; }

        /// <summary>
        ///商品其他名称
        /// </summary>
        [DataMember]
        public string ProductOthterName { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public string OrginName { get; set; }

        /// <summary>
        /// 缺省备货时间，天
        /// </summary>
        [DataMember]
        public int DefaultLeadTimeDays { get; set; }

        /// <summary>
        /// 实际备货时间，天
        /// </summary>
        [DataMember]
        public int? RealLeadTimeDays { get; set; }

        /// <summary>
        /// 贸易类型
        /// </summary>
        [DataMember]
        public TradeType ProductTradeType { get; set; }

        /// <summary>
        /// 是否需效期
        /// </summary>
        [DataMember]
        public int? NeedValid { get; set; }

        /// <summary>
        /// 是否需黏贴中文标签
        /// </summary>
        [DataMember]
        public int? NeedLabel { get; set; }

        /// <summary>
        /// 备案状态
        /// </summary>
        [DataMember]
        public ProductEntryStatus EntryStatus { get; set; }

        /// <summary>
        /// 备案扩展状态
        /// </summary>
        [DataMember]
        public ProductEntryStatusEx? EntryStatusEx { get; set; }

        /// <summary>
        /// 备案备注
        /// </summary>
        [DataMember]
        public string AuditNote { get; set; }

        /// <summary>
        /// 商检流水号
        /// </summary>
        [DataMember]
        public string InspectionNum { get; set; }

        /// <summary>
        /// HSCode
        /// </summary>
        [DataMember]
        public string HSCode { get; set; }

        /// <summary>
        /// 产品不属于我国禁止进境物
        /// </summary>
        [DataMember]
        public int? NotProhibitedEntry { get; set; }

        /// <summary>
        /// 产品不在1712号公告名录内
        /// </summary>
        [DataMember]
        public int? NotInNotice1712 { get; set; }

        /// <summary>
        /// 商品不属于转基因产品
        /// </summary>
        [DataMember]
        public int? NotTransgenic { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }
    }
}
