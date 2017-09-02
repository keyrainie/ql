using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{
    [Serializable]
    [DataContract]
    public class CategoryInfo
    {
        [DataMember]
        public int SysNo { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
    }
}
