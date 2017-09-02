using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 类别申请审核
    /// </summary>
    [DataContract]
    public class CategoryRequestApprovalInfo : ICompany, ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 审核人编号
        /// </summary>
        [DataMember]
        public int? AuditUserSysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public CategoryAuditStatus? Status { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        [DataMember]
        public int? CategorySysNo { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public OperationType? OperationType { get; set; }

        /// <summary>
        /// 分类状态
        /// </summary>
        [DataMember]
        public CategoryStatus CategoryStatus { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// 类别类型
        /// </summary>
        [DataMember]
        public CategoryType CategoryType { get; set; }

        /// <summary>
        /// 上级编号
        /// </summary>
        [DataMember]
        public int? ParentSysNumber { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
        public string CategoryID { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        [DataMember]
        public string Reasons { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string C3Code
        { get; set; }
    }
}
