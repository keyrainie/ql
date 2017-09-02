using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.IM
{
  public class ProductShowQueryFilter
    {
        /// <summary>
        /// 页面包
        /// </summary>
         public PagingInfo PageInfo { get; set; }

      /// <summary>
      /// 首次上架开始时间
      /// </summary>
        public DateTime? FirstOnlineTimeFrom { get; set; }
      /// <summary>
      /// 首次上架结束时间
      /// </summary>
        public DateTime? FirstOnlineTimeTo { get; set; }

      /// <summary>
      /// 更新时间开始时间
      /// </summary>
        public DateTime? EditDateFrom { get; set; }
      /// <summary>
      /// 更新时间结束时间 
      /// </summary>
        public DateTime? EditDateTo { get; set; }
      
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

      /// <summary>
      /// 状态
      /// </summary>
        public ProductStatus? Status { get; set; }
    }
}
