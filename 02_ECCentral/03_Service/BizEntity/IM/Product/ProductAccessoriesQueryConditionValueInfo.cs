using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.IM
{
   public class ProductAccessoriesQueryConditionValueInfo
    {
       /// <summary>
       /// 选项值
       /// </summary>
       public string ConditionValue { get; set; }
       /// <summary>
       /// 条件编号
       /// </summary>
       public int ConditionSysNo { get; set; }
      /// <summary>
      /// 父节点选项值
      /// </summary>
       public int ConditionValueParentSysNo { get; set; }

      /// <summary>
      /// 查询功能编号
      /// </summary>
       public int MasterSysNo { get; set; }

       /// <summary>
       /// 编号
       /// </summary>
       public int SysNo { get; set; }
    }
}
