using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 属性信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PropertyGroupInfo : IIdentity
    {

        /// <summary>
        ///  属性组标题（前台使用）
        /// </summary>
        [DataMember]
        public LanguageContent PropertyGroupName { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

    }
}
