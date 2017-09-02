using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO.PurchaseOrder
{
    /// <summary>
    /// 扣款项维护信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class Deduct
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 扣款类型
        /// </summary>
        [DataMember]
        public DeductType? DeductType { get; set; }

        /// <summary>
        /// 记账类型
        /// </summary>
        [DataMember]
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// 扣款方式
        /// </summary>
        [DataMember]
        public DeductMethod? DeductMethod { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public Status? Status { get; set; }
        /// <summary>
        ///生成日期
        /// </summary>
        [DataMember]
        public DateTime InDate { get; set; }

        /// <summary>
        ///创建人
        /// </summary>
        [DataMember]
        public int InUser { get; set; }

        /// <summary>
        ///编辑人
        /// </summary>
        [DataMember]
        public int? EditUser { get; set; }

        /// <summary>
        ///编辑时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }
    }
}
