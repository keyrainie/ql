using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 类别信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryInfo : IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 父级类别SysNo
        /// </summary>
        [DataMember]
        public int? ParentSysNumber { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [DataMember]
        public LanguageContent CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        [DataMember]
        public CategoryStatus Status { get; set; }

        /// <summary>
        /// 基于类别设置指标
        /// </summary>
        [DataMember]
        public CategorySetting CategorySetting { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        [DataMember]
        public string CategoryID { get; set; }

        /// <summary>
        /// 操作类型 
        /// </summary>
        [DataMember]
        public OperationType? OperationType { get; set; }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string C3Code { get; set; }
    }
}
