using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class SOItemInfoModel
    {
        public int SOSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductTitle { get; set; }

        public int Quantity { get; set; }

        public int Weight { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public int Point { get; set; }

        public int PointType { get; set; }

        public decimal DiscountAmt { get; set; }

        public decimal OriginalPrice { get; set; }

        public string Warranty { get; set; }

        public SOItemType ProductType { get; set; }

        public string DefaultImage { get; set; }

        public int GiftID { get; set; }

        public GroupPropertyInfo GroupPropertyInfo { get; set; }

        //public decimal TariffRate { get; set; }
        public decimal PromotionDiscount { get; set; }
        public decimal TariffAmt { get; set; }
        public decimal TariffPrice
        {
            get { return TariffAmt; }
        }
        /// <summary>
        /// 订单所属商家
        /// </summary>
        public int MerchantSysNo { set; get; }
    }
}