using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 动态类别
    /// </summary>
    [DataContract]
    [Serializable]
    public class ECDynamicCategory : IIdentity
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? Level { get; set; }       

        [DataMember]
        public DynamicCategoryStatus? Status { get; set; }

        [DataMember]
        public int? Priority { get; set; }

        [DataMember]
        public int? ParentSysNo { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public bool IsShow { get; set; }

        [DataMember]
        public DynamicCategoryType? CategoryType { get; set; }

        [DataMember]
        public List<ECDynamicCategory> SubCategories { get; set; }
    }
}
