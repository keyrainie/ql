using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商附件信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorAttachInfo
    {
        /// <summary>
        /// 供应商申请表
        /// </summary>
       [DataMember]
        public string VendorApplyForm { get; set; }

        /// <summary>
        /// 组织机构代码证
        /// </summary>
       [DataMember]
       public string OrganizeCodeCertificate { get; set; }

        /// <summary>
        /// 企业法人营业执照
        /// </summary>
       [DataMember]
       public string EnterpriseBusinessLicence { get; set; }

        /// <summary>
        /// 税务登记证
        /// </summary>
       [DataMember]
       public string TaxationAffairsRegistration { get; set; }

        /// <summary>
        /// 经销协议
        /// </summary>
       [DataMember]
       public string AgreementBeingSold { get; set; }

        /// <summary>
        /// 代销协议
        /// </summary>
       [DataMember]
       public string AgreementConsign { get; set; }

        /// <summary>
        /// 售后协议
        /// </summary>
       [DataMember]
       public string AgreementAfterSold { get; set; }

        /// <summary>
        /// 其它协议
        /// </summary>
       [DataMember]
       public string AgreementOther { get; set; }

    }
}
