using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Common
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

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 简单对象ID编号
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// 简单对象名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }


        [DataMember]
        public string BakString1 { get; set; }

        [DataMember]
        public string BakString2 { get; set; }

        [DataMember]
        public string BakString3 { get; set; }

        [DataMember]
        public int? BakInt1 { get; set; }

        [DataMember]
        public decimal? BakDecimal { get; set; }


        public override string ToString()
        {
            return ID.ToString();
        }
    }


    public class SimpleObjectEqualityComparer : IEqualityComparer<SimpleObject>
    {

        public bool Equals(SimpleObject x, SimpleObject y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }
            return x.ID.Equals(y.ID, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(SimpleObject obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
