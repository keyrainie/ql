using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品扩展信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductExtInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 是否允许退货
        /// </summary>
        [DataMember]
        public int IsPermitRefund { get; set; }

        /// <summary>
        /// 批次管理信息
        /// </summary>
        [DataMember]
        public ProductBatchManagementInfo ProductBatchManagementInfo { get; set; }
    }
}
