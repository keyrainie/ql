using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ProductInfoViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string PromotionTitle { get; set; }
        public string ImageUrl { get; set; }
        public int ProductStatus { get; set; }
        public int ProductType { get; set; }
        public int ProductQuantity { get; set; }
    }
}