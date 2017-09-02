using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
   public class ProductAccessoriesQueryConditionInfo
    {

        /// <summary>
        /// 条件
        /// </summary>
       public AccessoriesQueryConditionInfo Condition { get; set; }

        /// <summary>
        /// 父级条件
        /// </summary>
       public AccessoriesQueryConditionInfo ParentCondition { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public PriorityType? Priority { get; set; }

       
      
    }
}
