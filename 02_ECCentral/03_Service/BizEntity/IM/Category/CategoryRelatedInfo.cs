using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 相关分类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryRelatedInfo : ICompany, IIdentity, ILanguage
    {
        /// <summary>
        /// 三级类别编号1
        /// </summary>
        [DataMember]
        public int? C3SysNo1 { get; set; }

        /// <summary>
        /// 三级类别编号2
        /// </summary>
        [DataMember]
        public int? C3SysNo2 { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int? Priority { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode
        {
            get;

            set;

        }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
    }
}
