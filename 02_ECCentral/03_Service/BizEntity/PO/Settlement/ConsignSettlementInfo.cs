using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代销结算单
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsignSettlementInfo : SettleRequestBase
    {
        /// <summary>
        /// 3级类别返点系统编号
        /// </summary>
        [DataMember]
        public int? ReturnPointC3SysNo { get; set; }

        /// <summary>
        /// PM返点系统编号
        /// </summary>
        [DataMember]
        public int? PM_ReturnPointSysNo { get; set; }

        /// <summary>
        /// 返点信息
        /// </summary>
        [DataMember]
        public ConsignSettlementEIMSInfo EIMSInfo { get; set; }

        /// <summary>
        /// 代销结算商品财务记录列表
        /// </summary>
        [DataMember]
        public List<ConsignSettlementItemInfo> ConsignSettlementItemInfoList { get; set; }


        /// <summary>
        /// 是否高级PM 用于PM产品线相关验证  BY Jack.W.Wang 2012-11-8 CRL21776
        /// </summary>
        [DataMember]
        public bool? IsManagerPM { get; set; }

        /// <summary>
        /// 结算单商品所属商品线 用于结算单查询  BY Jack.W.Wang 2012-11-8 CRL21776
        /// </summary>
        [DataMember]
        public int? ProductLineSysNo { get; set; }

        /// <summary>
        /// 扣款金额
        /// </summary>
        [DataMember]
        public decimal DeductAmt { get; set; }

        [DataMember]
        public AccountType? DeductAccountType { get; set; }

        [DataMember]
        public DeductMethod? DeductMethod { get; set; }

        [DataMember]
        public string ConsignRange { get; set; }
    }
}
