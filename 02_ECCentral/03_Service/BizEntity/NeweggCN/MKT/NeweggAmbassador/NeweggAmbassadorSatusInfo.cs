using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Enum;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT
{
    [Serializable]
    [DataContract]
    public class NeweggAmbassadorSatusInfo
    {
        [DataMember]
        public int? AmbassadorSysNo { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public AmbassadorStatus? OrignCustomerMark { get; set; }  //'大使状态'-- [1：未激活；2：已激活]

        
    }
}
