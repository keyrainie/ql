using System;
using System.Runtime.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.SO
{
    [Serializable]
    public class NetpayInfo : EntityBase
    {
        [DataMember]
        public int SOSysNo { get; set; }
        [DataMember]
        public int PayTypeSysNo { get; set; }
        [DataMember]
        public decimal PayAmount { get; set; }
        [DataMember]
        public int Source { get; set; }
        [DataMember]
        public NetPayStatusType Status { get; set; }
        [DataMember]
        public int? CurrencySysNo { get; set; }
        [DataMember]
        public decimal? OrderAmt { get; set; }
        [DataMember]
        public decimal? PrePayAmt { get; set; }
        [DataMember]
        public decimal? PointPayAmt { get; set; }
        [DataMember]
        public decimal? GiftCardPayAmt { get; set; }
    }
}
