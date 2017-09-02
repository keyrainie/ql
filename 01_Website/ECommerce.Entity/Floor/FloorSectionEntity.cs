using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    /// <summary>
    /// 首页楼层Tab实体
    /// </summary>
    [Serializable]
    public class FloorSectionEntity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// Tab 名称
        /// </summary>
        public string SectionName { get; set; }
        /// <summary>
        /// Tab 优先级
        /// </summary>
        public int Priority { get; set; }
    }
}
