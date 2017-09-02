using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace SendMailARAmtOfVIPCustomer.Biz.Entities
{
   public class ARAmtOfVIPCustomerEntity:EntityBase
    {
       [DataMapping("CustomerID", DbType.String)]
       public string CustomerID { get; set; }

       [DataMapping("CustomerName", DbType.String)]
       public string CustomerName { get; set; }

       [DataMapping("ArAMT", DbType.Decimal)]
       public decimal ArAMT { get; set; }
       
    }
}
