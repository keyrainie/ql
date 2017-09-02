using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品进口信息
    /// </summary>
    public class ProductEntryInfo
    {
        
        public int SysNo { get; set; }

        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品英文名称
        /// </summary>
        public string ProductName_EN { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }


        /// <summary>
        /// 功能
        /// </summary>
        public string Functions { get; set; }

        /// <summary>
        /// 成份
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }


        /// <summary>
        /// 用途
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 计税单位
        /// </summary>
        public string TaxUnit { get; set; }


        /// <summary>
        /// 申报单位
        /// </summary>
        public string ApplyUnit { get; set; }

        /// <summary>
        /// 计税单位数量
        /// </summary>
        public decimal TaxQty { get; set; }


        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public EntryBizType BizType { get; set; }


        /// <summary>
        /// 申报关区
        /// </summary>
        public int ApplyDistrict { get; set; }
        /// <summary>
        /// 货号	自贸专区商品完成自贸区货物备案后，归并关系中的企业内部货号
        /// </summary>
        public string Product_SKUNO { get; set; }
        /// <summary>
        /// 物资序号	自贸专区商品完成自贸区货物备案后，填写该值
        /// </summary>
        public string Supplies_Serial_No { get; set; }
        /// <summary>
        /// 申报数量
        /// </summary>
        public int ApplyQty { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        public decimal SuttleWeight { get; set; }

        /// <summary>
        /// 其他备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 物资序号	自贸专区商品完成自贸区货物备案后，填写该值
        /// </summary>
        public string Supplies_Serial_No_1 { get; set; }

        /// <summary>
        /// 税则编号
        /// </summary>
        public string TariffCode { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 备案信息
        /// </summary>
        public string EntryCode { get; set; }

        /// <summary>
        /// 存储方式
        /// </summary>
        public ProductStoreType ProductStoreType { get; set; }

        /// <summary>
        /// •	进口检疫审批许可证确认（自贸）
        /// </summary>
        public string Remark1 { get; set; }

        /// <summary>
        /// 输出国或地区官方出具检疫证书确认（自贸）
        /// </summary>
        public string Remark2 { get; set; }

        /// <summary>
        /// 原产地证明确认（自贸）
        /// </summary>
        public string Remark3 { get; set; }

        /// <summary>
        /// 品牌方授权确认（直邮）
        /// </summary>
        public string Remark4 { get; set; }

        /// <summary>
        ///商品其他名称
        /// </summary>
        public string ProductOthterName { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        public DateTime ManufactureDate { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        public string OrginName { get; set; }


        public SourceCompany SourceCompany { get; set; }

        public string OriginCountryName { get; set; }

        /// <summary>
        /// 备货时间，多少天，RealLeadTimeDays不存在即取DefaultLeadTimeDays
        /// </summary>
        public int LeadTimeDays { get; set; }
    }
}
