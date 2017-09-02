using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.Entity;
using IPP.InventoryMgmt.JobV31.DataAccess;

namespace IPP.InventoryMgmt.Taobao.JobV31.Provider
{
    public class ProductQueryProvider
    {
        public static Dictionary<int, List<ProductEntity>> QueryByStock(QueryConditionEntity<QueryProduct> query)
        {
            Dictionary<int, List<ProductEntity>> dic = new Dictionary<int, List<ProductEntity>>();
            List<ProductEntity> list = Query(query);
            foreach (ProductEntity product in list)
            {
                int wareHourseNumber = product.WarehouseNumber;
                if (!dic.ContainsKey(wareHourseNumber))
                {
                    dic.Add(wareHourseNumber, new List<ProductEntity>());
                }
                dic[wareHourseNumber].Add(product);
            }
            return dic;
        }

        public static List<ProductEntity> Query(QueryConditionEntity<QueryProduct> query)
        {
            return ProductDA.Query(query);
        }
    }
}
