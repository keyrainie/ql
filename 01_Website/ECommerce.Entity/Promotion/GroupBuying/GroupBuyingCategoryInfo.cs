using System;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Promotion.GroupBuying
{
    /// <summary>
    /// 团购类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class GroupBuyingCategoryInfo : EntityBase
    {
        [DataMember]
        public int SysNo { get; set; }
        [DataMember]
        public int CategoryType { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Status { get; set; }
        /// <summary>
        /// 是否是热门团购
        /// </summary>
        [DataMember]
        public int IsHotKey { get; set; }
    }
}
