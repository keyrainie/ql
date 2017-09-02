using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPProductInfo
    {
        /// <summary>
        /// 系统商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// ERP ProductID
        /// </summary>
        [DataMember]
        public int? ERPProductID { get; set; }

        /// <summary>
        /// 库存模式
        /// </summary>
        [DataMember]
        public ProductInventoryType? InventoryType { get; set; }
    }
}
