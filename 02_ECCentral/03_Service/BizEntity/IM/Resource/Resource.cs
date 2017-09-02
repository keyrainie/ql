using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 资源信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class Resource : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

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
        /// 资源数据
        /// </summary>
        [DataMember]
        public byte[] ResourceData { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// 临时文件名
        /// </summary>
        [DataMember]
        public string TemporaryName { get; set; }
    }
}
