using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Customer.Society
{
    /// <summary>
    /// 社团信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SocietyInfo
    {
        /// <summary>
        /// 社团编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 社团名字
        /// </summary>
        [DataMember]
        public string SocietyName { get; set; }
    }
}
