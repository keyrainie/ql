using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品销售区域批量信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductSalesAreaBatchInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public int StockSysNo { get; set; }

        /// <summary>
        /// 省份编号
        /// </summary>
        [DataMember]
        public int ProvinceSysNo { get; set; }
    }
}
