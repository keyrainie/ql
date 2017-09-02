using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    public partial class ProductInfo
    {
        /// <summary>
        /// 第一次上架时间
        /// </summary>
        [DataMember]
        public DateTime? FirstOnSaleDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        public DateTime? LastEditDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public UserInfo OperateUser { get; set; }
    }
}
