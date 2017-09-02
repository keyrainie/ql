using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 公司信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class Company
    {
        /// <summary>
        /// 公司代码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; } 
    }
}
