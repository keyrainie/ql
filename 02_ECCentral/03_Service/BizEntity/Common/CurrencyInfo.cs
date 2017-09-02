using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 货币信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CurrencyInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public string CurrencyID { get; set; }

        /// <summary>
        /// 货币名称
        /// </summary>
        [DataMember]
        public string CurrencyName { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        [DataMember]
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// 是否本地货币
        /// </summary>
        [DataMember]
        public bool? IsLocal { get; set; }

        /// <summary>
        /// 兑换本地货币汇率
        /// </summary>
        [DataMember]
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [DataMember]
        public int? ListOrder { get; set; }

        /// <summary>
        /// 货币状态
        /// </summary>
        [DataMember]
        public int? Status { get; set; }

        public string CurrencySymbolAndName { get; set; }

    }
}
