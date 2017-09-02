using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 单件退还信息
    /// </summary>
    public class RegisterReturnInfo
    {
        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 退还人系统编号
        /// </summary>
        public int? ReturnUserSysNo { get; set; }
        /// <summary>
        /// 退还人姓名
        /// </summary>
        public string ReturnUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 退还时间
        /// </summary>
        public DateTime? ReturnTime { get; set; }
    }
}
