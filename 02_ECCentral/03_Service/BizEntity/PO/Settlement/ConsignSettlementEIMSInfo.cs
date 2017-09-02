using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代销结算单EIMS信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsignSettlementEIMSInfo
    {

        /// <summary>
        /// PM系统编号
        /// </summary>
        [DataMember]
        public int? PMSysNo { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        [DataMember]
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 返点系统编号
        /// </summary>
        [DataMember]
        public int? ReturnPointSysNo { get; set; }

        /// <summary>
        /// 返点名称
        /// </summary>
        [DataMember]
        public string ReturnPointName { get; set; }

        /// <summary>
        /// 返点数
        /// </summary>
        [DataMember]
        public decimal? ReturnPoint { get; set; }

        /// <summary>
        /// 已使用返点数
        /// </summary>
        [DataMember]
        public decimal? UsingReturnPoint { get; set; }

        /// <summary>
        /// 剩余返点数
        /// </summary>
        [DataMember]
        public decimal? RemnantReturnPoint { get; set; }

    }
}
