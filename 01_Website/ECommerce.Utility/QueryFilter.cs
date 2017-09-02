using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Utility
{
    [DataContract]
    [Serializable]
    public abstract class QueryFilter
    {
        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string SortFields { get; set; }
    }
}
