using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 送修返还信息
    /// </summary>
    public class RegisterResponseInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 返还件机身编号
        /// </summary>
        public string ResponseProductNo { get; set; }

        /// <summary>
        /// 送修返还结果类型
        /// </summary>
        public string VendorRepairResultType { get; set; }

        /// <summary>
        /// 送修返还描述
        /// </summary>
        public string ResponseDesc { get; set; }

        /// <summary>
        /// 返还人系统编号
        /// </summary>
        public int? ResponseUserSysNo { get; set; }

        /// <summary>
        /// 返还人姓名
        /// </summary>
        public string ResponseUserName { get; set; }

        /// <summary>
        /// 返还时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }
    }
}
