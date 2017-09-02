using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品维护同组商品
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductMaintainGroupProduct
    {
        /// <summary>
        /// 分组属性列表
        /// </summary>
        [DataMember]
        public List<CategoryPropertyInfo> SplitGroupProperties { get; set; }
        /// <summary>
        /// 分组属性值列表
        /// </summary>
        [DataMember]
        public List<CategoryPropertyValueInfo> SplitGroupPropertyValues { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<ProductMaintainInfo> ProductList { get; set; }

        public ProductMaintainGroupProduct()
        {
            this.SplitGroupProperties = new List<CategoryPropertyInfo>();
            this.SplitGroupPropertyValues = new List<CategoryPropertyValueInfo>();
            this.ProductList = new List<ProductMaintainInfo>();
        }
    }
    
    /// <summary>
    /// 商品维护同组商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductMaintainGroupProductInfo : EntityBase
    {
        [DataMember]
        public int ProductGroupSysNo { get; set; }
        [DataMember]
        public string PropertyGroupName { get; set; }
        [DataMember]
        public int PropertySysNo1 { get; set; }
        [DataMember]
        public string PropertyName1 { get; set; }
        [DataMember]
        public int ValueSysNo1 { get; set; }
        [DataMember]
        public string ValueName1 { get; set; }
        [DataMember]
        public int PropertySysNo2 { get; set; }
        [DataMember]
        public string PropertyName2 { get; set; }
        [DataMember]
        public int ValueSysNo2 { get; set; }
        [DataMember]
        public string ValueName2 { get; set; }
    }
}
