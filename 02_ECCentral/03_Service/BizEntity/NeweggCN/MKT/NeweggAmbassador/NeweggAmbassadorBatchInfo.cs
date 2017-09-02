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
    public class NeweggAmbassadorBatchInfo
    {
        public NeweggAmbassadorBatchInfo()
        {
            NeweggAmbassadors = new List<NeweggAmbassadorSatusInfo>();
        }

        [DataMember]
        public List<NeweggAmbassadorSatusInfo> NeweggAmbassadors { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }
        
    }
}
