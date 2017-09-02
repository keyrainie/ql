using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动在前台网站显示位置的条件 
    /// </summary>
    public class PSPagePositionSetting
    {
        /// <summary>
        /// 页面类型系统编码
        /// </summary>
        public int? PageTypeSysNo { get; set; }

        /// <summary>
        /// 页面类型下相关值的系统编码：如商品分类SysNo
        /// </summary>
        public int? PageTypeReferenceSysNo { get; set; }

        /// <summary>
        /// 位置系统编码
        /// </summary>
        public int? PositionSysNo { get; set; }
    }
}
