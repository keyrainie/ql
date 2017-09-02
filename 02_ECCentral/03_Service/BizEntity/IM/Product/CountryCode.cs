using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM.Product
{
    [Serializable]
    [DataContract]
    public class CountryCode
    {
        [DataMember]
        public int? SysNo { get; set; }


        [DataMember]
        public string CountryName { get; set; }


        [DataMember]
        public string CounttryCode { get; set; }
    }
}
