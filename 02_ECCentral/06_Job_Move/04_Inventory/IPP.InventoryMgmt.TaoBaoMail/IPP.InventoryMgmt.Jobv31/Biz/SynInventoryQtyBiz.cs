using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.Common;
using IPP.InventoryMgmt.JobV31.Service;
using Newegg.Oversea.Framework.ExceptionHandler;
using System.Threading;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class SynInventoryQtyBiz
    {
        public static List<TaobaoProduct> QuerySynProduct()
        {
            IEnumerable<TaobaoProduct> list = new List<TaobaoProduct>();
            QueryTaobaoProduct QueryInventory = new QueryTaobaoProduct(TaoBaoService.QueryInventory);
            QueryTaobaoProduct QueryOnSale = new QueryTaobaoProduct(TaoBaoService.QueryOnSale);
            IAsyncResult QueryOnSale_Result = QueryOnSale.BeginInvoke(null, null);
            IAsyncResult QueryInventory_Result = QueryInventory.BeginInvoke(null, null);
            List<TaobaoProduct> inventory_result = QueryInventory.EndInvoke(QueryInventory_Result);
            List<TaobaoProduct> onsale_result = QueryOnSale.EndInvoke(QueryOnSale_Result);
            list = list.Union(inventory_result);
            list = list.Union(onsale_result);
            IEqualityComparer<TaobaoProduct> compare = new TaobaoProductCompare();
            return list.Distinct(compare).ToList();
        }


        private class TaobaoProductCompare : IEqualityComparer<TaobaoProduct>
        {
            public bool Equals(TaobaoProduct x, TaobaoProduct y)
            {
                if (x != null && y != null)
                {
                    return x.NumberID == y.NumberID;
                }
                return x == y;
            }

            public int GetHashCode(TaobaoProduct obj)
            {
                return obj.GetHashCode();
            }

        }

    }
}
