using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.RMA.Restful.ResponseMsg
{
    public class RegisterMemoRsp
    {
        public int RegisterSysNo { get; set; }

        public string Memo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string VendorName { get; set; }

    }
}
