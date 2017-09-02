using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商历史操作记录信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorHistoryLog : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? LogSysNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        public int? VendorSysNo { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        [DataMember]
        public DateTime? HistoryDate { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        [DataMember]
        public string HistoryReason { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public VendorModifyRequestType? RequestType { get; set; }

        /// <summary>
        /// 供应商操作类型
        /// </summary>
        [DataMember]
        public VendorModifyActionType? ActionType { get; set; }

        /// <summary>
        /// 展示名称
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 日志信息创建人编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [DataMember]
        public VendorModifyRequestStatus? Status { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 审核人名称
        /// </summary>
        [DataMember]
        public string AuditUserName { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

    }
}
