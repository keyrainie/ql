using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 商户信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class Merchant : IIdentity
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        [DataMember]
        public int? MerchantID
        {
            get;
            set;
        }
        /// <summary>
        /// 商户名称
        /// </summary>
        [DataMember]
        public string MerchantName
        {
            get;
            set;
        }
        /// <summary>
        /// 商户系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 商家类型（原:供应商开票方式）
        /// </summary>
        [DataMember]
        public VendorInvoiceType VendorInvoiceType { get; set; }

        /// <summary>
        /// 导购URL
        /// </summary>
        [DataMember]
        public string ShoppingGuideURL
        {
            get;
            set;
        }
    }
}
