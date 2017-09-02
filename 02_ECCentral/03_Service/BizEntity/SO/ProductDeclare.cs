using ECCentral.BizEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 申报商品详细信息
    /// </summary>
    public class ProductDeclare
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 品牌（中文）
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 品牌（英文）
        /// </summary>
        public string BrandNameEN { get; set; }
        /// <summary>
        /// 商品名称（中文）
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品名称（英文）
        /// </summary>
        public string ProductNameEN { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string ProductMode { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 功能
        /// </summary>
        public string Functions { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string Purpose { get; set; }
        /// <summary>
        /// 成份
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        public DateTime ManufactureDate { get; set; }
        /// <summary>
        /// 单位（计税单位）
        /// </summary>
        public string TaxUnit { get; set; }
        /// <summary>
        /// 单位数量
        /// </summary>
        public int TaxQty { get; set; }
        /// <summary>
        /// 单价（人民币）
        /// </summary>
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 毛重（KG）
        /// </summary>
        public string GrossWeight { get; set; }
        /// <summary>
        /// 净重（KG）
        /// </summary>
        public string SuttleWeight { get; set; }
        /// <summary>
        /// 贸易类型
        /// </summary>
        public TradeType BizType { get; set; }
        /// <summary>
        /// 货号
        /// </summary>
        public string ProductSKUNO { get; set; }
        /// <summary>
        /// 物资序号
        /// </summary>
        public string SuppliesSerialNo { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 申报数量
        /// </summary>
        public int ApplyQty { get; set; }
        /// <summary>
        /// 关区代码
        /// </summary>
        public string CustomsCode { get; set; }
    }
    /// <summary>
    /// 待申报商品信息
    /// </summary>
    public class WaitDeclareProduct
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }
    }
    public class DeclareProductResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message { get; set; }
    }
}
