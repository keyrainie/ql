using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Product.Models
{
   public class ProductInfoFilter
    {
       public int ProductSysNo { get; set; }
       public string ProductID { get; set; }
       public int ProductCommonInfoSysNo { get; set; }
       public string CompanyCode { get; set; }
       public string LaguageCode { get; set; }
       public string StoreCompanyCode { get; set; }
    }
}
