using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品质保信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductWarrantyInfo
    {
        /// <summary>
        /// 主机保修期（天）
        /// </summary>
        [DataMember]
        public int HostWarrantyDay { get; set; }

        /// <summary>
        /// 零部件保修期（天）
        /// </summary>
        [DataMember]
        public int PartWarrantyDay { get; set; }

        /// <summary>
        /// 保修细则
        /// </summary>
        [DataMember]
        public LanguageContent Warranty { get; set; }

        /// <summary>
        /// 厂商售后电话
        /// </summary>
        [DataMember]
        public String ServicePhone { get; set; }

        /// <summary>
        /// 售后服务中心网址
        /// </summary>
        [DataMember]
        public String ServiceInfo { get; set; }

        /// <summary>
        /// 提供增值税发票
        /// </summary>
        [DataMember]
        public OfferVATInvoice OfferVATInvoice { get; set; }

        /// <summary>
        /// 质保信息显示
        /// </summary>
        [DataMember]
        public WarrantyShow WarrantyShow { get; set; }

        /// <summary>
        /// 是否不提供延保
        /// </summary>
        [DataMember]
        public bool IsNoExtendWarranty { get; set; }

    }
}
