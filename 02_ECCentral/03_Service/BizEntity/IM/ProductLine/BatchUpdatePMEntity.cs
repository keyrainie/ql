using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class BatchUpdatePMEntity:ICompany
    {
        [DataMember]
        public int? PMUserSysNo { get; set; }

        [DataMember]
        public int? MerchandiserSysNo { get; set; }

        [DataMember]
        public string BackupPMSysNoList { get; set; }

        [DataMember]
        public bool IsEmptyC2Create { get; set; }

        [DataMember]
        public string BakPMUpdateType { get; set; }

        [DataMember]
        public List<ProductLineInfo> ProductLineList { get; set; }

        [DataMember]
        public List<int?> BackupPMList { get; set; }

        [DataMember]
        public string InUser { get; set; }
        
        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string LanguageCode { get; set; }
    }
}
