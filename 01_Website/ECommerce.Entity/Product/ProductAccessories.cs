using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
   public class ProductAccessories
    {
       public int SysNo { get; set; }

       public int ProductSysNo { get; set; }

       public string AccessoriesID { get; set; }

       public string AccessoriesName { get; set; }

       public int Quantity { get; set; }

       public int Priority { get; set; }

       public int Status { get; set; }

    }
}
