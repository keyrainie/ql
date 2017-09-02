using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    [Serializable]
    [DataContract]
    public class TariffInfo : IIdentity, ILanguage
    {
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 父类别
        /// </summary>
        [DataMember]
        public int? ParentSysNo { get; set; }
        /// <summary>
        /// 税号
        /// </summary>
        [DataMember]
        public string Tariffcode { get; set; }
        /// <summary>
        /// 税号删除后面的0，以便查询
        /// </summary>
        [DataMember]
        public string Tcode { get; set; }
        /// <summary>
        /// 品名及规格
        /// </summary>
        [DataMember]
        public string ItemCategoryName { get; set; }
        /// <summary>
        /// 0:可用，1：不可用
        /// </summary>
        [DataMember]
        public TariffStatus? Status { get; set; }
        /// <summary>
        /// 单位（如千克，个）
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 完税价格（RMB:元）
        /// </summary>
        [DataMember]
        public decimal? TariffPrice { get; set; }
        /// <summary>
        /// 税率(存入百分比数字，例如，如果是10%，直接存入10)
        /// </summary>
        [DataMember]
        public int? TariffRate { get; set; }
        /// <summary>
        /// 创建者Id
        /// </summary>
        [DataMember]
        public int? InUserSysNo { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        [DataMember]
        public string InUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }
        /// <summary>
        /// 编辑者Id
        /// </summary>
        [DataMember]
        public int? EditUserSysNo { get; set; }
        /// <summary>
        /// 编辑者
        /// </summary>
        [DataMember]
        public string EditUserName { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }
    }
}
