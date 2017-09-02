using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 资源信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResourceForNewegg
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? ResourceSysNo { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        [DataMember]
        public int ProductGroupSysNo { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        [DataMember]
        public ResourcesType Type { get; set; }

        /// <summary>
        /// 资源地址
        /// </summary>
        [DataMember]
        public string ResourceURL { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductResourceStatus Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [DataMember]
        public string TemporaryName { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public UserInfo OperateUser { get; set; }
    }
}
