using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    public class ProductPriceCompareInvalidReq
    {
        public int SysNo { get; set; }

        public string CommaSeperatedReasonCodes { get; set; }
    }
}
