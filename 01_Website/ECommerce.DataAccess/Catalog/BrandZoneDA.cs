using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Catalog
{
    public class BrandZoneDA
    {
        public static List<ProductItemInfo> GetAllHotProductList()
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Catalog_GetBrandZoneHotProductList");
            return dataCommand.ExecuteEntityList<ProductItemInfo>();
        }
    }
}
