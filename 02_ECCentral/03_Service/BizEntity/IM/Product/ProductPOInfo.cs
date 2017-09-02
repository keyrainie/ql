using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM.Product
{
    /// <summary>
    /// 商品采购信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductPOInfo
    {
        /// <summary>
        /// 最小包装数量
        /// </summary>
        [DataMember]
        public int MinPackNumber { get; set; }

        /// <summary>
        /// 采购备注
        /// </summary>
        [DataMember]
        public string POMemo { get; set; }

        /// <summary>
        /// 最后采购Vendor
        /// </summary>
        [DataMember]
        public int? LastVendorSysNo { get; set; }

        /// <summary>
        /// 库存模式
        /// </summary>
        [DataMember]
        public ProductInventoryType InventoryType { get; set; }

        /// <summary>
        /// ERP大类码ID
        /// </summary>
        [DataMember]
        public int? ERPProductID { get; set; }
    }
}
