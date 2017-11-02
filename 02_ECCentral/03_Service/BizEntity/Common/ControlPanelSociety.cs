using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace ECCentral.BizEntity.Common
{

    [Serializable]
    [DataContract]
    public class ControlPanelSociety
    {
        [DataMember]
        public int? OrganizationID { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public string Province { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public string CommissionID { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
    [Serializable]
    [DataContract]
    public class ComBoxData
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
