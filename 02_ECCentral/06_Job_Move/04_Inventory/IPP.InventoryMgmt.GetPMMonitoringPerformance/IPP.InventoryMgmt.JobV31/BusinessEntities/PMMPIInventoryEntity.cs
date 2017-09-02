using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    public class PMMPIInventoryEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("DisplayName", DbType.String)]
        public string DisplayName { get; set; }

        [DataMapping("Category1SysNo", DbType.Int32)]
        public int Category1SysNo { get; set; }

        [DataMapping("Category1Name", DbType.String)]
        public string Category1Name { get; set; }

        [DataMapping("Category2SysNo", DbType.Int32)]
        public int? Category2SysNo { get; set; }

        [DataMapping("Category2Name", DbType.String)]
        public string Category2Name { get; set; }

        [DataMapping("PMSysNo", DbType.Int32)]
        public int? PMSysNo { get; set; }

        [DataMapping("HotSaleProductShortageRate", DbType.Decimal)]
        public string HotSaleProductShortageRate { get; set; }

        [DataMapping("AllProductShortageRate", DbType.Decimal)]
        public string AllProductShortageRate { get; set; }

        [DataMapping("Losing", DbType.Decimal)]
        public decimal Losing { get; set; }

        public int IsOutOfStock { get; set; }
    }
}
