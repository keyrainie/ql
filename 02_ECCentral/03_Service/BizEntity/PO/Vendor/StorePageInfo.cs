using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 店铺网页的主信息。
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageInfo
    {
        private string m_CompanyCode = "8601";
        private string m_LanguageCode = "zh-CN";

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 关联PageType的Key
        /// </summary>
        [DataMember]
        public string PageTypeKey { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        [DataMember]
        public string PageName { get; set; }

        /// <summary>
        /// Page的JSON数据,草稿信息的保存,和PublicshedStorePageInfo.DataValue有区别
        /// </summary>
        [DataMember]
        public string DataValue { get; set; }

        /// <summary>
        /// 生成的页面链接URL
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 【枚举】状态，0=无效的，1=有效的
        /// </summary>
        [DataMember]
        public int? Status { get; set; }

        /// <summary>
        /// 创建者系统编号
        /// </summary>
        [DataMember]
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        [DataMember]
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        [DataMember]
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        [DataMember]
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        [DataMember]
        public int? SellerSysNo { get; set; }

        /// <summary>
        /// 平台公司系统编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get { return m_CompanyCode; }
            set { m_CompanyCode = value; }
        }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode
        {
            get { return m_LanguageCode; }
            set { m_LanguageCode = value; }
        }
    }
}