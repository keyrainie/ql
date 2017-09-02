using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity
{
    [Serializable]
    [DataContract]
    public class QueryResult<T> 
    {
        [DataMember]
        public List<T> ResultList { get; set; }

         [DataMember]
        public PageInfo PageInfo { get; set; }
    }
}
