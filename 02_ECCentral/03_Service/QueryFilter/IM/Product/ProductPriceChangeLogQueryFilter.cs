using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class ProductPriceChangeLogQueryFilter
    {
       public PagingInfo PageInfo { get; set; }
       /// <summary>
       /// 商品系统编号
       /// </summary>
       public string ProductSysno { get; set; }
       /// <summary>
       /// 商品ID
       /// </summary>
       public string ProductID { get; set; }
       
       /// <summary>
       /// 查询开始时间
       /// </summary>
       public DateTime? CreateDateFrom { get; set; }
       /// <summary>
       /// 查询结束时间
       /// </summary>
       public DateTime? CreateDateTo { get; set; }

       /// <summary>
       /// 类型
       /// </summary>
       public string PriceLogType { get; set; }
    }
}
