using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPCustomerInfo
    {
        [DataMember]
        public string ADDRESS { get; set; }

        [DataMember]
        public string ADDRESS_PYM { get; set; }

        [DataMember]
        public string BZ { get; set; }

        [DataMember]
        public string BZ_ADDRESS { get; set; }

        [DataMember]
        public string DJR { get; set; }

        [DataMember]
        public string DJRQ { get; set; }

        [DataMember]
        public int? JLBH { get; set; }

        [DataMember]
        public string LSFS { get; set; }

        [DataMember]
        public string NAME { get; set; }

        [DataMember]
        public string NAME_PYM { get; set; }

        [DataMember]
        public string SHQY { get; set; }

    }
}
