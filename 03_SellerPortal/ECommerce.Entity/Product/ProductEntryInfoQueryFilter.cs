using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 报关信息查询条件
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductEntryInfoQueryFilter : QueryFilter
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public int SellerSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 货号
        /// </summary>
        [DataMember]
        public string ProductSKUNO { get; set; }
        /// <summary>
        /// 物资序号
        /// </summary>
        [DataMember]
        public string SuppliesSerialNo { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public string ManufactureDateBegin { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public string ManufactureDateEnd { get; set; }
        /// <summary>
        /// 备案状态
        /// </summary>
        [DataMember]
        public int? EntryStatus { get; set; }
    }
}
