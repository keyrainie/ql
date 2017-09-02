using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品滞销库存信息
    /// </summary>
    public class UnmarketabelInventoryInfo
    {
        /// <summary>
        /// 只用于页面级的显示 所以 数据类型 基本全部定义为 String
        /// </summary>
        public string ProductID { get; set; }
        
        public string ProductSysNo { get; set; }
        
        public string DateRange { get; set; }

        public string SUMQuantity { get; set; }

        public Decimal SUMPrice { get; set; }

        public int MAXInStockDays { get; set; }

        public string UnSale0015DaysQty { get; set; }

        public string UnSale0015DaysPrice { get; set; }

        public string UnSale1630DaysQty { get; set; }

        public string UnSale1630DaysPrice { get; set; }

        public string UnSale3145DaysQty { get; set; }

        public string UnSale3145DaysPrice { get; set; }

        public string UnSale4660DaysQty { get; set; }

        public string UnSale4660DaysPrice { get; set; }

        public string UnSale61900DaysQty { get; set; }

        public string UnSale6190DaysPrice { get; set; }

        public string UnSale91120DaysQty { get; set; }

        public string UnSale91120DaysPrice { get; set; }

        public string UnSale121150DaysQty { get; set; }

        public string UnSale121150DaysPrice { get; set; }

        public string UnSale151180DaysQty { get; set; }

        public string UnSale151180DaysPrice { get; set; }

        public string UnSale181360DaysQty { get; set; }

        public string UnSale181360DaysPrice { get; set; }

        public string UnSale361720DaysQty { get; set; }

        public string UnSale361720DaysPrice { get; set; }

        public string UnSaleUP720DaysQty { get; set; }

        public string UnSaleUP720DaysPrice { get; set; }

        public string UnSale0030DaysQty { get; set; }

        public string UnSale0030DaysPrice { get; set; }

        public string UnSale3160DaysQty { get; set; }

        public string UnSale3160DaysPrice { get; set; }

        public string UnSale121180DaysQty { get; set; }

        public string UnSale121180DaysPrice { get; set; }

        public string UnSaleUP180DaysQty { get; set; }

        public string UnSaleUP180DaysPrice { get; set; }
    }
}
