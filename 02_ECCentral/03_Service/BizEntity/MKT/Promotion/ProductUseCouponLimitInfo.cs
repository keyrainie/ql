using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    public class ProductUseCouponLimitInfo
    {
       /// <summary>
       /// 系统编号
       /// </summary>
       public int SysNo { get; set; }

       /// <summary>
       /// 用户
       /// </summary>
       public UserInfo User { get; set; }

       /// <summary>
       /// 商品ID
       /// </summary>
       public string ProductId { get; set; }
       /// <summary>
       /// 类型
       /// </summary>
       public CouponLimitType? CouponLimitType { get; set; }

    

    }
}
