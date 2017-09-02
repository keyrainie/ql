using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 基于类别设置的配件
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryAccessory : IIdentity
    {
        /// <summary>
        /// 配件
        /// </summary>
        [DataMember]
        public AccessoryInfo Accessory { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public CategoryAccessoriesStatus Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int? Priority { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [DataMember]
        public IsDefault IsDefault { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
    }
}
