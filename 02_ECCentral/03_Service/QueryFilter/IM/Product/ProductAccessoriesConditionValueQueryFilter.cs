using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class ProductAccessoriesConditionValueQueryFilter
    {

       public PagingInfo PagingInfo { get; set; }
       /// <summary>
       /// 条件的SysNo
       /// </summary>
       public int ConditionSysNo { get; set; }

       /// <summary>
       /// 选项值 
       /// </summary>
       public string ConditionValue { get; set; }

       public string IsTreeQuery { get; set; }

       public int MasterSysNo { get; set; } 
    }
}
