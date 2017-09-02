using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品卡使用日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class GiftCardRedeemLog
    {
        [DataMember]
        public int? TransactionNumber { get; set; }

        /// <summary>
        /// 礼品卡编号
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 顾客系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [DataMember]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 使用单据的编号
        /// </summary>
        [DataMember]
        public int? ActionSysNo { get; set; }

        /// <summary>
        /// 类型，比如：SO
        /// </summary>
        [DataMember]
        public ActionType ActionType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ValidStatus Status { get; set; }

        /// <summary>
        /// 礼品卡总金额
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 可用金额
        /// </summary>
        [DataMember]
        public decimal AvailAmount { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public CardMaterialType Type { get; set; }
    }
}
