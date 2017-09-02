using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.WebFramework;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品库存调整单Entity
    /// </summary>
    public class ProductStockAdjustInfo
    {
        public int? SysNo { get; set; }
        public ProductStockAdjustStatus? Status { get; set; }
        public int? StockSysNo { get; set; }
        public int? CurrencyCode { get; set; }
        public string Memo { get; set; }
        public int? VendorSysNo { get; set; }
        public int? InUserSysNo { get; set; }
        public DateTime? InDate { get; set; }
        public int? AuditUserSysNo { get; set; }
        public DateTime? AuditDate { get; set; }
        public int? EditUserSysNo { get; set; }
        public DateTime? EditDate { get; set; }

        public List<ProductStockAdjustItemInfo> AdjustItemList { get; set; }

        public ProductStockAdjustInfo()
        {
            this.AdjustItemList = new List<ProductStockAdjustItemInfo>();
        }
    }

    public class ProductStockAdjustViewInfo : ProductStockAdjustInfo
    {
        public string StockName { get; set; }
        public string StatusText
        {
            get
            {
                return Status.HasValue ? Status.Value.GetEnumDescription() : string.Empty;
            }
        }
        public string CurrencyCodeText
        {
            get

            { return CurrencyCode == 1 ? "人民币" : string.Empty; }
        }
        public string InUserName { get; set; }
        public string AuditUserName { get; set; }
        public string EditUserName { get; set; }

        public string InDateText
        {
            get
            {
                return InDate.HasValue ? InDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }
        public string AuditDateText
        {
            get
            {
                return AuditDate.HasValue ? AuditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }
        public string EditDateText
        {
            get
            {
                return EditDate.HasValue ? EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }
    }

    public class ProductStockAdjustItemInfo
    {
        public int? SysNo { get; set; }
        public int? AdjustSysNo { get; set; }
        public int? ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductTitle { get; set; }
        public int? AvailableQty { get; set; }
        public int? OnlineQty { get; set; }
        public decimal? CurrentPrice { get; set; }
        public int? AdjustQty { get; set; }
        public int? InUserSysNo { get; set; }
        public DateTime? InDate { get; set; }
    }
}
