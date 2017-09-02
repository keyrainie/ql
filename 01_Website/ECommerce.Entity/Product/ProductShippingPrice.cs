using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class ProductShippingPrice
    {
        public int ProductSysNo { get; set; }

        public int SellerSysNo { get; set; }

        public int StockSysNo { get; set; }

        public int ShipTypeSysNo { get; set; }

        public int Status { get; set; }

        public string ShipTypeName { get; set; }

        public int AreaSysNo { get; set; }

        public decimal UnitPrice { get; set; }

        public int BaseWeight { set; get; }
    }
}
