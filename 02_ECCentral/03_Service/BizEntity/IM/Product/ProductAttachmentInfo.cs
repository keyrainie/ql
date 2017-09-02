using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品附件信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductAttachmentInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 附件商品信息
        /// </summary>
        [DataMember]
        public ProductInfo AttachmentProduct { get; set; }

        /// <summary>
        /// 附件数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }
    }
}
