using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 货到付款 的付款方式以及金额
    /// </summary>
    [Serializable]
    [DataContract]
    public class PosInfo
    {
        /// <summary>
        /// 预付款
        /// </summary>
        [DataMember]
        public decimal? PosPrePay { get; set; }
        /// <summary>
        /// 现金
        /// </summary>
        [DataMember]
        public decimal? PosCash { get; set; }
        /// <summary>
        /// 银行卡
        /// </summary>
        [DataMember]
        public decimal? PosBankCard { get; set; }
    }
}
