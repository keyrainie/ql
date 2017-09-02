using ECommerce.Enums;
namespace ECommerce.Entity.SO
{
    public class SOItemInfo
    {
        public int SysNo { get; set; }

        public int SOSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string MasterProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductTitle { get; set; }

        public int Quantity { get; set; }

        public int Weight { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public decimal UnitCostWithoutTax { get; set; }

        public int Point { get; set; }

        public int PointType { get; set; }

        public decimal DiscountAmt { get; set; }

        public decimal OriginalPrice { get; set; }

        public string Warranty { get; set; }

        public SOItemType ProductType { get; set; }

        public string DefaultImage { get; set; }

        public int GiftID { get; set; }

        // public GroupPropertyInfo GroupPropertyInfo { get; set; }

        public decimal TariffRate { get; set; }
        public decimal PromotionDiscount { get; set; }
        public decimal TariffAmt { get; set; }

        public decimal TariffPrice
        {
            get { return TariffAmt; }
        }

        /// <summary>
        /// 商品仓库编号
        /// </summary>
        public int? WarehouseNumber
        {
            get;
            set;
        }
    }
}