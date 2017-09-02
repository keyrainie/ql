using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace IPP.ContentMgmt.SyncShopPrice.Entities
{
    [Serializable]
    public class ProductPriceEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public string ProductSysNo
        {
            get;
            set;
        }
        [DataMapping("ProductID", DbType.String)]
        public string ProductID
        {
            get;
            set;
        }
        [DataMapping("Price", DbType.Decimal)]
        public decimal Price
        {
            get;
            set;
        }
    }
}
