using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    public class GetGiftItemByMasterProductsReq
    {
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<int> MasterProductSysNoList { get; set; }
    }
}
