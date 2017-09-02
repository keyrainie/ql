using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA跟进日志
    /// </summary>
    public class InternalMemoInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 单件系统编号
        /// </summary>
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public InternalMemoStatus? Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 来源（未用）
        /// </summary>
        public int? SourceSysNo { get; set; }

        /// <summary>
        /// ReasonCode系统编号
        /// </summary>
        public int? ReasonCodeSysNo { get; set; }

        /// <summary>
        /// 提醒时间（未用）
        /// </summary>
        public DateTime? RemindTime { get; set; }

        /// <summary>
        /// 责任部门（未用）
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 是否紧急（未用）
        /// </summary>
        public int? Urgency { get; set; }

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
    }
}
