using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace SendMKTPointEmail.Biz.Entities
{

   public class CustomerPointInfoEntity:EntityBase
    {
       [DataMapping("CustomerID", DbType.String)]
       public string CustomerID { get; set; }

       [DataMapping("ValidScore", DbType.Int32)]
       public int ValidScore { get; set; }

       [DataMapping("PointLowerLimit", DbType.Int32)]
       public int PointLowerLimit { get; set; }
    }
}
