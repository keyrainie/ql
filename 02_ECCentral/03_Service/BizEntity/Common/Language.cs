using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    [Serializable]
    [DataContract]
    public class Language
    {

        [DataMember]
        public string LanguageName { get; set; }

        [DataMember]
        public string LanguageCode { get; set; }

        [DataMember]
        public string ShortCode { get; set; }

        /// <summary>
        /// 1=有效，0=无效
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// 1=默认，0=非默认
        /// </summary>
        [DataMember]
        public int IsDefault { get; set; }
    }
}
