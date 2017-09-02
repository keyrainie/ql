using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商账期信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorPayTermsItemInfo
    {
        /// <summary>
        /// 账期系统编号
        /// </summary>
        [DataMember]
        public int? PayTermsNo { get; set; }

        /// <summary>
        /// 是否代销
        /// </summary>
        [DataMember]
        public int? IsConsignment { get; set; }

        /// <summary>
        /// 账期计算公式
        /// </summary>
        [DataMember]
        public string DiscribComputer { get; set; }

        /// <summary>
        /// 账期名称描述
        /// </summary>
        [DataMember]
        public string PayTermsName { get; set; }

    }
}
