using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 检测信息
    /// </summary>
    public class RegisterCheckInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 检测结果类型
        /// </summary>
        public string InspectionResultType { get; set; }

        /// <summary>
        /// 检测人系统编号
        /// </summary>
        public int? CheckUserSysNo { get; set; }

        /// <summary>
        /// 做Check时操作员姓名
        /// </summary>
        public string CheckUserName { get; set; }        

        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime? CheckTime { get; set; }

        /// <summary>
        /// 检测描述
        /// </summary>
        public string CheckDesc { get; set; }

        /// <summary>
        /// 是否建议退款
        /// </summary>
        public bool? IsRecommendRefund { get; set; }
    }
}
