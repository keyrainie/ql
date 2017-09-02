using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ECCentral.BizEntity.Customer;


namespace ECCentral.BizEntity.SO
{
    [Serializable]
    [DataContract]
    public class SOCustomerAuthentication : IIdentity
    {
        [DataMember]
        public int? SOSysNo { get; set; }

        [DataMember]
        public int? CustomerSysNo { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IDCardType? IDCardType { get; set; }

        [DataMember]
        public string IDCardNumber { get; set; }

        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public Gender? Gender { get; set; }

        [DataMember]
        public int? SysNo { get; set; }
    }
}
