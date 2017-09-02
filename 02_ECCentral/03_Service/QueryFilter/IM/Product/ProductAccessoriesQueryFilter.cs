using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;


namespace ECCentral.QueryFilter.IM
{
   public class ProductAccessoriesQueryFilter
    {

       /// <summary>
       /// 页面包
       /// </summary>
       public PagingInfo PagingInfo { get; set; }

       /// <summary>
       /// 配件功能名称
       /// </summary>
       public string AccessoriesQueryName { get; set; }

       /// <summary>
       /// 状态
       /// </summary>
       public ValidStatus? Status { get; set; }

       /// <summary>
       /// 创建人
       /// </summary>
       public string CreateUserName { get; set; }

       /// <summary>
       /// 创建起始时间
       /// </summary>
       public DateTime? CreateDateFrom { get; set; }

       /// <summary>
       /// 结束时间
       /// </summary>
       public DateTime? CreateDateTo { get; set; }
    }
}
