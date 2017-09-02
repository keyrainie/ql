using System;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Promotion.GroupBuying
{
    /// <summary>
    /// 团购查询信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class GroupBuyingQueryInfo : QueryFilterBase
    {
        /// <summary>
        /// 团购分类
        /// </summary>
        [DataMember]
        public int? CategorySysNo { get; set; }
        /// <summary>
        /// 排序类型
        /// 0-默认排序
        /// 10-销量升序
        /// 11-销量降序
        /// 20-价格升序
        /// 21-价格降序
        /// 30-评论升序
        /// 31-评论降序
        /// 40-上架时间升序
        /// 41-上架时间降序
        /// 50-团购结束时间升序
        /// 51-团购结束时间倒序
        /// </summary>
        [DataMember]
        public int SortType { get; set; }
        /// <summary>
        /// 0普通团购1虚拟团购
        /// </summary>
        [DataMember]
        public int? GroupBuyingTypeSysNo { get; set; }
    }
}
