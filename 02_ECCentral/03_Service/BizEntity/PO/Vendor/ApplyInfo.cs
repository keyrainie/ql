using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO.Vendor
{
    //ChrisHe
    [Serializable]
    [DataContract]
    public class ApplyInfo
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int SellerSysNo { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string ApplicationType { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }
    }
}
