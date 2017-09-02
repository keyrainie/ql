using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class BatchUpdateAvatarStatusReq
    {
        public List<int> CustomerSysNoList { get; set; }

        public AvtarShowStatus AvtarImageStatus { get; set; }

    }
}
