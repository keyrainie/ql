using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace GiveERPCustomerPoint.Entities
{
   public class ReturnSoItemInfo
    {
       [DataMapping("ProductSysNo", DbType.Int32)]
       public int ProductSysNo { get; set; }

       [DataMapping("ProductID", DbType.String)]
       public string ProductID { get; set; }

       [DataMapping("ProductName", DbType.String)]
       public string ProductName { get; set; }

       [DataMapping("BriefName", DbType.String)]
       public string BriefName { get; set; }

       [DataMapping("Quantity", DbType.Int32)]
       public int Quantity { get; set; }

       [DataMapping("BasicPrice", DbType.Decimal)]
       public Decimal BasicPrice { get; set; }

       [DataMapping("CurrentPrice", DbType.Decimal)]
       public decimal CurrentPrice { get; set; }

       [DataMapping("MerchantProductID", DbType.String)]
       public string MerchantProductID { get; set; }

       [DataMapping("ContractCode", DbType.String)]
       public string ContractCode { get; set; }

    }
}
