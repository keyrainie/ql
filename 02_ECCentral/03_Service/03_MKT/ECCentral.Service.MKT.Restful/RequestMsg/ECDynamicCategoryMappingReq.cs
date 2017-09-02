using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    public class ECDynamicCategoryMappingReq
    {
        public int? DynamicCategorySysNo { get; set; }
        public List<int> ProductSysNoList { get; set; }
    }   
}
