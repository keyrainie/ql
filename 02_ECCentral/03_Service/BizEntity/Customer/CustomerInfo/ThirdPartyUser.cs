using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 第三方用户
    /// </summary>
    [Serializable]
    [DataContract]
    public class ThirdPartyUser
    {
        [DataMember]
        public int CustomerSysNo { get; set; }

        [DataMember]
        public string CustomerID { get; set; }


        [DataMember]
        public string SubSource { get; set; }

        [DataMember]
        public string SourceName { get; set; }

        [DataMember]
        public string UserSource { get; set; }

    }
}
