using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 推荐品牌
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandRecommendedInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int Sysno { get; set; }

        /// <summary>
        /// 品牌集合，用半角逗号分隔多个
        /// </summary>
        [DataMember]
        public string BrandRank { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int BrandSysNo { get; set; }

        [DataMember]
        public UserInfo User { get; set; }

        /// <summary>
        /// 0=首页，1=一级栏目推荐，2=二级栏目推荐
        /// </summary>
        [DataMember]
        public int Level_No { get; set; }

        /// <summary>
        /// Level_No=1时，存一级ECCategorySysNo；=2时，存放2级ECCategorySysNo；=0时，存0
        /// </summary>
        [DataMember]
        public int Level_Code { get; set; }

        /// <summary>
        /// 分组标签
        /// </summary>
        [DataMember]
        public string Level_Name { get; set; }
    }
}
