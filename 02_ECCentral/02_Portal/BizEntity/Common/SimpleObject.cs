using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 简单对象，用于统一处理各Domain中传递简单数据，相当于简单通用的DTO，主要是考虑性能及数据大小问题
    /// </summary>
    [Serializable]
    [DataContract]
    public class SimpleObject
    {
        public SimpleObject() { }

        public SimpleObject(int? sysNo) : this(sysNo, null) { }

        public SimpleObject(int? sysNo, string name) : this(sysNo, null, name) { }

        public SimpleObject(int? sysNo, string id, string name)
        {
            SysNo = sysNo;
            ID = id;
            Name = name;
        }

        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

}
