using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace IPP.ContentMgmt.SyncShopPrice.Entities
{
    [Serializable]
    public class ProductShopPriceEntity
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
        [DataMapping("ShopPrice", DbType.Decimal)]
        public decimal ShopPrice
        {
            get;
            set;
        }
    }
}
