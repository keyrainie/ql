using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class CategoryRequestApprovalQueryFilter
    {   
       /// <summary>
       /// 类别
       /// </summary>
         public CategoryType? Category { get; set; }
       
       /// <summary>
       /// 页面包
       /// </summary>
         public PagingInfo PagingInfo { get; set; }
       

    }
}
