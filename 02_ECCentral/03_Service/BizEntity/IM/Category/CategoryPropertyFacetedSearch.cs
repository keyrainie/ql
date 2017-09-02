using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM.Category
{
    /// <summary>
    /// 高级搜索相关
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryPropertyFacetedSearch
    {
        /// <summary>
        /// 是否参与高级搜索（前台使用）
        /// </summary>
        [DataMember]
        public bool IsFacetedSearch { get; set; }

        /// <summary>
        /// 高级搜索显示优先级（前台使用）
        /// </summary>
        [DataMember]
        public int SearchPriority { get; set; }
    }
}
