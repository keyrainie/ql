using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.IM
{
  public  class AccessoriesQueryConditionInfo
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public string ConditionName { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public PriorityType? Priority { get; set; }

        /// <summary>
        /// 条件编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 选项值
        /// </summary>
        public string SelectItemValue { get; set; }

        /// <summary>
        /// 配件查询SysNO
        /// </summary>
        public int MasterSysNo { get; set; }
    }
}
