using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class ProductMapping
    {
        [DataMember]
        public ThirdPartner ThirdPartner { get; set; }

        [DataMember]
        public string SynProductID { get; set; }

        [DataMember]
        public StockRules StockRules { get; set; }

        [DataMember]
        public int LimitQty { get; set; }

        [DataMember]
        public ProductMappingStatus Status { get; set; }
    }
}
