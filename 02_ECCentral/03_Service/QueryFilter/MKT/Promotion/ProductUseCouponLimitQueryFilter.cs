using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
  public  class ProductUseCouponLimitQueryFilter
    {
      /// <summary>
      /// 商品编号
      /// </summary>
      public int? ProductSysNo { get; set; }
      /// <summary>
      /// 状态
      /// </summary>
      public ADStatus? Status { get; set; }

      /// <summary>
      /// 类型
      /// </summary>
      public CouponLimitType? CouponLimitType { get; set; }

      /// <summary>
      /// 也面包
      /// </summary>
      public PagingInfo PageInfo { get; set; }
    }
}
