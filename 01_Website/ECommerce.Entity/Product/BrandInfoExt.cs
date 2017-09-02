using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{

    public class BrandInfoExt:BrandInfo
    {
        [DataMember]
        public int ECSysNo { get; set; }
    }
}
