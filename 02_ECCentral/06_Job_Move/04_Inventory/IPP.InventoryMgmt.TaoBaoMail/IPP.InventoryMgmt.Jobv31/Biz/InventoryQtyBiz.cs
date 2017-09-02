using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.DA;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class InventoryQtyBiz
    {
        public static List<ThirdPartInventoryEntity> QueryProduct()
        {
            int records = InventoryQtyDA.QueryRecordsCount();
            int page = 1;
            int pageSize = 100;
            int pageCount = records <= pageSize ? 1 : (records % pageSize == 0 ? records / pageSize : records / pageSize + 1);
            IEnumerable<ThirdPartInventoryEntity> list = new List<ThirdPartInventoryEntity>();
            for (; page <= pageCount; page++)
            {
                list = list.Union(InventoryQtyDA.Query(pageSize, page));
            }
            IEqualityComparer<ThirdPartInventoryEntity> compare = new ThirdPartInventoryCompare();
            return list.Distinct(compare).ToList();
        }

        private class ThirdPartInventoryCompare : IEqualityComparer<ThirdPartInventoryEntity>
        {
            public bool Equals(ThirdPartInventoryEntity x, ThirdPartInventoryEntity y)
            {
                if (x != null && y != null)
                {
                    return x.ProductMappingSysno == y.ProductMappingSysno;
                }
                return x == y;
            }

            public int GetHashCode(ThirdPartInventoryEntity obj)
            {
                return obj.GetHashCode();
            }
        }

    }
}
