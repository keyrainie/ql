using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{

    /// <summary>
    ///品牌报备
    /// </summary>
    [Serializable]
    [DataContract]
    public class StoreBrandFilingInfo
    {

        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商家(卖家)系统编号
        /// </summary>
        [DataMember]
        public int? SellerSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///品牌编号
        /// </summary>
        [DataMember]
        public int? BrandSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商检号
        /// </summary>
        [DataMember]
        public string InspectionNo
        {
            get;
            set;
        }

        /// <summary>
        ///代理等级
        /// </summary>
        [DataMember]
        public string AgentLevel
        {
            get;
            set;
        }

        /// <summary>
        ///0:无效
        ///1:草稿
        ///2:审核通过
        /// </summary>
        [DataMember]
        public StoreBrandFilingStatus? Staus
        {
            get;
            set;
        }

        /// <summary>
        /// 创建者系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }
        
        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? EditDate { get; set; }

    
        private string m_CompanyCode = "8601";
        /// <summary>
        /// 平台公司系统编号
        /// </summary>
        public string CompanyCode
        {
            get
            {
                return m_CompanyCode;
            }
            set
            {
                m_CompanyCode = value;
            }
        }

        private string m_LanguageCode = "zh-CN";
        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode
        {
            get
            {
                return m_LanguageCode;
            }
            set
            {
                m_LanguageCode = value;
            }
        }

    }
}
