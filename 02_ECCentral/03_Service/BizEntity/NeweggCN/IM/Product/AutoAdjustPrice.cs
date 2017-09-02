using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class AutoAdjustPrice
    {
        [DataMember]
        public IsAutoAdjustPrice IsAutoAdjustPrice { get; set; }

        [DataMember]
        public DateTime? NotAutoPricingBeginDate { get; set; }

        [DataMember]
        public DateTime? NotAutoPricingEndDate { get; set; }
    }
}
