using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 商家审核时传的参数
    /// </summary>
    public class VendorApproveInfo
    {
        /// <summary>
        /// 商家系统编号
        /// </summary>
        [DataMember]
        public int VendorSysNo { get; set; }

        /// <summary>
        /// 商家状态
        /// </summary>
        [DataMember]
        public VendorStatus? VendorStatus { get; set; }

        /// <summary>
        /// 操作者系统编号
        /// </summary>
        [DataMember]
        public int? UserSysNo { get; set; }

        /// <summary>
        /// 操作者名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
    }
}
