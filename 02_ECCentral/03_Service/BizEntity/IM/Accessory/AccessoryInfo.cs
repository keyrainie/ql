using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 配件信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AccessoryInfo : IIdentity
    {
        /// <summary>
        /// 配件名称
        /// </summary>
        [DataMember]
        public string AccessoryID { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        [DataMember]
        public LanguageContent AccessoryName { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
    }
}
