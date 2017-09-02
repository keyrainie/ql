using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity;

namespace ECCentral.WPMessage.BizEntity
{
    [Serializable]
    [DataContract]
    public class WPMessageCategory
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public string SystemCode { get; set; }
        [DataMember]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 是否需要手动关闭
        /// </summary>
        [DataMember]
        public bool IsManual { get; set; }
    }
}
