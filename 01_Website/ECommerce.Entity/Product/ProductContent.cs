using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
   public class ProductContent
    {

       public ProductContentType ContentType { get; set; }

       public string ContentName
       {
           get
           {
              return  this.ContentType.GetDescription();
           }
       }

       public string Content { get; set; }
    }
}
