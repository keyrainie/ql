using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Store
{
    [Serializable]
    [DataContract]
    public class StoreDomainPage
    {
        [DataMember]
        public int SellerSysNo { get; set; }

        [DataMember]
        public int HomePageSysNo { get; set; }

        [DataMember]
        public string SecondDomain { get; set; }
    }
}
