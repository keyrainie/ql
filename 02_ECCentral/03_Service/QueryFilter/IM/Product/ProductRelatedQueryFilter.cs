using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductRelatedQueryFilter
    {
       /// <summary>
       /// 页面包
       /// </summary>
       public PagingInfo PageInfo { get; set; }

       /// <summary>
       /// 主商品ID
       /// </summary>
       public int ProductSysNo { get; set; }

       /// <summary>
       /// 相关商品ID
       /// </summary>
       public int RelatedProductSysNo { get; set; }

       /// <summary>
       /// PMID
       /// </summary>
       public int PMUserSysNo { get; set; }
    }
}
