using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 默认退换货信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DefaultRMAPolicyInfo : IIdentity
    {
        [DataMember]
        public int? SysNo { get; set; }
        [DataMember]
        public int? C3SysNo { get; set; }
        [DataMember]
        public int? BrandSysNo { get; set; }
        [DataMember]
        public int? RMAPolicySysNo{ get; set; }
        [DataMember]
        public UserInfo CreateUser { get; set; }
        [DataMember]
        public UserInfo EditUser { get; set; }
        [DataMember]
        public List<Int32> SysNos { get; set; }
    }
}
