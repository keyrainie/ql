using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品卡操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class GiftCardOperateLog
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
        public string Status { get; set; }

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

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public string EditUser { get; set; }
    }
}
